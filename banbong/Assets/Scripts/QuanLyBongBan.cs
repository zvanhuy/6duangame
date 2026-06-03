using UnityEngine;


// ============================================================
// GHI CHÚ CHỈNH SỬA NHANH
// File này quản lý bóng ở khu vực súng: bóng đang bắn và bóng xem trước.
// - Muốn đổi vị trí bóng xem trước: sửa viTriBongXemTruoc hoặc dòng new Vector2(2.05f, ...).
// - Muốn đổi kích thước bóng xem trước: tìm "banKinhBong * 1.55f".
// - Muốn bóng mới không lấy màu bóng xem trước: sửa hàm TaoBongMoiSauLanBan().
// ============================================================
/// <summary>
/// Quản lý 2 quả bóng ở khu vực súng:
/// 1. Bóng đang bắn: quả bóng người chơi chuẩn bị bắn.
/// 2. Bóng xem trước: quả bóng tiếp theo.
/// Script này chỉ tạo, đổi màu, đổi bóng sau mỗi lần bắn.
/// </summary>
public class QuanLyBongBan : MonoBehaviour
{
    [Header("Nhóm object")]
    [SerializeField] private Transform nhomSungBan;
    [SerializeField] private Transform nhomTrucXoaySung;

    [Header("Cấu hình bóng")]
    // Các thông số này thường được truyền từ QuanLyTroChoiBanBong.cs.
    // Sửa trong Inspector của "Quản lý trò chơi" sẽ ổn định hơn.
    [SerializeField] private float banKinhBong = 0.32f;
    [SerializeField] private Vector2 viTriSung = new Vector2(0f, -4.15f);

    // SỬA Ở ĐÂY nếu muốn đổi vị trí bóng xem trước.
    // X càng lớn bóng càng sang phải, Y càng lớn bóng càng lên cao.
    [SerializeField] private Vector2 viTriBongXemTruoc = new Vector2(2.05f, -4.0f);

    private Sprite[] spriteBong;
    private Bong bongDangBan;
    private Bong bongXemTruoc;

    public Bong BongDangBan => bongDangBan;
    public Bong BongXemTruoc => bongXemTruoc;

    public void CaiDat(Transform sungBan, Transform trucXoay, Sprite[] sprites, float banKinh, Vector2 viTriDauSung)
    {
        nhomSungBan = sungBan;
        nhomTrucXoaySung = trucXoay;
        spriteBong = sprites;
        banKinhBong = Mathf.Max(0.08f, banKinh);
        viTriSung = viTriDauSung;
        // SỬA Ở ĐÂY nếu muốn bóng xem trước nằm chỗ khác so với súng.
        // 2.05f là vị trí ngang bên phải, 0.15f là cao hơn súng một chút.
        viTriBongXemTruoc = new Vector2(2.05f, viTriSung.y + 0.15f);
    }

    public void TaoBongBanDau(bool dungLaiObjectCoSan)
    {
        bongDangBan = TimBongConTheoTen(nhomSungBan, "Bóng đang bắn");
        if (bongDangBan == null)
            bongDangBan = TaoBong("Bóng đang bắn", Random.Range(0, ThuVienHinhAnhBong.BangMauBong.Length), new Vector3(viTriSung.x, viTriSung.y, 0f), nhomSungBan, dungLaiObjectCoSan, banKinhBong * 2f);
        else
            ThuVienHinhAnhBong.CapNhatHinhAnhBong(bongDangBan, spriteBong, false, banKinhBong * 2f);

        bongDangBan.Hang = QuanLyLuoiBong.KhongCoBong;
        bongDangBan.Cot = QuanLyLuoiBong.KhongCoBong;
        bongDangBan.transform.position = new Vector3(viTriSung.x, viTriSung.y, 0f);
        TienIchBanBong.GanVaoNhom(bongDangBan.gameObject, nhomSungBan);

        bongXemTruoc = TimBongConTheoTen(nhomTrucXoaySung, "Bóng xem trước");
        if (bongXemTruoc == null)
            // SỬA Ở ĐÂY nếu muốn bóng xem trước to/nhỏ hơn.
            // 1.55f là hệ số kích thước của bóng xem trước.
            bongXemTruoc = TaoBong("Bóng xem trước", Random.Range(0, ThuVienHinhAnhBong.BangMauBong.Length), new Vector3(viTriBongXemTruoc.x, viTriBongXemTruoc.y, 0f), nhomTrucXoaySung, dungLaiObjectCoSan, banKinhBong * 1.55f);
        else
            ThuVienHinhAnhBong.CapNhatHinhAnhBong(bongXemTruoc, spriteBong, false, banKinhBong * 1.55f);

        bongXemTruoc.Hang = QuanLyLuoiBong.KhongCoBong;
        bongXemTruoc.Cot = QuanLyLuoiBong.KhongCoBong;
        bongXemTruoc.transform.position = new Vector3(viTriBongXemTruoc.x, viTriBongXemTruoc.y, 0f);
        TienIchBanBong.GanVaoNhom(bongXemTruoc.gameObject, nhomTrucXoaySung);
    }

    public void TaoBongMoiSauLanBan()
    {
        // Logic hiện tại:
        // Bóng đang bắn mới sẽ lấy màu của "Bóng xem trước".
        // Sau đó tạo một "Bóng xem trước" mới random màu.
        // Muốn bóng đang bắn luôn random, đổi dòng dưới thành:
        // int mauChoBongMoi = Random.Range(0, ThuVienHinhAnhBong.BangMauBong.Length);
        int mauChoBongMoi = bongXemTruoc != null ? bongXemTruoc.MaMau : Random.Range(0, ThuVienHinhAnhBong.BangMauBong.Length);

        if (bongXemTruoc != null)
            TienIchBanBong.HuyObject(bongXemTruoc.gameObject);

        // SỬA Ở ĐÂY nếu muốn đổi kích thước bóng đang bắn: thay hệ số 2f.
        bongDangBan = TaoBong("Bóng đang bắn", mauChoBongMoi, new Vector3(viTriSung.x, viTriSung.y, 0f), nhomSungBan, false, banKinhBong * 2f);

        // SỬA Ở ĐÂY nếu muốn đổi kích thước bóng xem trước: thay hệ số 1.55f.
        bongXemTruoc = TaoBong("Bóng xem trước", Random.Range(0, ThuVienHinhAnhBong.BangMauBong.Length), new Vector3(viTriBongXemTruoc.x, viTriBongXemTruoc.y, 0f), nhomTrucXoaySung, false, banKinhBong * 1.55f);
    }

    public void MatQuyenQuanLyBongDangBan()
    {
        bongDangBan = null;
    }

    private Bong TaoBong(string tenObject, int maMau, Vector3 viTri, Transform cha, bool dungLaiObjectCoSan, float kichCo)
    {
        GameObject obj = dungLaiObjectCoSan ? TienIchBanBong.LayHoacTaoCon(cha, tenObject) : new GameObject(tenObject);
        obj.name = tenObject;
        obj.transform.position = viTri;
        TienIchBanBong.GanVaoNhom(obj, cha);

        Bong bong = TienIchBanBong.LayHoacThem<Bong>(obj);
        bong.DatDuLieu(Mathf.Clamp(maMau, 0, ThuVienHinhAnhBong.BangMauBong.Length - 1), QuanLyLuoiBong.KhongCoBong, QuanLyLuoiBong.KhongCoBong);
        ThuVienHinhAnhBong.CapNhatHinhAnhBong(bong, spriteBong, false, kichCo);
        return bong;
    }

    private Bong TimBongConTheoTen(Transform nhomCha, string ten)
    {
        if (nhomCha == null)
            return null;

        Bong[] tatCaBong = nhomCha.GetComponentsInChildren<Bong>(true);
        foreach (Bong bong in tatCaBong)
        {
            if (bong != null && bong.name == ten)
                return bong;
        }
        return null;
    }
}
