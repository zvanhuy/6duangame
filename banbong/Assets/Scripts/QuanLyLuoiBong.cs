using System.Collections.Generic;
using UnityEngine;


// ============================================================
// GHI CHÚ CHỈNH SỬA NHANH
// File này quyết định LOGIC của lưới bóng.
// - Muốn đổi số bóng cần cùng màu để nổ: tìm "cum.Count >= 3".
// - Muốn đổi điểm khi nổ bóng: tìm "XoaCumBong(cum, 10)".
// - Muốn đổi điểm bóng rơi: tìm "XoaCumBong(bongRoi, 5)".
// - Muốn đổi khoảng cách giữa các bóng: tìm "khoangCach = banKinhBong * 2.05f".
// - Muốn đổi điều kiện thua: tìm hàm KiemTraThua().
// ============================================================
/// <summary>
/// Quản lý toàn bộ lưới bóng phía trên.
/// Script này lo các việc:
/// 1. Tạo bóng ban đầu trong lưới.
/// 2. Tìm ô gần nhất khi bóng bắn chạm lưới.
/// 3. Tìm cụm bóng cùng màu.
/// 4. Xóa cụm bóng từ 3 quả trở lên.
/// 5. Xóa bóng bị rơi tự do.
/// 6. Kiểm tra thắng / thua.
/// </summary>
public class QuanLyLuoiBong : MonoBehaviour
{
    public const int KhongCoBong = -1;

    [Header("Cấu hình lưới")]
    // Các giá trị này thường được truyền từ QuanLyTroChoiBanBong.cs qua hàm CaiDat().
    // Nếu muốn chỉnh ổn định nhất, sửa trong Inspector của object "Quản lý trò chơi".
    [SerializeField] private int soHang = 12;
    [SerializeField] private int soCot = 8;
    [SerializeField] private int soHangBanDau = 5;
    [SerializeField] private float banKinhBong = 0.32f;
    [SerializeField] private float viTriDinhLuoi = 4.05f;

    [Header("Nhóm chứa bóng trong Hierarchy")]
    [SerializeField] private Transform nhomLuoiBong;

    private Bong[,] luoiBong;
    private Sprite[] spriteBong;

    public int SoHang => soHang;
    public int SoCot => soCot;
    public float BanKinhBong => banKinhBong;

    public void CaiDat(Transform nhomLuoi, Sprite[] sprites, int hang, int cot, int hangBanDau, float banKinh, float dinhLuoi)
    {
        nhomLuoiBong = nhomLuoi;
        spriteBong = sprites;

        // Các dòng Mathf.Max/Clamp giúp tránh nhập sai trong Inspector.
        // Ví dụ soHang không được nhỏ hơn 1, banKinhBong không được quá nhỏ.
        soHang = Mathf.Max(1, hang);
        soCot = Mathf.Max(1, cot);
        soHangBanDau = Mathf.Clamp(hangBanDau, 1, soHang);
        banKinhBong = Mathf.Max(0.08f, banKinh);
        viTriDinhLuoi = dinhLuoi;

        // Mảng 2 chiều lưu bóng theo [hàng, cột].
        // Muốn đổi sang kiểu map khác thì phải sửa phần này và các hàm truy cập luoiBong[h, c].
        luoiBong = new Bong[soHang, soCot];
    }

    public void TaoHoacNapLuoiBanDau(bool dungLaiObjectCoSan)
    {
        if (luoiBong == null)
            luoiBong = new Bong[soHang, soCot];

        XoaDuLieuLuoiTamThoi();

        bool coBongCoSan = NapLuoiBongCoSanTuScene();
        if (!coBongCoSan)
            TaoLuoiBongBanDau(dungLaiObjectCoSan);
    }

    private void XoaDuLieuLuoiTamThoi()
    {
        for (int h = 0; h < soHang; h++)
        {
            for (int c = 0; c < soCot; c++)
                luoiBong[h, c] = null;
        }
    }

    private bool NapLuoiBongCoSanTuScene()
    {
        bool coBong = false;
        if (nhomLuoiBong == null)
            return false;

        Bong[] tatCaBong = nhomLuoiBong.GetComponentsInChildren<Bong>(true);
        foreach (Bong bong in tatCaBong)
        {
            if (bong == null || bong.Hang < 0 || bong.Hang >= soHang || bong.Cot < 0 || bong.Cot >= soCot)
                continue;

            bong.MaMau = Mathf.Clamp(bong.MaMau, 0, ThuVienHinhAnhBong.BangMauBong.Length - 1);
            bong.transform.position = LayViTriO(bong.Hang, bong.Cot);
            ThuVienHinhAnhBong.CapNhatHinhAnhBong(bong, spriteBong, true, banKinhBong * 2f);
            luoiBong[bong.Hang, bong.Cot] = bong;
            coBong = true;
        }

        return coBong;
    }

    private void TaoLuoiBongBanDau(bool dungLaiObjectCoSan)
    {
        for (int hang = 0; hang < soHangBanDau; hang++)
        {
            for (int cot = 0; cot < soCot; cot++)
            {
                string tenBong = TaoTenBongTrongLuoi(hang, cot);
                Bong bongCu = TimBongConTheoTen(nhomLuoiBong, tenBong);
                // SỬA Ở ĐÂY nếu muốn đổi cách random màu bóng ban đầu.
                // Hiện tại: nếu bóng đã có sẵn trong Scene thì giữ màu cũ; nếu chưa có thì random.
                // Muốn chỉ random 3 màu đầu: đổi Random.Range(0, ThuVienHinhAnhBong.BangMauBong.Length) thành Random.Range(0, 3).
                int maMau = bongCu != null
                    ? Mathf.Clamp(bongCu.MaMau, 0, ThuVienHinhAnhBong.BangMauBong.Length - 1)
                    : Random.Range(0, ThuVienHinhAnhBong.BangMauBong.Length);

                Bong bong = TaoBongTrongLuoi(tenBong, maMau, hang, cot, dungLaiObjectCoSan);
                luoiBong[hang, cot] = bong;
            }
        }
    }

    private Bong TaoBongTrongLuoi(string tenObject, int maMau, int hang, int cot, bool dungLaiObjectCoSan)
    {
        GameObject obj = dungLaiObjectCoSan ? TienIchBanBong.LayHoacTaoCon(nhomLuoiBong, tenObject) : new GameObject(tenObject);
        TienIchBanBong.GanVaoNhom(obj, nhomLuoiBong);
        obj.name = tenObject;
        obj.transform.position = LayViTriO(hang, cot);

        Bong bong = TienIchBanBong.LayHoacThem<Bong>(obj);
        bong.DatDuLieu(maMau, hang, cot);
        ThuVienHinhAnhBong.CapNhatHinhAnhBong(bong, spriteBong, true, banKinhBong * 2f);
        return bong;
    }

    public bool GanBongVaoLuoi(Bong bongDangBan, Vector3 viTriCham, out int diemCong, out bool daThang, out bool daThua, out string lyDoLoi)
    {
        diemCong = 0;
        daThang = false;
        daThua = false;
        lyDoLoi = string.Empty;

        if (bongDangBan == null)
        {
            lyDoLoi = "Không tìm thấy bóng đang bắn.";
            return false;
        }

        Vector2Int oGanNhat = TimOTrongGanNhat(viTriCham);
        if (oGanNhat.x < 0)
        {
            daThua = true;
            lyDoLoi = "Hết chỗ bắn.";
            return false;
        }

        int hang = oGanNhat.x;
        int cot = oGanNhat.y;
        bongDangBan.Hang = hang;
        bongDangBan.Cot = cot;
        bongDangBan.name = TaoTenBongTrongLuoi(hang, cot);
        bongDangBan.transform.position = LayViTriO(hang, cot);
        TienIchBanBong.GanVaoNhom(bongDangBan.gameObject, nhomLuoiBong);
        ThuVienHinhAnhBong.CapNhatHinhAnhBong(bongDangBan, spriteBong, true, banKinhBong * 2f);
        luoiBong[hang, cot] = bongDangBan;

        List<Vector2Int> cum = TimCumCungMau(hang, cot, bongDangBan.MaMau);

        // SỬA Ở ĐÂY nếu muốn đổi luật nổ bóng.
        // Hiện tại: cụm từ 3 bóng cùng màu trở lên sẽ nổ.
        // Muốn dễ hơn: đổi 3 thành 2. Muốn khó hơn: đổi 3 thành 4.
        if (cum.Count >= 3)
        {
            // SỬA Ở ĐÂY nếu muốn tăng/giảm điểm khi nổ bóng.
            // Số 10 là điểm mỗi quả bóng trong cụm bị xóa.
            // Ví dụ mỗi bóng 20 điểm: đổi XoaCumBong(cum, 10) thành XoaCumBong(cum, 20).
            diemCong += XoaCumBong(cum, 10);

            // Điểm bóng rơi tự do nằm trong hàm XoaBongRoiTuDo(), mặc định 5 điểm/quả.
            diemCong += XoaBongRoiTuDo();
        }

        daThua = KiemTraThua();
        // SỬA Ở ĐÂY nếu muốn đổi điều kiện thắng.
        // Hiện tại: thắng khi không còn quả bóng nào trong lưới.
        daThang = DemSoBongConLai() == 0;
        return true;
    }

    public bool ChamVaoBongTrongLuoi(Vector3 viTri)
    {
        // SỬA Ở ĐÂY nếu bóng va chạm quá sớm hoặc quá muộn.
        // Tăng 1.75f => bóng dễ chạm lưới hơn. Giảm 1.75f => bóng phải gần hơn mới chạm.
        float khoangCham = banKinhBong * 1.75f;

        for (int h = 0; h < soHang; h++)
        {
            for (int c = 0; c < soCot; c++)
            {
                Bong bong = luoiBong[h, c];
                if (bong == null)
                    continue;

                if (Vector2.Distance(viTri, bong.transform.position) <= khoangCham)
                    return true;
            }
        }

        return false;
    }

    public Vector3 LayViTriO(int hang, int cot)
    {
        // SỬA Ở ĐÂY nếu muốn đổi khoảng cách giữa các bóng.
        // 2.05f càng lớn thì bóng càng cách xa nhau, càng nhỏ thì bóng càng sát nhau.
        float khoangCach = banKinhBong * 2.05f;

        // 0.86f tạo kiểu lưới tổ ong. Tăng/giảm nhẹ sẽ đổi khoảng cách dọc giữa các hàng.
        float khoangDoc = khoangCach * 0.86f;

        // batDauX giúp căn giữa cả hàng bóng.
        float batDauX = -((soCot - 1) * khoangCach) / 2f;

        // leHang làm hàng lẻ lệch nửa ô để tạo lưới ziczac.
        // Muốn hàng thẳng như bảng vuông thì đổi cả 2 trường hợp về 0f.
        float leHang = (hang % 2 == 0) ? 0f : khoangCach * 0.5f;

        float x = batDauX + cot * khoangCach + leHang;
        float y = viTriDinhLuoi - hang * khoangDoc;
        return new Vector3(x, y, 0f);
    }

    private Vector2Int TimOTrongGanNhat(Vector3 viTri)
    {
        // Logic này quyết định bóng bắn sẽ được gắn vào ô trống nào.
        // Game sẽ tìm ô trống gần vị trí va chạm nhất.
        // Muốn gắn bóng theo hướng bắn hoặc theo hàng/cột cụ thể thì cần sửa hàm này.
        float khoangGanNhat = float.MaxValue;
        Vector2Int oTotNhat = new Vector2Int(-1, -1);

        for (int h = 0; h < soHang; h++)
        {
            for (int c = 0; c < soCot; c++)
            {
                if (luoiBong[h, c] != null)
                    continue;

                Vector3 viTriO = LayViTriO(h, c);
                float khoang = Vector2.Distance(viTri, viTriO);
                if (khoang < khoangGanNhat)
                {
                    khoangGanNhat = khoang;
                    oTotNhat = new Vector2Int(h, c);
                }
            }
        }

        return oTotNhat;
    }

    private List<Vector2Int> TimCumCungMau(int hangBatDau, int cotBatDau, int maMau)
    {
        // Đây là thuật toán BFS tìm tất cả bóng cùng màu nối liền với bóng vừa bắn.
        // Nếu sửa sai phần này, bóng có thể không nổ hoặc nổ nhầm màu.
        List<Vector2Int> ketQua = new List<Vector2Int>();
        bool[,] daTham = new bool[soHang, soCot];
        Queue<Vector2Int> hangDoi = new Queue<Vector2Int>();
        hangDoi.Enqueue(new Vector2Int(hangBatDau, cotBatDau));
        daTham[hangBatDau, cotBatDau] = true;

        while (hangDoi.Count > 0)
        {
            Vector2Int o = hangDoi.Dequeue();
            Bong bong = luoiBong[o.x, o.y];
            if (bong == null || bong.MaMau != maMau)
                continue;

            ketQua.Add(o);
            foreach (Vector2Int keBen in LayCacOKeBen(o.x, o.y))
            {
                if (!HopLe(keBen.x, keBen.y) || daTham[keBen.x, keBen.y])
                    continue;

                daTham[keBen.x, keBen.y] = true;
                hangDoi.Enqueue(keBen);
            }
        }

        return ketQua;
    }

    private int XoaCumBong(List<Vector2Int> cum, int diemMoiBong)
    {
        // diemMoiBong được truyền từ chỗ gọi hàm.
        // Ví dụ XoaCumBong(cum, 10) nghĩa là mỗi bóng bị xóa được 10 điểm.
        int diem = 0;
        foreach (Vector2Int o in cum)
        {
            Bong bong = luoiBong[o.x, o.y];
            if (bong != null)
                TienIchBanBong.HuyObject(bong.gameObject);

            luoiBong[o.x, o.y] = null;
            diem += diemMoiBong;
        }
        return diem;
    }

    private int XoaBongRoiTuDo()
    {
        bool[,] duocTreo = new bool[soHang, soCot];
        Queue<Vector2Int> hangDoi = new Queue<Vector2Int>();

        for (int c = 0; c < soCot; c++)
        {
            if (luoiBong[0, c] != null)
            {
                duocTreo[0, c] = true;
                hangDoi.Enqueue(new Vector2Int(0, c));
            }
        }

        while (hangDoi.Count > 0)
        {
            Vector2Int o = hangDoi.Dequeue();
            foreach (Vector2Int keBen in LayCacOKeBen(o.x, o.y))
            {
                if (!HopLe(keBen.x, keBen.y) || duocTreo[keBen.x, keBen.y] || luoiBong[keBen.x, keBen.y] == null)
                    continue;

                duocTreo[keBen.x, keBen.y] = true;
                hangDoi.Enqueue(keBen);
            }
        }

        List<Vector2Int> bongRoi = new List<Vector2Int>();
        for (int h = 0; h < soHang; h++)
        {
            for (int c = 0; c < soCot; c++)
            {
                if (luoiBong[h, c] != null && !duocTreo[h, c])
                    bongRoi.Add(new Vector2Int(h, c));
            }
        }

        // SỬA Ở ĐÂY nếu muốn tăng/giảm điểm bóng rơi tự do.
        // Số 5 là điểm mỗi quả không còn dính với trần và bị rơi.
        // Ví dụ muốn bóng rơi được 15 điểm/quả thì đổi 5 thành 15.
        return XoaCumBong(bongRoi, 5);
    }

    private List<Vector2Int> LayCacOKeBen(int hang, int cot)
    {
        // Logic ô kề bên của lưới tổ ong.
        // Không giống lưới vuông 4 hướng, bóng ở đây có tối đa 6 ô kề.
        // Nếu muốn đổi sang lưới vuông, phải sửa mảng buocChan/buocLe bên dưới.
        bool hangLe = hang % 2 == 1;
        int[,] buocChan = { { 0, -1 }, { 0, 1 }, { -1, -1 }, { -1, 0 }, { 1, -1 }, { 1, 0 } };
        int[,] buocLe = { { 0, -1 }, { 0, 1 }, { -1, 0 }, { -1, 1 }, { 1, 0 }, { 1, 1 } };
        int[,] buoc = hangLe ? buocLe : buocChan;

        List<Vector2Int> ketQua = new List<Vector2Int>();
        for (int i = 0; i < 6; i++)
            ketQua.Add(new Vector2Int(hang + buoc[i, 0], cot + buoc[i, 1]));

        return ketQua;
    }

    private bool HopLe(int hang, int cot)
    {
        return hang >= 0 && hang < soHang && cot >= 0 && cot < soCot;
    }

    private bool KiemTraThua()
    {
        // SỬA Ở ĐÂY nếu muốn đổi điều kiện thua.
        // Hiện tại: nếu bóng xuất hiện ở 2 hàng cuối thì thua.
        // Muốn dễ hơn: đổi soHang - 2 thành soHang - 1.
        // Muốn khó hơn: đổi soHang - 2 thành soHang - 3.
        for (int h = soHang - 2; h < soHang; h++)
        {
            for (int c = 0; c < soCot; c++)
            {
                if (luoiBong[h, c] != null)
                    return true;
            }
        }
        return false;
    }

    private int DemSoBongConLai()
    {
        int dem = 0;
        for (int h = 0; h < soHang; h++)
        {
            for (int c = 0; c < soCot; c++)
            {
                if (luoiBong[h, c] != null)
                    dem++;
            }
        }
        return dem;
    }

    private string TaoTenBongTrongLuoi(int hang, int cot)
    {
        return "Bóng trong lưới H" + (hang + 1).ToString("00") + " C" + (cot + 1).ToString("00");
    }

    private Bong TimBongConTheoTen(Transform nhomCha, string ten)
    {
        if (nhomCha == null)
            return null;

        for (int i = 0; i < nhomCha.childCount; i++)
        {
            Transform con = nhomCha.GetChild(i);
            if (con.name == ten)
                return con.GetComponent<Bong>();
        }
        return null;
    }
}
