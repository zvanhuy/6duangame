using UnityEngine;


// ============================================================
// GHI CHÚ CHỈNH SỬA NHANH
// File này là thư viện hàm hỗ trợ tạo object/component.
// Thường KHÔNG sửa gameplay ở đây.
// Chỉ sửa nếu muốn đổi cách tạo object, tạo hình chữ nhật, tạo material đường bắn.
// ============================================================
/// <summary>
/// Tập hợp hàm dùng chung để tạo, tìm, gán object trong Scene.
/// File này giúp các script khác ngắn gọn hơn, không bị lặp code.
/// </summary>
public static class TienIchBanBong
{
    public static T LayHoacThem<T>(GameObject obj) where T : Component
    {
        T thanhPhan = obj.GetComponent<T>();
        if (thanhPhan == null)
            thanhPhan = obj.AddComponent<T>();
        return thanhPhan;
    }

    public static GameObject LayHoacTaoDoiTuongGoc(string ten)
    {
        // Hàm này tìm object ở cấp gốc Hierarchy theo tên.
        // Nếu chưa có thì tạo mới. Dùng để object hiện sẵn trước khi Play.
        GameObject obj = GameObject.Find(ten);
        if (obj == null)
            obj = new GameObject(ten);

        if (obj.transform.parent != null)
            obj.transform.SetParent(null, true);

        return obj;
    }

    public static GameObject LayHoacTaoCon(Transform cha, string ten)
    {
        if (cha != null)
        {
            for (int i = 0; i < cha.childCount; i++)
            {
                Transform con = cha.GetChild(i);
                if (con.name == ten)
                    return con.gameObject;
            }
        }

        GameObject obj = new GameObject(ten);
        if (cha != null)
            obj.transform.SetParent(cha, false);

        return obj;
    }

    public static void GanVaoNhom(GameObject obj, Transform cha)
    {
        if (obj != null && cha != null && obj.transform.parent != cha)
            obj.transform.SetParent(cha, true);
    }

    public static void HuyObject(GameObject obj)
    {
        if (obj == null)
            return;

        if (Application.isPlaying)
            Object.Destroy(obj);
        else
            Object.DestroyImmediate(obj);
    }

    public static void XoaObjectGocTheoTen(params string[] danhSachTen)
    {
        foreach (string ten in danhSachTen)
        {
            GameObject obj = GameObject.Find(ten);
            if (obj != null && obj.transform.parent == null)
                HuyObject(obj);
        }
    }

    public static Sprite TaoSpriteHinhVuong()
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

    public static GameObject TaoHinhChuNhat(string ten, Transform cha, Vector3 viTri, Vector2 kichThuoc, Color mau, int sortingOrder)
    {
        // SỬA Ở ĐÂY nếu muốn đổi cách tạo nền/tường/đế súng.
        // mau là màu object, kichThuoc là kích thước, sortingOrder quyết định object nằm trước/sau.
        GameObject obj = LayHoacTaoCon(cha, ten);
        obj.transform.position = viTri;
        obj.transform.localScale = new Vector3(kichThuoc.x, kichThuoc.y, 1f);

        SpriteRenderer ve = LayHoacThem<SpriteRenderer>(obj);
        if (ve.sprite == null)
            ve.sprite = TaoSpriteHinhVuong();

        ve.color = mau;
        ve.sortingOrder = sortingOrder;
        return obj;
    }

    public static Material TaoVatLieuLine()
    {
        // SỬA Ở ĐÂY nếu muốn dùng shader khác cho thân súng/đường ngắm.
        Shader shader = Shader.Find("Sprites/Default");
        if (shader == null)
            return null;

        Material vatLieu = new Material(shader);
        vatLieu.name = "Vật liệu đường bắn";
        return vatLieu;
    }
}
