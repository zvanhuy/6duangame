using UnityEngine;
using UnityEngine.InputSystem;


// ============================================================
// GHI CHÚ CHỈNH SỬA NHANH
// File này quyết định cách NGẮM, BẮN, BÓNG BAY và BẬT TƯỜNG.
// - Muốn bóng nhanh/chậm: sửa tocDoBan.
// - Muốn đổi độ dài đường ngắm: tìm "huongBan * 2.0f".
// - Muốn cho phép bắn ngang/thấp hơn: tìm "huong.y > 0.2f".
// - Muốn đổi phím bắn: tìm hàm CoBamBan().
// - Muốn đổi giới hạn bật tường: sửa gioiHanTrai/gioiHanPhai ở QuanLyTroChoiBanBong.cs.
// ============================================================
/// <summary>
/// Điều khiển súng bắn bóng.
/// Script này lo các việc:
/// 1. Đọc chuột, cảm ứng, phím Space bằng Input System mới.
/// 2. Cập nhật hướng ngắm.
/// 3. Bắn bóng.
/// 4. Cho bóng bay và bật tường.
/// Khi bóng chạm lưới, script này báo lại cho Quản lý trò chơi xử lý.
/// </summary>
public class DieuKhienSung : MonoBehaviour
{
    [Header("Tham chiếu")]
    [SerializeField] private Camera cameraChinh;
    [SerializeField] private LineRenderer thanSung;
    [SerializeField] private LineRenderer duongNgam;
    [SerializeField] private QuanLyBongBan quanLyBongBan;
    [SerializeField] private QuanLyLuoiBong quanLyLuoiBong;
    [SerializeField] private QuanLyTroChoiBanBong quanLyTroChoi;

    [Header("Cấu hình bắn")]
    // Các thông số này thường được truyền từ QuanLyTroChoiBanBong.cs qua hàm CaiDat().
    // Muốn chỉnh bằng Inspector thì sửa ở object "Quản lý trò chơi" cho chắc chắn.
    [Tooltip("Tốc độ bay của bóng sau khi bắn.")]
    [SerializeField] private float tocDoBan = 8.5f;
    [Tooltip("Tường trái, bóng chạm vào sẽ bật lại.")]
    [SerializeField] private float gioiHanTrai = -2.65f;
    [Tooltip("Tường phải, bóng chạm vào sẽ bật lại.")]
    [SerializeField] private float gioiHanPhai = 2.65f;
    [Tooltip("Độ cao đỉnh lưới, bóng bay tới đây sẽ được gắn vào lưới.")]
    [SerializeField] private float viTriDinhLuoi = 4.05f;
    [Tooltip("Bán kính bóng, dùng để tính va chạm với tường/lưới.")]
    [SerializeField] private float banKinhBong = 0.32f;
    [Tooltip("Vị trí súng bắn.")]
    [SerializeField] private Vector2 viTriSung = new Vector2(0f, -4.15f);

    private Vector2 huongBan = Vector2.up;
    private bool dangBay;

    public void CaiDat(Camera cameraGame, LineRenderer lineThanSung, LineRenderer lineDuongNgam, QuanLyBongBan qlBongBan, QuanLyLuoiBong qlLuoi, QuanLyTroChoiBanBong qlTroChoi, float tocDo, float trai, float phai, float dinhLuoi, float banKinh, Vector2 dauSung)
    {
        cameraChinh = cameraGame;
        thanSung = lineThanSung;
        duongNgam = lineDuongNgam;
        quanLyBongBan = qlBongBan;
        quanLyLuoiBong = qlLuoi;
        quanLyTroChoi = qlTroChoi;
        tocDoBan = tocDo;
        gioiHanTrai = trai;
        gioiHanPhai = phai;
        viTriDinhLuoi = dinhLuoi;
        banKinhBong = banKinh;
        viTriSung = dauSung;
        CapNhatDuongNgam();
    }

    public void CapNhatDieuKhien()
    {
        if (quanLyBongBan == null || quanLyBongBan.BongDangBan == null)
            return;

        if (dangBay)
        {
            CapNhatBongDangBay();
            return;
        }

        CapNhatHuongNgamTuDauVao();
        CapNhatDuongNgam();

        if (CoBamBan())
            BanBong();
    }

    public void DatTrangThaiSanSang()
    {
        dangBay = false;
        CapNhatDuongNgam();
    }

    public void DungBan()
    {
        dangBay = false;
    }

    private void CapNhatHuongNgamTuDauVao()
    {
        Vector3 diemManHinh;
        bool coDauVao = LayViTriDauVao(out diemManHinh);
        if (!coDauVao || cameraChinh == null)
            return;

        Vector3 diemTheGioi = cameraChinh.ScreenToWorldPoint(diemManHinh);
        diemTheGioi.z = 0f;
        Vector2 huong = ((Vector2)diemTheGioi - viTriSung).normalized;

        // SỬA Ở ĐÂY nếu muốn giới hạn hướng bắn.
        // 0.2f nghĩa là chỉ cho bắn lên trên, không cho bắn quá ngang hoặc bắn xuống.
        // Giảm xuống 0f => cho bắn ngang hơn. Tăng lên 0.5f => bắt buộc bắn dốc lên nhiều hơn.
        if (huong.y > 0.2f)
            huongBan = huong;
    }

    private void CapNhatDuongNgam()
    {
        Vector3 dau = new Vector3(viTriSung.x, viTriSung.y, 0f);
        // SỬA Ở ĐÂY nếu muốn đổi chiều dài súng và đường ngắm.
        // 0.75f là độ dài thân súng. 2.0f là độ dài đường ngắm.
        Vector3 dauNong = dau + (Vector3)(huongBan * 0.75f);
        Vector3 cuoiNgam = dau + (Vector3)(huongBan * 2.0f);

        if (thanSung != null)
        {
            thanSung.positionCount = 2;
            thanSung.SetPosition(0, dau);
            thanSung.SetPosition(1, dauNong);
        }

        if (duongNgam != null)
        {
            duongNgam.positionCount = 2;
            duongNgam.SetPosition(0, dauNong);
            duongNgam.SetPosition(1, cuoiNgam);
        }
    }

    private void BanBong()
    {
        Bong bong = quanLyBongBan.BongDangBan;
        if (bong == null || dangBay)
            return;

        dangBay = true;
        bong.DatHuongBay(huongBan);
        bong.gameObject.name = "Bóng đang bay";
    }

    private void CapNhatBongDangBay()
    {
        Bong bong = quanLyBongBan.BongDangBan;
        if (bong == null)
        {
            dangBay = false;
            return;
        }

        Vector3 viTri = bong.transform.position;
        Vector2 huong = bong.HuongBay;
        // SỬA Ở ĐÂY nếu muốn thay đổi công thức di chuyển của bóng.
        // tocDoBan càng lớn bóng càng nhanh. Time.deltaTime giúp tốc độ ổn định theo FPS.
        viTri += (Vector3)(huong * tocDoBan * Time.deltaTime);

        // Logic bật tường trái/phải.
        // Nếu bóng xuyên tường, kiểm tra gioiHanTrai/gioiHanPhai và banKinhBong.
        if (viTri.x <= gioiHanTrai + banKinhBong)
        {
            viTri.x = gioiHanTrai + banKinhBong;
            huong.x = Mathf.Abs(huong.x); // đổi hướng X sang dương để bật sang phải
        }
        else if (viTri.x >= gioiHanPhai - banKinhBong)
        {
            viTri.x = gioiHanPhai - banKinhBong;
            huong.x = -Mathf.Abs(huong.x); // đổi hướng X sang âm để bật sang trái
        }

        bong.transform.position = viTri;
        bong.DatHuongBay(huong);

        // SỬA Ở ĐÂY nếu muốn đổi thời điểm bóng gắn vào lưới.
        // Điều kiện 1: bóng bay tới đỉnh lưới.
        // Điều kiện 2: bóng chạm một bóng đang có trong lưới.
        if (viTri.y >= viTriDinhLuoi + banKinhBong * 0.25f || quanLyLuoiBong.ChamVaoBongTrongLuoi(viTri))
        {
            dangBay = false;
            quanLyTroChoi.XuLyBongChamLuoi(viTri);
        }
    }

    private bool LayViTriDauVao(out Vector3 viTriManHinh)
    {
        viTriManHinh = Vector3.zero;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 viTriCham = Touchscreen.current.primaryTouch.position.ReadValue();
            viTriManHinh = new Vector3(viTriCham.x, viTriCham.y, 0f);
            return true;
        }

        if (Mouse.current != null)
        {
            Vector2 viTriChuot = Mouse.current.position.ReadValue();
            viTriManHinh = new Vector3(viTriChuot.x, viTriChuot.y, 0f);
            return true;
        }

        return false;
    }

    private bool CoBamBan()
    {
        // SỬA Ở ĐÂY nếu muốn đổi cách bắn.
        // Dòng dưới: bắn bằng click chuột trái.
        // Nếu muốn chỉ bắn khi nhả chuột thì xóa Mouse.current.leftButton.wasPressedThisFrame.
        if (Mouse.current != null && (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.leftButton.wasReleasedThisFrame))
            return true;

        // Dòng dưới: bắn bằng cảm ứng điện thoại.
        if (Touchscreen.current != null && (Touchscreen.current.primaryTouch.press.wasPressedThisFrame || Touchscreen.current.primaryTouch.press.wasReleasedThisFrame))
            return true;

        // Dòng dưới: bắn bằng phím Space.
        // Muốn đổi sang phím Enter thì thay spaceKey bằng enterKey.
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            return true;

        return false;
    }
}
