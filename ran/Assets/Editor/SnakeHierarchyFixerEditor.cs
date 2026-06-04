#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class SnakeHierarchyFixerEditor
{
    private static bool daTuDongChay;

    static SnakeHierarchyFixerEditor()
    {
        EditorApplication.delayCall += TuDongSuaKhiMoUnity;
    }

    private static void TuDongSuaKhiMoUnity()
    {
        if (daTuDongChay || Application.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        daTuDongChay = true;
        TaoLaiHierarchyDayDu(false);
    }

    [MenuItem("Snake/Sửa lại Hierarchy đầy đủ để dễ chỉnh")]
    public static void TaoLaiHierarchyBangMenu()
    {
        TaoLaiHierarchyDayDu(true);
    }

    private static void TaoLaiHierarchyDayDu(bool hienThongBao)
    {
        Transform nhomCamera = LayHoacTaoRoot("00_CAMERA");
        Transform nhomBoard = LayHoacTaoRoot("01_BOARD_NEN_TUONG");
        Transform nhomRan = LayHoacTaoRoot("02_SNAKE_RAN");
        Transform nhomMoi = LayHoacTaoRoot("03_FOOD_MOI");
        Transform nhomUI = LayHoacTaoRoot("04_UI_GIAO_DIEN");
        Transform nhomAudio = LayHoacTaoRoot("05_AUDIO");
        Transform nhomManager = LayHoacTaoRoot("99_GAME_MANAGER");

        Camera cam = Camera.main;
        if (cam != null)
            DatCha(cam.transform, nhomCamera);

        GameObject manager = GameObject.Find("Trình quản lý trò chơi rắn");
        if (manager != null)
            DatCha(manager.transform, nhomManager);

        TaoBoardPreview(nhomBoard);
        TaoRanPreview(nhomRan);
        TaoMoiPreview(nhomMoi);
        TaoAudioPreview(nhomAudio);

        // Nhóm UI đã được tạo sẵn trong scene. Dòng này chỉ đảm bảo nhóm không bị rỗng nếu mở nhầm scene cũ.
        if (nhomUI.childCount == 0)
            TaoCon(nhomUI, "Giao diện người chơi - mở bản scene mới để sửa UI đầy đủ");

        if (!Application.isPlaying)
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        if (hienThongBao)
            EditorUtility.DisplayDialog("Snake", "Đã tạo lại Hierarchy đầy đủ. Hãy mở các nhóm 01, 02, 03, 04, 05 để sửa.", "OK");
    }

    private static void TaoBoardPreview(Transform nhomBoard)
    {
        Transform mau = TaoCon(nhomBoard, "MAU_DE_SUA_NEN_TUONG");
        Transform sinhKhiChoi = TaoCon(nhomBoard, "DOI_TUONG_SINH_KHI_CHOI");
        sinhKhiChoi.SetAsLastSibling();

        Sprite spriteVuong = LoadSprite("Assets/Resources/AnhRan/o_vuong.png");
        CaiSprite(TaoCon(mau, "Mẫu nền lớn - sửa màu nền ở đây"), spriteVuong, new Color(0.16f, 0.24f, 0.30f), -20, new Vector3(0f, 0f, 0f), new Vector3(6.7f, 9.4f, 1f));
        CaiSprite(TaoCon(mau, "Mẫu vùng chơi - sửa màu vùng chơi"), spriteVuong, new Color(0.08f, 0.13f, 0.17f), -19, new Vector3(0f, 0f, -0.01f), new Vector3(5.7f, 8.36f, 1f));
        CaiSprite(TaoCon(mau, "Mẫu tường trên"), spriteVuong, new Color(0.26f, 0.39f, 0.50f), -10, new Vector3(0f, 4.35f, 0f), new Vector3(6.4f, 0.38f, 1f));
        CaiSprite(TaoCon(mau, "Mẫu tường dưới"), spriteVuong, new Color(0.26f, 0.39f, 0.50f), -10, new Vector3(0f, -4.35f, 0f), new Vector3(6.4f, 0.38f, 1f));
        CaiSprite(TaoCon(mau, "Mẫu tường trái"), spriteVuong, new Color(0.26f, 0.39f, 0.50f), -10, new Vector3(-3.05f, 0f, 0f), new Vector3(0.38f, 8.75f, 1f));
        CaiSprite(TaoCon(mau, "Mẫu tường phải"), spriteVuong, new Color(0.26f, 0.39f, 0.50f), -10, new Vector3(3.05f, 0f, 0f), new Vector3(0.38f, 8.75f, 1f));
    }

    private static void TaoRanPreview(Transform nhomRan)
    {
        Transform mau = TaoCon(nhomRan, "MAU_DE_SUA_RAN");
        Transform sinhKhiChoi = TaoCon(nhomRan, "CAC_DOT_RAN_SINH_KHI_CHOI");
        sinhKhiChoi.SetAsLastSibling();

        Sprite dauRan = LoadSprite("Assets/Resources/AnhRan/dau_ran.png");
        Sprite thanRan = LoadSprite("Assets/Resources/AnhRan/than_ran.png");
        Sprite spriteVuong = LoadSprite("Assets/Resources/AnhRan/o_vuong.png");

        CaiSprite(TaoCon(mau, "Mẫu đầu rắn - sửa màu/sprite ở đây"), dauRan != null ? dauRan : spriteVuong, Color.white, 3, new Vector3(-0.45f, 0f, 0f), new Vector3(0.38f, 0.38f, 1f));
        CaiSprite(TaoCon(mau, "Mẫu thân rắn - sửa màu/sprite ở đây"), thanRan != null ? thanRan : spriteVuong, Color.white, 2, new Vector3(-0.85f, 0f, 0f), new Vector3(0.38f, 0.38f, 1f));
    }

    private static void TaoMoiPreview(Transform nhomMoi)
    {
        Transform mau = TaoCon(nhomMoi, "MAU_DE_SUA_MOI");
        Transform sinhKhiChoi = TaoCon(nhomMoi, "CAC_MOI_SINH_KHI_CHOI");
        sinhKhiChoi.SetAsLastSibling();

        Sprite moi = LoadSprite("Assets/Resources/AnhRan/moi.png");
        Sprite spriteVuong = LoadSprite("Assets/Resources/AnhRan/o_vuong.png");
        CaiSprite(TaoCon(mau, "Mẫu mồi - sửa màu/sprite ở đây"), moi != null ? moi : spriteVuong, Color.white, 4, new Vector3(0f, 0f, 0f), new Vector3(0.32f, 0.32f, 1f));
    }

    private static void TaoAudioPreview(Transform nhomAudio)
    {
        Transform nguon = TaoCon(nhomAudio, "Nguồn phát âm thanh");
        AudioSource source = nguon.GetComponent<AudioSource>();
        if (source == null)
            source = nguon.gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;

        Transform anMoi = TaoCon(nhomAudio, "Audio ăn mồi - an_moi.wav");
        AudioSource sourceAnMoi = anMoi.GetComponent<AudioSource>();
        if (sourceAnMoi == null)
            sourceAnMoi = anMoi.gameObject.AddComponent<AudioSource>();
        sourceAnMoi.playOnAwake = false;
        sourceAnMoi.clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Resources/Audio/an_moi.wav");

        Transform chet = TaoCon(nhomAudio, "Audio chết - chet.wav");
        AudioSource sourceChet = chet.GetComponent<AudioSource>();
        if (sourceChet == null)
            sourceChet = chet.gameObject.AddComponent<AudioSource>();
        sourceChet.playOnAwake = false;
        sourceChet.clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Resources/Audio/chet.wav");
    }

    private static Transform LayHoacTaoRoot(string ten)
    {
        GameObject obj = GameObject.Find(ten);
        if (obj == null)
            obj = new GameObject(ten);

        obj.transform.SetParent(null);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
        return obj.transform;
    }

    private static Transform TaoCon(Transform cha, string ten)
    {
        Transform con = cha.Find(ten);
        if (con == null)
        {
            GameObject obj = new GameObject(ten);
            con = obj.transform;
            DatCha(con, cha);
        }

        con.localRotation = Quaternion.identity;
        return con;
    }

    private static void DatCha(Transform con, Transform cha)
    {
        if (con.parent != cha)
            con.SetParent(cha, false);
    }

    private static void CaiSprite(Transform obj, Sprite sprite, Color mau, int sortingOrder, Vector3 localPos, Vector3 localScale)
    {
        obj.localPosition = localPos;
        obj.localScale = localScale;

        SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
        if (renderer == null)
            renderer = obj.gameObject.AddComponent<SpriteRenderer>();

        renderer.sprite = sprite;
        renderer.color = mau;
        renderer.sortingOrder = sortingOrder;
    }

    private static Sprite LoadSprite(string path)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }
}
#endif
