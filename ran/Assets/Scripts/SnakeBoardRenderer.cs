using System.Collections.Generic;
using UnityEngine;

public class SnakeBoardRenderer
{
    private readonly Transform nhomBanChoiGoc;
    private readonly Transform nhomRanGoc;
    private readonly Transform nhomMoiGoc;

    private readonly Transform nhomBanChoiSinhKhiChoi;
    private readonly Transform nhomRanSinhKhiChoi;
    private readonly Transform nhomMoiSinhKhiChoi;

    private readonly SnakeAssets assets;
    private readonly int soCot;
    private readonly int soHang;
    private readonly float kichThuocO;

    private readonly Color mauNenLon = new Color(0.16f, 0.24f, 0.30f);
    private readonly Color mauVungChoi = new Color(0.08f, 0.13f, 0.17f);
    private readonly Color mauTuong = new Color(0.26f, 0.39f, 0.50f);
    private readonly Color mauDauRan = new Color(0.20f, 0.90f, 0.30f);
    private readonly Color mauThanRan = new Color(0.12f, 0.65f, 0.22f);
    private readonly Color mauMoi = new Color(1.00f, 0.20f, 0.18f);

    public SnakeBoardRenderer(Transform nhomBanChoi, Transform nhomRan, Transform nhomMoi, SnakeAssets assets, int soCot, int soHang, float kichThuocO)
    {
        this.nhomBanChoiGoc = nhomBanChoi;
        this.nhomRanGoc = nhomRan;
        this.nhomMoiGoc = nhomMoi;
        this.assets = assets;
        this.soCot = soCot;
        this.soHang = soHang;
        this.kichThuocO = kichThuocO;

        // Các object thật khi chơi sẽ nằm trong những nhóm này.
        // Các nhóm MAU_DE_SUA_* trong Hierarchy sẽ không bị xóa khi bấm Play,
        // nên thầy có thể mở ra để xem/sửa mà không bị mất.
        nhomBanChoiSinhKhiChoi = LayHoacTaoNhomCon(nhomBanChoiGoc, "DOI_TUONG_SINH_KHI_CHOI");
        nhomRanSinhKhiChoi = LayHoacTaoNhomCon(nhomRanGoc, "CAC_DOT_RAN_SINH_KHI_CHOI");
        nhomMoiSinhKhiChoi = LayHoacTaoNhomCon(nhomMoiGoc, "CAC_MOI_SINH_KHI_CHOI");
    }

    public void TaoNenVaTuong()
    {
        TaoOTrangTri("Nền bàn chơi", new Vector2(0f, 0f), soCot + 1.4f, soHang + 1.4f, LayMauTuMau("Mẫu nền lớn - sửa màu nền ở đây", mauNenLon), -20);
        TaoOTrangTri("Vùng chơi", new Vector2(0f, 0f), soCot, soHang, LayMauTuMau("Mẫu vùng chơi - sửa màu vùng chơi", mauVungChoi), -19);

        float rong = soCot * kichThuocO;
        float cao = soHang * kichThuocO;
        Color mauTuongTuHierarchy = LayMauTuMau("Mẫu tường trên", mauTuong);
        TaoOTrangTri("Tường trên", new Vector2(0f, cao / 2f + kichThuocO / 2f), soCot + 2, 1f, mauTuongTuHierarchy, -10);
        TaoOTrangTri("Tường dưới", new Vector2(0f, -cao / 2f - kichThuocO / 2f), soCot + 2, 1f, mauTuongTuHierarchy, -10);
        TaoOTrangTri("Tường trái", new Vector2(-rong / 2f - kichThuocO / 2f, 0f), 1f, soHang + 2, mauTuongTuHierarchy, -10);
        TaoOTrangTri("Tường phải", new Vector2(rong / 2f + kichThuocO / 2f, 0f), 1f, soHang + 2, mauTuongTuHierarchy, -10);
    }

    public GameObject TaoDotRan(string ten, Vector2Int o, bool laDau)
    {
        GameObject obj = new GameObject(ten);
        obj.transform.SetParent(nhomRanSinhKhiChoi, false);
        obj.transform.position = ChuyenOThanhViTri(o);
        obj.transform.localScale = Vector3.one * kichThuocO * 0.92f;

        SpriteRenderer ve = obj.AddComponent<SpriteRenderer>();
        ve.sprite = LaySpriteRan(laDau);
        ve.color = LayMauRan(laDau);
        ve.sortingOrder = laDau ? 3 : 2;
        return obj;
    }

    public void DoiDauCuThanhThan(List<GameObject> hinhDotRan)
    {
        if (hinhDotRan.Count <= 1 || hinhDotRan[1] == null)
            return;

        hinhDotRan[1].name = "Thân rắn";
        SpriteRenderer veThanCu = hinhDotRan[1].GetComponent<SpriteRenderer>();
        veThanCu.sprite = LaySpriteRan(false);
        veThanCu.color = LayMauRan(false);
        veThanCu.sortingOrder = 2;
    }

    public GameObject TaoMoi(Vector2Int oMoi, int soThuTu)
    {
        GameObject objMoi = new GameObject("Mồi " + soThuTu);
        objMoi.transform.SetParent(nhomMoiSinhKhiChoi, false);
        objMoi.transform.position = ChuyenOThanhViTri(oMoi);
        objMoi.transform.localScale = Vector3.one * kichThuocO * 0.80f;

        SpriteRenderer ve = objMoi.AddComponent<SpriteRenderer>();
        Sprite spriteMauMoi = LaySpriteTuMau("Mẫu mồi - sửa màu/sprite ở đây");
        ve.sprite = spriteMauMoi != null ? spriteMauMoi : (assets.SpriteMoi != null ? assets.SpriteMoi : assets.SpriteTron);
        ve.color = LayMauTuMau("Mẫu mồi - sửa màu/sprite ở đây", assets.SpriteMoi != null ? Color.white : mauMoi);
        ve.sortingOrder = 4;
        return objMoi;
    }

    public void XoaBanChoi()
    {
        XoaConCuaNhom(nhomBanChoiSinhKhiChoi);
    }

    public void XoaRanVaMoi()
    {
        XoaConCuaNhom(nhomRanSinhKhiChoi);
        XoaConCuaNhom(nhomMoiSinhKhiChoi);
    }

    private GameObject TaoOTrangTri(string ten, Vector2 viTri, float rongTheoO, float caoTheoO, Color mau, int thuTuSapXep)
    {
        GameObject obj = new GameObject(ten);
        obj.transform.SetParent(nhomBanChoiSinhKhiChoi, false);
        obj.transform.position = new Vector3(viTri.x, viTri.y, 0f);
        obj.transform.localScale = new Vector3(rongTheoO * kichThuocO, caoTheoO * kichThuocO, 1f);

        SpriteRenderer ve = obj.AddComponent<SpriteRenderer>();
        ve.sprite = assets.SpriteVuong;
        ve.color = mau;
        ve.sortingOrder = thuTuSapXep;
        return obj;
    }

    private Sprite LaySpriteRan(bool laDau)
    {
        string tenMau = laDau ? "Mẫu đầu rắn - sửa màu/sprite ở đây" : "Mẫu thân rắn - sửa màu/sprite ở đây";
        Sprite spriteTuHierarchy = LaySpriteTuMau(tenMau);
        if (spriteTuHierarchy != null)
            return spriteTuHierarchy;

        if (laDau)
            return assets.SpriteDauRan != null ? assets.SpriteDauRan : assets.SpriteVuong;

        return assets.SpriteThanRan != null ? assets.SpriteThanRan : assets.SpriteVuong;
    }

    private Color LayMauRan(bool laDau)
    {
        string tenMau = laDau ? "Mẫu đầu rắn - sửa màu/sprite ở đây" : "Mẫu thân rắn - sửa màu/sprite ở đây";
        Color mauMacDinh = laDau ? mauDauRan : mauThanRan;
        if (laDau && assets.SpriteDauRan != null)
            mauMacDinh = Color.white;
        if (!laDau && assets.SpriteThanRan != null)
            mauMacDinh = Color.white;

        return LayMauTuMau(tenMau, mauMacDinh);
    }

    private Sprite LaySpriteTuMau(string tenMau)
    {
        GameObject obj = GameObject.Find(tenMau);
        if (obj == null)
            return null;

        SpriteRenderer ve = obj.GetComponent<SpriteRenderer>();
        return ve != null ? ve.sprite : null;
    }

    private Color LayMauTuMau(string tenMau, Color mauMacDinh)
    {
        GameObject obj = GameObject.Find(tenMau);
        if (obj == null)
            return mauMacDinh;

        SpriteRenderer ve = obj.GetComponent<SpriteRenderer>();
        return ve != null ? ve.color : mauMacDinh;
    }

    private Vector3 ChuyenOThanhViTri(Vector2Int o)
    {
        float x = (o.x - (soCot - 1) / 2f) * kichThuocO;
        float y = (o.y - (soHang - 1) / 2f) * kichThuocO;
        return new Vector3(x, y, 0f);
    }

    private Transform LayHoacTaoNhomCon(Transform cha, string ten)
    {
        Transform con = cha.Find(ten);
        if (con != null)
            return con;

        GameObject obj = new GameObject(ten);
        obj.transform.SetParent(cha, false);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
        return obj.transform;
    }

    private void XoaConCuaNhom(Transform nhom)
    {
        if (nhom == null)
            return;

        for (int i = nhom.childCount - 1; i >= 0; i--)
            Object.Destroy(nhom.GetChild(i).gameObject);
    }
}
