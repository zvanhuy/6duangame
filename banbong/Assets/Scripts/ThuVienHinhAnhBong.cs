using UnityEngine;


// ============================================================
// GHI CHÚ CHỈNH SỬA NHANH
// File này quyết định màu và hình ảnh của bóng.
// - Muốn thêm/bớt màu bóng: sửa BangMauBong.
// - Muốn dùng ảnh thật: đặt ảnh vào Assets/Resources/AnhBong.
// - Muốn đổi kích thước texture bóng tự vẽ: tìm "int kichThuoc = 128".
// ============================================================
/// <summary>
/// Chịu trách nhiệm tạo hình ảnh quả bóng.
/// Nếu trong Resources/AnhBong có ảnh bóng thì dùng ảnh đó.
/// Nếu thiếu ảnh thì tự tạo sprite tròn bằng code.
/// </summary>
public static class ThuVienHinhAnhBong
{
    public static readonly Color[] BangMauBong =
    {
        // SỬA Ở ĐÂY nếu muốn đổi màu bóng.
        // Thứ tự màu tương ứng MaMau trong Bong.cs:
        // 0 = đỏ, 1 = xanh dương, 2 = xanh lá, 3 = vàng, 4 = tím.
        // Muốn thêm màu mới thì thêm dòng new Color(...) vào đây.
        // Lưu ý: thêm quá nhiều màu sẽ làm game khó hơn vì khó tạo cụm cùng màu.
        new Color(1.00f, 0.20f, 0.18f),
        new Color(0.18f, 0.54f, 1.00f),
        new Color(0.15f, 0.82f, 0.34f),
        new Color(1.00f, 0.88f, 0.12f),
        new Color(0.75f, 0.27f, 1.00f)
    };

    public static Sprite[] TaoTatCaSpriteBong()
    {
        // Unity sẽ tìm ảnh trong thư mục Assets/Resources/AnhBong.
        // Nếu có ảnh thì dùng ảnh. Nếu thiếu ảnh thì tự vẽ bóng tròn bằng code.
        Sprite[] anhCoSan = Resources.LoadAll<Sprite>("AnhBong");
        Sprite[] sprites = new Sprite[BangMauBong.Length];

        for (int i = 0; i < sprites.Length; i++)
        {
            if (anhCoSan != null && i < anhCoSan.Length && anhCoSan[i] != null)
                sprites[i] = anhCoSan[i];
            else
                sprites[i] = TaoSpriteTron(BangMauBong[i]);
        }

        return sprites;
    }

    public static void CapNhatHinhAnhBong(Bong bong, Sprite[] spriteBong, bool namTrongLuoi, float kichCo)
    {
        if (bong == null)
            return;

        bong.MaMau = Mathf.Clamp(bong.MaMau, 0, BangMauBong.Length - 1);
        bong.transform.localScale = Vector3.one * kichCo;

        SpriteRenderer ve = TienIchBanBong.LayHoacThem<SpriteRenderer>(bong.gameObject);
        if (spriteBong != null && bong.MaMau >= 0 && bong.MaMau < spriteBong.Length && spriteBong[bong.MaMau] != null)
            ve.sprite = spriteBong[bong.MaMau];
        else
            ve.sprite = TaoSpriteTron(BangMauBong[bong.MaMau]);

        ve.color = Color.white;

        // SỬA Ở ĐÂY nếu bóng bị nằm sau/trước sai lớp.
        // Bóng trong lưới sortingOrder = 1, bóng đang bắn/xem trước = 10 để hiện phía trên.
        ve.sortingOrder = namTrongLuoi ? 1 : 10;

        CircleCollider2D vaCham = TienIchBanBong.LayHoacThem<CircleCollider2D>(bong.gameObject);
        vaCham.radius = 0.5f;
    }

    public static Sprite TaoSpriteTron(Color mau)
    {
        // SỬA Ở ĐÂY nếu muốn ảnh bóng tự vẽ nét hơn hoặc nhẹ hơn.
        // 128 là kích thước texture. Tăng lên 256 đẹp hơn nhưng tốn bộ nhớ hơn.
        int kichThuoc = 128;
        Texture2D tex = new Texture2D(kichThuoc, kichThuoc, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;

        Vector2 tam = new Vector2(kichThuoc / 2f, kichThuoc / 2f);
        float banKinh = kichThuoc * 0.46f;

        for (int y = 0; y < kichThuoc; y++)
        {
            for (int x = 0; x < kichThuoc; x++)
            {
                float khoang = Vector2.Distance(new Vector2(x, y), tam);
                if (khoang > banKinh)
                {
                    tex.SetPixel(x, y, Color.clear);
                    continue;
                }

                float tiLe = khoang / banKinh;
                Color mauDiem = Color.Lerp(Color.white, mau, Mathf.Clamp01(tiLe * 0.85f));
                if (tiLe > 0.86f)
                    mauDiem = Color.Lerp(mauDiem, Color.black, 0.18f);

                tex.SetPixel(x, y, mauDiem);
            }
        }

        Vector2 diemSang = new Vector2(kichThuoc * 0.33f, kichThuoc * 0.68f);
        for (int y = 0; y < kichThuoc; y++)
        {
            for (int x = 0; x < kichThuoc; x++)
            {
                float khoang = Vector2.Distance(new Vector2(x, y), diemSang);
                if (khoang < kichThuoc * 0.12f)
                {
                    Color cu = tex.GetPixel(x, y);
                    if (cu.a > 0f)
                        tex.SetPixel(x, y, Color.Lerp(cu, Color.white, 0.55f));
                }
            }
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, kichThuoc, kichThuoc), new Vector2(0.5f, 0.5f), kichThuoc);
    }
}
