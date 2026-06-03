using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering.Universal;


// ============================================================
// GHI CHÚ CHỈNH SỬA NHANH
// File này tạo toàn bộ Hierarchy tiếng Việt và giao diện khi chưa bấm Play.
// - Muốn đổi tên object trong Hierarchy: sửa các chuỗi như "Camera Chính", "Lưới bóng".
// - Muốn đổi màu nền/tường/súng: tìm new Color(...) trong các hàm TaoNenChinh(), TaoTuong(), TaoCauTrucSung().
// - Muốn đổi kích thước camera: tìm camera.orthographicSize.
// - Muốn đổi vị trí UI: tìm TaoCacKhungThongTin(), TaoBangKetThuc(), DatRect().
// ============================================================
/// <summary>
/// Tạo cấu trúc Hierarchy cho game bắn bóng.
/// Script này giúp mọi object hiện sẵn trong Hierarchy ngay cả khi chưa bấm Play.
/// Logic chơi game không nằm ở đây, mà được tách sang các script khác.
/// </summary>
public static class TrinhTaoCauTrucBanBong
{
    public static CauTrucCanhBanBong TaoCauTruc(QuanLyTroChoiBanBong quanLyTroChoi, Vector2 viTriSung, float gioiHanTrai, float gioiHanPhai)
    {
        CauTrucCanhBanBong cauTruc = new CauTrucCanhBanBong();

        // Xóa các nhóm cũ của bản trước để Hierarchy không bị lẫn cấu trúc cũ và mới.
        TienIchBanBong.XoaObjectGocTheoTen("01_Nền trang trí", "02_Lưới bóng", "03_Súng và bóng bắn", "04_Giao diện UI");

        // SỬA Ở ĐÂY nếu muốn đổi tên object Camera trong Hierarchy.
        GameObject cameraObj = TienIchBanBong.LayHoacTaoDoiTuongGoc("Camera Chính");
        cauTruc.CameraChinh = TaoCamera(cameraObj);

        TaoDenToanCuc();
        TaoNenChinh();
        TaoTuong(gioiHanTrai, gioiHanPhai);
        TaoEventSystem();
        TaoHieuUngNoBong();
        TaoMauBong();

        GameObject quanLyObj = quanLyTroChoi.gameObject;
        // SỬA Ở ĐÂY nếu muốn đổi tên object quản lý chính trong Hierarchy.
        quanLyObj.name = "Quản lý trò chơi";
        quanLyObj.transform.SetParent(null, true);
        cauTruc.QuanLyLuoiBong = TienIchBanBong.LayHoacThem<QuanLyLuoiBong>(quanLyObj);

        // SỬA Ở ĐÂY nếu muốn đổi tên nhóm chứa bóng trong lưới.
        GameObject luoiObj = TienIchBanBong.LayHoacTaoDoiTuongGoc("Lưới bóng");
        cauTruc.NhomLuoiBong = luoiObj.transform;

        // SỬA Ở ĐÂY nếu muốn đổi tên nhóm súng bắn.
        GameObject sungObj = TienIchBanBong.LayHoacTaoDoiTuongGoc("Súng bắn");
        cauTruc.NhomSungBan = sungObj.transform;
        cauTruc.DieuKhienSung = TienIchBanBong.LayHoacThem<DieuKhienSung>(sungObj);
        TaoCauTrucSung(cauTruc, sungObj.transform, viTriSung);

        GameObject duongNgamObj = TienIchBanBong.LayHoacTaoDoiTuongGoc("Đường ngắm");
        cauTruc.DuongNgam = TaoLineRenderer(duongNgamObj, 0.04f, Color.white, new Color(1f, 1f, 1f, 0.15f), 4);

        GameObject qlBongBanObj = TienIchBanBong.LayHoacTaoDoiTuongGoc("Quản lý bóng bắn");
        cauTruc.QuanLyBongBan = TienIchBanBong.LayHoacThem<QuanLyBongBan>(qlBongBanObj);

        TaoCanvas(cauTruc, quanLyTroChoi);
        return cauTruc;
    }

    private static Camera TaoCamera(GameObject cameraObj)
    {
        Camera camera = TienIchBanBong.LayHoacThem<Camera>(cameraObj);
        TienIchBanBong.LayHoacThem<AudioListener>(cameraObj);
        cameraObj.tag = "MainCamera";
        camera.orthographic = true;

        // SỬA Ở ĐÂY nếu muốn camera nhìn rộng/hẹp hơn.
        // Tăng 5.15f => nhìn được nhiều khu vực hơn, object nhỏ lại.
        // Giảm 5.15f => phóng to game hơn.
        camera.orthographicSize = 5.15f;

        // SỬA Ở ĐÂY nếu muốn đổi màu nền camera.
        camera.backgroundColor = new Color(0.55f, 0.78f, 0.96f);
        cameraObj.transform.position = new Vector3(0f, 0f, -10f);
        return camera;
    }

    private static void TaoDenToanCuc()
    {
        GameObject denObj = TienIchBanBong.LayHoacTaoDoiTuongGoc("Đèn toàn cục 2D");
        Light2D den = TienIchBanBong.LayHoacThem<Light2D>(denObj);
        den.lightType = Light2D.LightType.Global;
        den.intensity = 1f;
    }

    private static void TaoNenChinh()
    {
        GameObject nenObj = TienIchBanBong.LayHoacTaoDoiTuongGoc("Nền chính");
        nenObj.transform.position = new Vector3(0f, 0f, 1f);
        nenObj.transform.localScale = new Vector3(6.2f, 10.5f, 1f);

        SpriteRenderer ve = TienIchBanBong.LayHoacThem<SpriteRenderer>(nenObj);
        if (ve.sprite == null)
            ve.sprite = TienIchBanBong.TaoSpriteHinhVuong();
        // SỬA Ở ĐÂY nếu muốn đổi màu nền chính.
        ve.color = new Color(0.55f, 0.80f, 0.98f);

        // sortingOrder càng nhỏ thì object càng nằm phía sau.
        ve.sortingOrder = -20;
    }

    private static void TaoTuong(float gioiHanTrai, float gioiHanPhai)
    {
        GameObject tuongObj = TienIchBanBong.LayHoacTaoDoiTuongGoc("Tường");
        Transform tuong = tuongObj.transform;

        // SỬA Ở ĐÂY nếu muốn đổi tên/màu/kích thước tường.
        // new Color(...) là màu tường, new Vector2(...) là kích thước tường.
        GameObject trai = TienIchBanBong.TaoHinhChuNhat("Tường trái", tuong, new Vector3(gioiHanTrai - 0.1f, 0f, -0.1f), new Vector2(0.12f, 10.2f), new Color(0.95f, 0.86f, 0.35f), -5);
        GameObject phai = TienIchBanBong.TaoHinhChuNhat("Tường phải", tuong, new Vector3(gioiHanPhai + 0.1f, 0f, -0.1f), new Vector2(0.12f, 10.2f), new Color(0.95f, 0.86f, 0.35f), -5);
        GameObject tren = TienIchBanBong.TaoHinhChuNhat("Tường trên", tuong, new Vector3(0f, 4.65f, -0.1f), new Vector2(6.2f, 0.16f), new Color(0.95f, 0.86f, 0.35f), -5);

        TienIchBanBong.LayHoacThem<BoxCollider2D>(trai);
        TienIchBanBong.LayHoacThem<BoxCollider2D>(phai);
        TienIchBanBong.LayHoacThem<BoxCollider2D>(tren);
    }

    private static void TaoCauTrucSung(CauTrucCanhBanBong cauTruc, Transform sung, Vector2 viTriSung)
    {
        sung.position = Vector3.zero;

        GameObject trucObj = TienIchBanBong.LayHoacTaoCon(sung, "Trục xoay súng");
        trucObj.transform.position = new Vector3(viTriSung.x, viTriSung.y, 0f);
        cauTruc.TrucXoaySung = trucObj.transform;

        GameObject hinhSungObj = TienIchBanBong.LayHoacTaoCon(trucObj.transform, "Hình súng");
        // SỬA Ở ĐÂY nếu muốn đổi độ dày/màu thân súng.
        // 0.18f là độ dày, 2 giá trị new Color là màu đầu/cuối.
        cauTruc.ThanSung = TaoLineRenderer(hinhSungObj, 0.18f, new Color(1f, 0.55f, 0.08f), new Color(1f, 0.85f, 0.15f), 5);

        GameObject dauNongObj = TienIchBanBong.LayHoacTaoCon(trucObj.transform, "Đầu nòng");
        dauNongObj.transform.position = new Vector3(viTriSung.x, viTriSung.y + 0.75f, 0f);

        GameObject xemTruocObj = TienIchBanBong.LayHoacTaoCon(trucObj.transform, "Bóng xem trước");
        xemTruocObj.transform.position = new Vector3(2.05f, viTriSung.y + 0.15f, 0f);

        GameObject bongDangBanObj = TienIchBanBong.LayHoacTaoCon(sung, "Bóng đang bắn");
        bongDangBanObj.transform.position = new Vector3(viTriSung.x, viTriSung.y, 0f);

        // SỬA Ở ĐÂY nếu muốn đổi kích thước/màu đế súng.
        GameObject deSung = TienIchBanBong.TaoHinhChuNhat("Đế súng", sung, new Vector3(viTriSung.x, viTriSung.y - 0.22f, 0f), new Vector2(0.85f, 0.28f), new Color(1f, 0.58f, 0.05f), 6);
        deSung.transform.localScale = new Vector3(0.85f, 0.28f, 1f);
    }

    private static LineRenderer TaoLineRenderer(GameObject obj, float doDay, Color mauDau, Color mauCuoi, int sortingOrder)
    {
        LineRenderer line = TienIchBanBong.LayHoacThem<LineRenderer>(obj);
        Material vatLieu = TienIchBanBong.TaoVatLieuLine();
        if (vatLieu != null)
            line.sharedMaterial = vatLieu;

        line.positionCount = 2;
        line.startWidth = doDay;
        line.endWidth = doDay * 0.75f;
        line.startColor = mauDau;
        line.endColor = mauCuoi;
        line.sortingOrder = sortingOrder;
        line.useWorldSpace = true;
        line.SetPosition(0, Vector3.zero);
        line.SetPosition(1, Vector3.up);
        return line;
    }

    private static void TaoEventSystem()
    {
        GameObject eventObj = TienIchBanBong.LayHoacTaoDoiTuongGoc("EventSystem");
        TienIchBanBong.LayHoacThem<EventSystem>(eventObj);

        StandaloneInputModule moduleCu = eventObj.GetComponent<StandaloneInputModule>();
        if (moduleCu != null)
        {
            if (Application.isPlaying)
                Object.Destroy(moduleCu);
            else
                Object.DestroyImmediate(moduleCu);
        }

        TienIchBanBong.LayHoacThem<InputSystemUIInputModule>(eventObj);
    }

    private static void TaoHieuUngNoBong()
    {
        GameObject obj = TienIchBanBong.LayHoacTaoDoiTuongGoc("Hiệu ứng nổ bóng");
        ParticleSystem ps = TienIchBanBong.LayHoacThem<ParticleSystem>(obj);
        var main = ps.main;
        // SỬA Ở ĐÂY nếu muốn đổi hiệu ứng nổ bóng.
        // startLifetime: thời gian hạt tồn tại, startSpeed: tốc độ bay, startSize: kích thước, maxParticles: số hạt tối đa.
        main.startLifetime = 0.25f;
        main.startSpeed = 2.5f;
        main.startSize = 0.12f;
        main.maxParticles = 40;
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private static void TaoMauBong()
    {
        GameObject obj = TienIchBanBong.LayHoacTaoDoiTuongGoc("Mẫu bóng");
        obj.transform.position = new Vector3(100f, 100f, 0f);
        Bong bong = TienIchBanBong.LayHoacThem<Bong>(obj);
        bong.DatDuLieu(0, QuanLyLuoiBong.KhongCoBong, QuanLyLuoiBong.KhongCoBong);
        ThuVienHinhAnhBong.CapNhatHinhAnhBong(bong, ThuVienHinhAnhBong.TaoTatCaSpriteBong(), false, 0.64f);
        obj.SetActive(true);
    }

    private static void TaoCanvas(CauTrucCanhBanBong cauTruc, QuanLyTroChoiBanBong quanLyTroChoi)
    {
        GameObject canvasObj = TienIchBanBong.LayHoacTaoDoiTuongGoc("Canvas");
        Canvas canvas = TienIchBanBong.LayHoacThem<Canvas>(canvasObj);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = TienIchBanBong.LayHoacThem<CanvasScaler>(canvasObj);
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        // SỬA Ở ĐÂY nếu muốn đổi tỉ lệ thiết kế UI.
        // 1080x1920 phù hợp màn hình dọc điện thoại.
        scaler.referenceResolution = new Vector2(1080, 1920);
        TienIchBanBong.LayHoacThem<GraphicRaycaster>(canvasObj);

        TaoBangThuaVaThang(cauTruc, canvasObj.transform);
        TaoCacKhungThongTin(cauTruc, canvasObj.transform);

        GameObject uiControllerObj = TienIchBanBong.LayHoacTaoCon(canvasObj.transform, "Bộ điều khiển UI");
        cauTruc.QuanLyGiaoDien = TienIchBanBong.LayHoacThem<QuanLyGiaoDien>(uiControllerObj);
    }

    private static void TaoBangThuaVaThang(CauTrucCanhBanBong cauTruc, Transform canvas)
    {
        cauTruc.BangThua = TaoBangKetThuc(canvas, "Bảng thua", "GAME OVER", out cauTruc.TxtThua, out cauTruc.NutChoiLaiKhiThua);
        cauTruc.BangThang = TaoBangKetThuc(canvas, "Bảng thắng", "VICTORY", out cauTruc.TxtThang, out cauTruc.NutChoiLaiKhiThang);

        cauTruc.BangThua.SetActive(false);
        cauTruc.BangThang.SetActive(false);
    }

    private static GameObject TaoBangKetThuc(Transform canvas, string tenBang, string noiDung, out Text textTieuDe, out Button nutChoiLai)
    {
        GameObject bang = TienIchBanBong.LayHoacTaoCon(canvas, tenBang);
        Image nen = TienIchBanBong.LayHoacThem<Image>(bang);
        nen.color = new Color(0f, 0f, 0f, 0.58f);
        DatFullManHinh(bang.GetComponent<RectTransform>());

        GameObject chuObj = TienIchBanBong.LayHoacTaoCon(bang.transform, tenBang == "Bảng thua" ? "Chữ THUA" : "Chữ THẮNG");
        // SỬA Ở ĐÂY nếu muốn đổi cỡ chữ bảng thắng/thua.
        // 58 là cỡ chữ tiêu đề.
        textTieuDe = TaoText(chuObj, noiDung, 58, TextAnchor.MiddleCenter, Color.white);
        DatRect(chuObj.GetComponent<RectTransform>(), new Vector2(0.5f, 0.56f), new Vector2(0.5f, 0.56f), new Vector2(0.5f, 0.5f), new Vector2(760f, 300f), Vector2.zero);

        GameObject nutObj = TienIchBanBong.LayHoacTaoCon(bang.transform, "Nút chơi lại");
        Image anhNut = TienIchBanBong.LayHoacThem<Image>(nutObj);
        anhNut.color = new Color(1f, 0.78f, 0.2f, 0.96f);
        nutChoiLai = TienIchBanBong.LayHoacThem<Button>(nutObj);
        DatRect(nutObj.GetComponent<RectTransform>(), new Vector2(0.5f, 0.36f), new Vector2(0.5f, 0.36f), new Vector2(0.5f, 0.5f), new Vector2(420f, 95f), Vector2.zero);

        GameObject chuNutObj = TienIchBanBong.LayHoacTaoCon(nutObj.transform, "Chữ nút");
        Text chuNut = TaoText(chuNutObj, "CHƠI LẠI", 36, TextAnchor.MiddleCenter, new Color(0.22f, 0.12f, 0.02f));
        DatFullManHinh(chuNut.rectTransform);
        return bang;
    }

    private static void TaoCacKhungThongTin(CauTrucCanhBanBong cauTruc, Transform canvas)
    {
        // SỬA Ở ĐÂY nếu muốn đổi các khung UI ở màn hình chơi.
        // Mỗi khung dùng TaoKhungThongTin(...): tên object, tiêu đề, vị trí neo, kích thước.
        GameObject khungDiem = TaoKhung(canvas, "Khung điểm", new Vector2(0.14f, 0.95f), new Vector2(240f, 90f));
        TaoNhan(khungDiem.transform, "Chữ điểm", "ĐIỂM", new Vector2(-50f, 16f));
        cauTruc.TxtDiem = TaoGiaTri(khungDiem.transform, "Giá trị điểm", "0", new Vector2(55f, -12f));

        GameObject khungCapDo = TaoKhung(canvas, "Khung cấp độ", new Vector2(0.50f, 0.95f), new Vector2(240f, 90f));
        TaoNhan(khungCapDo.transform, "Chữ cấp độ", "LEVEL", new Vector2(-50f, 16f));
        cauTruc.TxtCapDo = TaoGiaTri(khungCapDo.transform, "Giá trị cấp độ", "1", new Vector2(55f, -12f));

        GameObject khungBong = TaoKhung(canvas, "Khung bóng", new Vector2(0.86f, 0.95f), new Vector2(240f, 90f));
        TaoNhan(khungBong.transform, "Chữ bóng", "BÓNG", new Vector2(-50f, 16f));
        cauTruc.TxtSoBong = TaoGiaTri(khungBong.transform, "Giá trị bóng", "0", new Vector2(55f, -12f));

        GameObject khungGioiHan = TaoKhung(canvas, "Khung giới hạn", new Vector2(0.5f, 0.08f), new Vector2(420f, 75f));
        GameObject icon = TienIchBanBong.LayHoacTaoCon(khungGioiHan.transform, "Biểu tượng bóng");
        Image anhIcon = TienIchBanBong.LayHoacThem<Image>(icon);
        anhIcon.color = new Color(1f, 0.85f, 0.18f);
        DatRect(icon.GetComponent<RectTransform>(), new Vector2(0.12f, 0.5f), new Vector2(0.12f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(42f, 42f), Vector2.zero);
        cauTruc.TxtGioiHan = TaoGiaTri(khungGioiHan.transform, "Chữ giới hạn bóng", "Kéo để ngắm - thả để bắn", new Vector2(55f, 0f));
    }

    private static GameObject TaoKhung(Transform canvas, string ten, Vector2 anchor, Vector2 kichThuoc)
    {
        GameObject obj = TienIchBanBong.LayHoacTaoCon(canvas, ten);
        Image anh = TienIchBanBong.LayHoacThem<Image>(obj);
        anh.color = new Color(0f, 0f, 0f, 0.28f);
        DatRect(obj.GetComponent<RectTransform>(), anchor, anchor, new Vector2(0.5f, 0.5f), kichThuoc, Vector2.zero);
        return obj;
    }

    private static void TaoNhan(Transform cha, string ten, string noiDung, Vector2 viTri)
    {
        GameObject obj = TienIchBanBong.LayHoacTaoCon(cha, ten);
        Text text = TaoText(obj, noiDung, 22, TextAnchor.MiddleCenter, Color.white);
        DatRect(text.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(130f, 40f), viTri);
    }

    private static Text TaoGiaTri(Transform cha, string ten, string noiDung, Vector2 viTri)
    {
        GameObject obj = TienIchBanBong.LayHoacTaoCon(cha, ten);
        Text text = TaoText(obj, noiDung, 34, TextAnchor.MiddleCenter, Color.white);
        DatRect(text.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(250f, 55f), viTri);
        return text;
    }

    private static Text TaoText(GameObject obj, string noiDung, int coChu, TextAnchor canLe, Color mau)
    {
        Text text = TienIchBanBong.LayHoacThem<Text>(obj);
        text.text = noiDung;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = coChu;
        text.alignment = canLe;
        text.color = mau;
        text.fontStyle = FontStyle.Bold;
        return text;
    }

    private static void DatFullManHinh(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static void DatRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 kichThuoc, Vector2 viTri)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.sizeDelta = kichThuoc;
        rect.anchoredPosition = viTri;
    }
}
