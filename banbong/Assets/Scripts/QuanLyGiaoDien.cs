using UnityEngine;
using UnityEngine.UI;


// ============================================================
// GHI CHÚ CHỈNH SỬA NHANH
// File này quản lý UI: điểm, cấp độ, số bóng, bảng thắng/thua.
// - Muốn đổi chữ GAME OVER/VICTORY: tìm HienBangThua() và HienBangThang().
// - Muốn đổi nút chơi lại: sửa GanNutChoiLai().
// - Muốn đổi vị trí UI: mở TrinhTaoCauTrucBanBong.cs, tìm TaoCacKhungThongTin().
// ============================================================
/// <summary>
/// Quản lý giao diện người chơi.
/// Script này chỉ lo UI: điểm, cấp độ, số bóng, bảng thua, bảng thắng, nút chơi lại.
/// </summary>
public class QuanLyGiaoDien : MonoBehaviour
{
    [Header("Text hiển thị")]
    [SerializeField] private Text txtDiem;
    [SerializeField] private Text txtCapDo;
    [SerializeField] private Text txtSoBong;
    [SerializeField] private Text txtGioiHan;

    [Header("Bảng kết thúc")]
    [SerializeField] private GameObject bangThua;
    [SerializeField] private GameObject bangThang;
    [SerializeField] private Text txtThua;
    [SerializeField] private Text txtThang;
    [SerializeField] private Button nutChoiLaiKhiThua;
    [SerializeField] private Button nutChoiLaiKhiThang;

    private QuanLyTroChoiBanBong quanLyTroChoi;

    public void CaiDat(QuanLyTroChoiBanBong qlTroChoi, Text diem, Text capDo, Text soBong, Text gioiHan, GameObject panelThua, GameObject panelThang, Text chuThua, Text chuThang, Button btnThua, Button btnThang)
    {
        quanLyTroChoi = qlTroChoi;
        txtDiem = diem;
        txtCapDo = capDo;
        txtSoBong = soBong;
        txtGioiHan = gioiHan;
        bangThua = panelThua;
        bangThang = panelThang;
        txtThua = chuThua;
        txtThang = chuThang;
        nutChoiLaiKhiThua = btnThua;
        nutChoiLaiKhiThang = btnThang;

        GanNutChoiLai();
        AnTatCaBangKetThuc();
    }

    public void GanNutChoiLai()
    {
        // Dòng AddListener bên dưới gán sự kiện cho nút chơi lại.
        // Muốn nút làm việc khác thì thay quanLyTroChoi.ChoiLai() bằng hàm khác.
        if (nutChoiLaiKhiThua != null)
        {
            nutChoiLaiKhiThua.onClick.RemoveAllListeners();
            nutChoiLaiKhiThua.onClick.AddListener(() => quanLyTroChoi.ChoiLai());
        }

        if (nutChoiLaiKhiThang != null)
        {
            nutChoiLaiKhiThang.onClick.RemoveAllListeners();
            nutChoiLaiKhiThang.onClick.AddListener(() => quanLyTroChoi.ChoiLai());
        }
    }

    public void CapNhatDiem(int diem)
    {
        // SỬA Ở ĐÂY nếu muốn đổi cách hiện điểm.
        // Ví dụ: txtDiem.text = "Điểm: " + diem;
        if (txtDiem != null)
            txtDiem.text = diem.ToString();
    }

    public void CapNhatCapDo(int capDo)
    {
        if (txtCapDo != null)
            txtCapDo.text = capDo.ToString();
    }

    public void CapNhatSoBong(int soBong)
    {
        if (txtSoBong != null)
            txtSoBong.text = soBong.ToString();
    }

    public void CapNhatGioiHan(string noiDung)
    {
        if (txtGioiHan != null)
            txtGioiHan.text = noiDung;
    }

    public void AnTatCaBangKetThuc()
    {
        if (bangThua != null)
            bangThua.SetActive(false);

        if (bangThang != null)
            bangThang.SetActive(false);
    }

    public void HienBangThua(int diem)
    {
        // Khi thua, bật Bảng thua và tắt Bảng thắng.
        if (bangThua != null)
            bangThua.SetActive(true);

        if (bangThang != null)
            bangThang.SetActive(false);

        if (txtThua != null)
            // SỬA Ở ĐÂY nếu muốn đổi chữ khi thua.
            // \n là ký tự xuống dòng trong chuỗi text.
            txtThua.text = "GAME OVER\nĐIỂM: " + diem + "\nNhấn R hoặc bấm nút để chơi lại";
    }

    public void HienBangThang(int diem)
    {
        if (bangThang != null)
            bangThang.SetActive(true);

        if (bangThua != null)
            bangThua.SetActive(false);

        if (txtThang != null)
            // SỬA Ở ĐÂY nếu muốn đổi chữ khi thắng.
            // Ví dụ đổi VICTORY thành CHIẾN THẮNG.
            txtThang.text = "VICTORY\nĐIỂM: " + diem + "\nNhấn R hoặc bấm nút để chơi lại";
    }
}
