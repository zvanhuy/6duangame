using UnityEngine;


// ============================================================
// GHI CHÚ CHỈNH SỬA NHANH
// File này là dữ liệu của TỪNG QUẢ BÓNG.
// Không nên cộng điểm, kiểm tra thắng/thua hoặc xử lý bắn ở đây.
// Muốn chỉnh điểm: mở QuanLyLuoiBong.cs, tìm "XoaCumBong(cum, 10)".
// Muốn chỉnh tốc độ bắn: mở QuanLyTroChoiBanBong.cs hoặc DieuKhienSung.cs, tìm "tocDoBan".
// ============================================================
/// <summary>
/// Script gắn trên từng quả bóng.
/// Chỉ lưu dữ liệu của bóng: nằm ở hàng/cột nào, màu gì, hướng bay hiện tại.
/// </summary>
public class Bong : MonoBehaviour
{
    [Header("Vị trí trong lưới")]
    // SỬA Ở ĐÂY nếu muốn đặt thủ công vị trí bóng trong Inspector:
    // Hang = hàng của bóng trong lưới, Cot = cột của bóng trong lưới.
    // Giá trị -1 nghĩa là bóng chưa nằm trong lưới, ví dụ "Bóng đang bắn".
    public int Hang = -1;
    public int Cot = -1;

    [Header("Màu bóng")]
    // SỬA Ở ĐÂY nếu muốn đổi màu riêng của 1 quả bóng trong Hierarchy:
    // 0 = đỏ, 1 = xanh dương, 2 = xanh lá, 3 = vàng, 4 = tím.
    // Bảng màu nằm trong ThuVienHinhAnhBong.cs, biến BangMauBong.
    public int MaMau;

    [Header("Hướng bay khi đang bắn")]
    // Không cần sửa tay dòng này. Hướng bay được DieuKhienSung.cs tự gán khi bắn.
    public Vector2 HuongBay;

    public void DatDuLieu(int maMau, int hang, int cot)
    {
        // Hàm này được gọi khi tạo bóng hoặc khi bóng bắn được gắn vào lưới.
        // Muốn đổi cách lưu màu/hàng/cột thì chỉnh tại 3 dòng dưới.
        MaMau = maMau;
        Hang = hang;
        Cot = cot;
    }

    public void DatHuongBay(Vector2 huong)
    {
        // normalized giúp hướng bay có độ dài = 1.
        // Không nên bỏ normalized, vì nếu bỏ thì tốc độ bóng có thể nhanh/chậm bất thường.
        HuongBay = huong.normalized;
    }
}
