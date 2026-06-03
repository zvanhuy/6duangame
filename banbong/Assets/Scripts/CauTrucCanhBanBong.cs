using UnityEngine;
using UnityEngine.UI;


// ============================================================
// GHI CHÚ CHỈNH SỬA NHANH
// File này chỉ là "hộp chứa tham chiếu" tới các object trong Hierarchy.
// Thường KHÔNG sửa logic gameplay ở đây.
// Muốn thêm object UI/Hiearchy mới thì thêm biến public ở đây,
// sau đó gán biến đó trong TrinhTaoCauTrucBanBong.cs.
// ============================================================
/// <summary>
/// Gói dữ liệu chứa toàn bộ object quan trọng trong Scene.
/// Trình tạo cấu trúc sẽ trả về object này để GameManager gán script cho đúng chỗ.
/// </summary>
public class CauTrucCanhBanBong
{
    // Nhóm tham chiếu camera và các nhóm chính trong Hierarchy.
    // Muốn đổi tên object trong Hierarchy thì sửa ở TrinhTaoCauTrucBanBong.cs,
    // KHÔNG sửa tên biến ở đây nếu không cần thiết.
    public Camera CameraChinh;
    public Transform NhomLuoiBong;
    public Transform NhomSungBan;
    public Transform TrucXoaySung;
    public LineRenderer ThanSung;
    public LineRenderer DuongNgam;
    public QuanLyLuoiBong QuanLyLuoiBong;
    public QuanLyBongBan QuanLyBongBan;
    public DieuKhienSung DieuKhienSung;
    public QuanLyGiaoDien QuanLyGiaoDien;

    // Nhóm tham chiếu UI.
    // Muốn đổi nội dung chữ hiển thị thì sửa ở QuanLyGiaoDien.cs.
    // Muốn đổi vị trí/kích thước UI thì sửa ở TrinhTaoCauTrucBanBong.cs.
    public Text TxtDiem;
    public Text TxtCapDo;
    public Text TxtSoBong;
    public Text TxtGioiHan;
    public GameObject BangThua;
    public GameObject BangThang;
    public Text TxtThua;
    public Text TxtThang;
    public Button NutChoiLaiKhiThua;
    public Button NutChoiLaiKhiThang;
}
