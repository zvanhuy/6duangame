using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


// ============================================================
// GHI CHÚ CHỈNH SỬA NHANH
// Đây là file điều phối chính của game.
// Các dòng dễ sửa nhất nằm trong nhóm [Header] bên dưới.
// - Muốn tăng/giảm số hàng, số cột, số hàng ban đầu: sửa soHang, soCot, soHangBanDau.
// - Muốn bóng to/nhỏ: sửa banKinhBong.
// - Muốn súng bắn nhanh/chậm: sửa tocDoBan.
// - Muốn đổi vị trí súng: sửa viTriSung.
// - Muốn đổi chữ hướng dẫn: tìm "Kéo để ngắm - thả/click/Space để bắn".
// - Muốn đổi phím chơi lại: tìm hàm CoBamChoiLai().
// - Muốn đổi cách cộng điểm: mở QuanLyLuoiBong.cs, tìm "diemCong".
// ============================================================
/// <summary>
/// Script trung tâm của game bắn bóng.
/// Script này KHÔNG còn ôm toàn bộ code như bản cũ.
/// Nó chỉ làm nhiệm vụ điều phối các script con:
/// - TrinhTaoCauTrucBanBong: tạo Hierarchy tiếng Việt.
/// - QuanLyLuoiBong: xử lý lưới bóng.
/// - QuanLyBongBan: tạo bóng đang bắn và bóng xem trước.
/// - DieuKhienSung: điều khiển ngắm và bắn.
/// - QuanLyGiaoDien: cập nhật UI.
/// </summary>
[ExecuteAlways]
public class QuanLyTroChoiBanBong : MonoBehaviour
{
    [Header("Cấu hình lưới bóng")]
    // SỬA Ở ĐÂY:
    // soHang: tổng số hàng tối đa của lưới. Tăng số này => game có nhiều khoảng trống hơn trước khi thua.
    // soCot: số cột bóng mỗi hàng. Tăng số này => lưới rộng hơn, cần chỉnh lại camera/tường nếu quá lớn.
    // soHangBanDau: số hàng bóng xuất hiện lúc bắt đầu. Tăng => game khó hơn, giảm => game dễ hơn.
    // banKinhBong: kích thước bóng. Tăng => bóng to hơn và khoảng cách giữa các bóng cũng lớn hơn.
    [Tooltip("Tổng số hàng tối đa của lưới bóng. Tăng để có nhiều chỗ bắn hơn.")]
    [SerializeField] private int soHang = 12;
    [Tooltip("Số cột của mỗi hàng bóng. Tăng quá nhiều cần chỉnh lại giới hạn tường/camera.")]
    [SerializeField] private int soCot = 8;
    [Tooltip("Số hàng bóng có sẵn khi bắt đầu game. Tăng = khó hơn, giảm = dễ hơn.")]
    [SerializeField] private int soHangBanDau = 5;
    [Tooltip("Kích thước bóng. Tăng = bóng to hơn, giảm = bóng nhỏ hơn.")]
    [SerializeField] private float banKinhBong = 0.32f;

    [Header("Cấu hình khu vực chơi")]
    // SỬA Ở ĐÂY:
    // gioiHanTrai/gioiHanPhai: giới hạn 2 bên. Bóng chạm vào đây sẽ bật lại.
    // viTriDinhLuoi: vị trí hàng bóng đầu tiên ở phía trên.
    // viTriSung: vị trí súng ở dưới màn hình. Y càng âm thì súng càng thấp.
    [Tooltip("Giới hạn tường trái. Bóng chạm vào sẽ bật lại.")]
    [SerializeField] private float gioiHanTrai = -2.65f;
    [Tooltip("Giới hạn tường phải. Bóng chạm vào sẽ bật lại.")]
    [SerializeField] private float gioiHanPhai = 2.65f;
    [Tooltip("Độ cao của hàng bóng đầu tiên.")]
    [SerializeField] private float viTriDinhLuoi = 4.05f;
    [Tooltip("Vị trí súng bắn bóng. X đổi ngang, Y đổi cao/thấp.")]
    [SerializeField] private Vector2 viTriSung = new Vector2(0f, -4.15f);

    [Header("Cấu hình bắn")]
    // SỬA Ở ĐÂY:
    // Muốn bóng bay nhanh hơn thì tăng tocDoBan, muốn chậm hơn thì giảm.
    [Tooltip("Tốc độ bóng sau khi bắn. Tăng = bóng bay nhanh hơn.")]
    [SerializeField] private float tocDoBan = 8.5f;

    [Header("Hiển thị trước khi Play")]
    // SỬA Ở ĐÂY:
    // hienTatCaObjectTruocKhiPlay = true: object hiện sẵn trong Hierarchy khi chưa bấm Play.
    // Nếu máy bị lag khi chỉnh Inspector, có thể tắt tuDongCapNhatKhiSuaInspector.
    [Tooltip("Bật để tạo sẵn toàn bộ object trong Hierarchy trước khi bấm Play.")]
    [SerializeField] private bool hienTatCaObjectTruocKhiPlay = true;
    [Tooltip("Bật để Unity tự cập nhật lại Hierarchy khi sửa thông số trong Inspector.")]
    [SerializeField] private bool tuDongCapNhatKhiSuaInspector = true;

    private CauTrucCanhBanBong cauTruc;
    private QuanLyLuoiBong quanLyLuoiBong;
    private QuanLyBongBan quanLyBongBan;
    private DieuKhienSung dieuKhienSung;
    private QuanLyGiaoDien quanLyGiaoDien;
    private Sprite[] spriteBong;

    private int diem;
    private int capDo = 1;
    private int soBongDaBan;
    private bool daKetThuc;

#if UNITY_EDITOR
    private bool dangHenKhoiTaoEditor;
#endif

    private void OnEnable()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying && hienTatCaObjectTruocKhiPlay)
            HenKhoiTaoTrongEditor();
#endif
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying && hienTatCaObjectTruocKhiPlay && tuDongCapNhatKhiSuaInspector)
            HenKhoiTaoTrongEditor();
    }

    private void HenKhoiTaoTrongEditor()
    {
        if (dangHenKhoiTaoEditor)
            return;

        dangHenKhoiTaoEditor = true;
        UnityEditor.EditorApplication.delayCall += () =>
        {
            dangHenKhoiTaoEditor = false;
            if (this == null || Application.isPlaying || !hienTatCaObjectTruocKhiPlay)
                return;

            Random.InitState(27052006);
            TaoHoacCapNhatCauTrucScene(true);
            CapNhatUI();

            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        };
    }
#endif

    private void Start()
    {
        if (!Application.isPlaying)
            return;

        KhoiTaoTroChoi();
    }

    private void Update()
    {
        if (!Application.isPlaying)
            return;

        if (daKetThuc)
        {
            if (CoBamChoiLai())
                ChoiLai();
            return;
        }

        if (dieuKhienSung != null)
            dieuKhienSung.CapNhatDieuKhien();
    }

    private void KhoiTaoTroChoi()
    {
        // Dòng này làm màu bóng mỗi lần chơi có thể khác nhau.
        // Muốn lưới bóng luôn giống nhau để test: đổi thành Random.InitState(27052006);
        Random.InitState(System.DateTime.Now.Millisecond);

        // SỬA Ở ĐÂY nếu muốn game bắt đầu với điểm/cấp độ khác.
        // Ví dụ muốn bắt đầu từ 100 điểm thì đổi diem = 100;
        diem = 0;
        capDo = 1;
        soBongDaBan = 0;
        daKetThuc = false;

        TaoHoacCapNhatCauTrucScene(true);
        CapNhatUI();

        if (quanLyGiaoDien != null)
            quanLyGiaoDien.AnTatCaBangKetThuc();
    }

    private void TaoHoacCapNhatCauTrucScene(bool dungLaiObjectCoSan)
    {
        ChuanHoaThongSo();
        spriteBong = ThuVienHinhAnhBong.TaoTatCaSpriteBong();

        cauTruc = TrinhTaoCauTrucBanBong.TaoCauTruc(this, viTriSung, gioiHanTrai, gioiHanPhai);
        quanLyLuoiBong = cauTruc.QuanLyLuoiBong;
        quanLyBongBan = cauTruc.QuanLyBongBan;
        dieuKhienSung = cauTruc.DieuKhienSung;
        quanLyGiaoDien = cauTruc.QuanLyGiaoDien;

        quanLyLuoiBong.CaiDat(cauTruc.NhomLuoiBong, spriteBong, soHang, soCot, soHangBanDau, banKinhBong, viTriDinhLuoi);
        quanLyLuoiBong.TaoHoacNapLuoiBanDau(dungLaiObjectCoSan);

        quanLyBongBan.CaiDat(cauTruc.NhomSungBan, cauTruc.TrucXoaySung, spriteBong, banKinhBong, viTriSung);
        quanLyBongBan.TaoBongBanDau(dungLaiObjectCoSan);

        dieuKhienSung.CaiDat(cauTruc.CameraChinh, cauTruc.ThanSung, cauTruc.DuongNgam, quanLyBongBan, quanLyLuoiBong, this, tocDoBan, gioiHanTrai, gioiHanPhai, viTriDinhLuoi, banKinhBong, viTriSung);
        quanLyGiaoDien.CaiDat(this, cauTruc.TxtDiem, cauTruc.TxtCapDo, cauTruc.TxtSoBong, cauTruc.TxtGioiHan, cauTruc.BangThua, cauTruc.BangThang, cauTruc.TxtThua, cauTruc.TxtThang, cauTruc.NutChoiLaiKhiThua, cauTruc.NutChoiLaiKhiThang);
    }

    private void ChuanHoaThongSo()
    {
        soHang = Mathf.Max(1, soHang);
        soCot = Mathf.Max(1, soCot);
        soHangBanDau = Mathf.Clamp(soHangBanDau, 1, soHang);
        banKinhBong = Mathf.Max(0.08f, banKinhBong);
        tocDoBan = Mathf.Max(1f, tocDoBan);
    }

    public void XuLyBongChamLuoi(Vector3 viTriCham)
    {
        if (daKetThuc || quanLyBongBan == null || quanLyLuoiBong == null)
            return;

        Bong bong = quanLyBongBan.BongDangBan;
        bool thanhCong = quanLyLuoiBong.GanBongVaoLuoi(bong, viTriCham, out int diemCong, out bool daThang, out bool daThua, out string lyDoLoi);

        quanLyBongBan.MatQuyenQuanLyBongDangBan();
        soBongDaBan++;

        // SỬA Ở ĐÂY nếu muốn thay đổi cách cộng tổng điểm.
        // Hiện tại: tổng điểm = điểm cũ + điểm vừa nhận.
        // Ví dụ muốn nhân đôi mọi điểm nhận được: đổi thành diem += diemCong * 2;
        // Điểm mỗi bóng nổ/rơi được tính trong QuanLyLuoiBong.cs.
        diem += diemCong;
        CapNhatUI();

        if (!thanhCong && daThua)
        {
            KetThucTroChoi(false);
            return;
        }

        if (!thanhCong)
        {
            Debug.LogWarning(lyDoLoi);
            KetThucTroChoi(false);
            return;
        }

        if (daThua)
        {
            KetThucTroChoi(false);
            return;
        }

        if (daThang)
        {
            KetThucTroChoi(true);
            return;
        }

        quanLyBongBan.TaoBongMoiSauLanBan();
        dieuKhienSung.DatTrangThaiSanSang();
    }

    private void KetThucTroChoi(bool thang)
    {
        daKetThuc = true;

        if (dieuKhienSung != null)
            dieuKhienSung.DungBan();

        if (quanLyGiaoDien == null)
            return;

        if (thang)
            quanLyGiaoDien.HienBangThang(diem);
        else
            quanLyGiaoDien.HienBangThua(diem);
    }

    private void CapNhatUI()
    {
        if (quanLyGiaoDien == null)
            return;

        quanLyGiaoDien.CapNhatDiem(diem);
        quanLyGiaoDien.CapNhatCapDo(capDo);
        quanLyGiaoDien.CapNhatSoBong(soBongDaBan);
        // SỬA Ở ĐÂY nếu muốn đổi câu hướng dẫn trên màn hình.
        quanLyGiaoDien.CapNhatGioiHan("Kéo để ngắm - thả/click/Space để bắn");
    }

    private bool CoBamChoiLai()
    {
        // SỬA Ở ĐÂY nếu muốn đổi phím chơi lại.
        // Hiện tại dùng phím R. Muốn dùng Space thì đổi rKey thành spaceKey.
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            return true;

        // Dòng này cho phép bấm chuột để chơi lại sau khi thua/thắng.
        // Nếu không muốn click chuột chơi lại thì xóa hoặc comment khối if này.
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;

        // Dòng này cho phép chạm màn hình để chơi lại trên điện thoại.
        // Nếu không muốn cảm ứng chơi lại thì xóa hoặc comment khối if này.
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            return true;

        return false;
    }

    public void ChoiLai()
    {
        if (!Application.isPlaying)
            return;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
