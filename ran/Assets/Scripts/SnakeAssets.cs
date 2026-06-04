using UnityEngine;

public class SnakeAssets
{
    public Sprite SpriteVuong { get; private set; }
    public Sprite SpriteTron { get; private set; }
    public Sprite SpriteDauRan { get; private set; }
    public Sprite SpriteThanRan { get; private set; }
    public Sprite SpriteMoi { get; private set; }
    public AudioClip AmAnMoi { get; private set; }
    public AudioClip AmChet { get; private set; }

    public static SnakeAssets TaiTaiNguyen()
    {
        SnakeAssets taiNguyen = new SnakeAssets();
        taiNguyen.SpriteVuong = Resources.Load<Sprite>("AnhRan/o_vuong");
        if (taiNguyen.SpriteVuong == null)
            taiNguyen.SpriteVuong = TaoSpriteVuong();
        taiNguyen.SpriteTron = TaoSpriteTron();
        taiNguyen.SpriteDauRan = Resources.Load<Sprite>("AnhRan/dau_ran");
        taiNguyen.SpriteThanRan = Resources.Load<Sprite>("AnhRan/than_ran");
        taiNguyen.SpriteMoi = Resources.Load<Sprite>("AnhRan/moi");
        taiNguyen.AmAnMoi = Resources.Load<AudioClip>("Audio/an_moi");
        taiNguyen.AmChet = Resources.Load<AudioClip>("Audio/chet");
        return taiNguyen;
    }

    private static Sprite TaoSpriteVuong()
    {
        Texture2D tex = new Texture2D(8, 8, TextureFormat.RGBA32, false);
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
                tex.SetPixel(x, y, Color.white);
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 8, 8), new Vector2(0.5f, 0.5f), 8);
    }

    private static Sprite TaoSpriteTron()
    {
        int kichThuoc = 64;
        Texture2D tex = new Texture2D(kichThuoc, kichThuoc, TextureFormat.RGBA32, false);
        Vector2 tam = new Vector2(kichThuoc / 2f, kichThuoc / 2f);
        float banKinh = kichThuoc * 0.45f;

        for (int y = 0; y < kichThuoc; y++)
        {
            for (int x = 0; x < kichThuoc; x++)
            {
                float khoang = Vector2.Distance(new Vector2(x, y), tam);
                tex.SetPixel(x, y, khoang <= banKinh ? Color.white : Color.clear);
            }
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, kichThuoc, kichThuoc), new Vector2(0.5f, 0.5f), kichThuoc);
    }
}
