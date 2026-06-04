using System.Collections.Generic;
using UnityEngine;

public class QuanLyTroChoiRan : MonoBehaviour
{
    [Header("Cấu hình bàn chơi")]
    [SerializeField] private int soCot = 15;
    [SerializeField] private int soHang = 22;
    [SerializeField] private float kichThuocO = 0.38f;
    [SerializeField] private float thoiGianMoiBuoc = 0.16f;

    [Header("Tăng độ khó theo điểm")]
    [SerializeField] private float thoiGianBuocNhanhNhat = 0.07f;
    [SerializeField] private int diemMoiLanAn = 10;
    [SerializeField] private int moiBaoNhieuDiemTangToc = 50;
    [SerializeField] private int moiBaoNhieuDiemTangMoi = 60;
    [SerializeField] private int soMoiToiDa = 5;

    [Header("Âm thanh")]
    [SerializeField] private float amLuongAnMoi = 0.9f;
    [SerializeField] private float amLuongChet = 1.0f;

    private readonly List<Vector2Int> cacDotRan = new List<Vector2Int>();
    private readonly List<GameObject> hinhDotRan = new List<GameObject>();
    private readonly List<Vector2Int> cacViTriMoi = new List<Vector2Int>();
    private readonly List<GameObject> hinhMoi = new List<GameObject>();

    private Vector2Int huongDi = Vector2Int.right;
    private Vector2Int huongCho = Vector2Int.right;
    private Vector2 viTriChamBatDau;

    private bool dangChoi;
    private bool daChet;
    private bool dangTamDung;
    private bool dangCham;

    private int diem;
    private int diemCaoNhat;
    private int soMoiMucTieu = 1;
    private float demThoiGian;
    private float thoiGianBuocHienTai;

    private const string KhoaLuuDiemCaoNhat = "Snake_HighScore";

    private SnakeHierarchy hierarchy;
    private SnakeAssets assets;
    private SnakeBoardRenderer boardRenderer;
    private SnakeUIController ui;
    private SnakeAudioPlayer audioPlayer;

    private void Start()
    {
        KhoiTaoTroChoi();
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (!dangChoi)
        {
            if (daChet && SnakeInputReader.KiemTraBamChoiLai())
                ChoiLai();

            return;
        }

        if (SnakeInputReader.KiemTraBamPause())
            DoiTrangThaiPause();

        if (dangTamDung)
            return;

        huongCho = SnakeInputReader.DocHuongDi(huongDi, huongCho, ref viTriChamBatDau, ref dangCham);
        demThoiGian += Time.deltaTime;

        if (demThoiGian >= thoiGianBuocHienTai)
        {
            demThoiGian = 0f;
            DiChuyenRan();
        }
    }

    private void KhoiTaoTroChoi()
    {
        hierarchy = new SnakeHierarchy(transform);
        hierarchy.TaoCacNhomHierarchy();
        hierarchy.CaiDatCamera();

        assets = SnakeAssets.TaiTaiNguyen();
        boardRenderer = new SnakeBoardRenderer(hierarchy.NhomBanChoi, hierarchy.NhomRan, hierarchy.NhomMoi, assets, soCot, soHang, kichThuocO);
        audioPlayer = new SnakeAudioPlayer(hierarchy.NhomAmThanh);
        ui = FindFirstObjectByType<SnakeUIController>(FindObjectsInactive.Include);
        if (ui == null)
        {
            Debug.LogError("Không tìm thấy SnakeUIController trong Hierarchy. Hãy kiểm tra nhóm 04_UI_GIAO_DIEN/Giao diện người chơi.");
            enabled = false;
            return;
        }

        diemCaoNhat = PlayerPrefs.GetInt(KhoaLuuDiemCaoNhat, 0);
        thoiGianBuocHienTai = thoiGianMoiBuoc;

        boardRenderer.XoaBanChoi();
        boardRenderer.XoaRanVaMoi();
        boardRenderer.TaoNenVaTuong();

        ui.KhoiTao(BatDauChoi, ChoiLai, DoiTrangThaiPause);
        HienManHinhBatDau();
    }

    private void HienManHinhBatDau()
    {
        dangChoi = false;
        daChet = false;
        dangTamDung = false;
        Time.timeScale = 1f;
        ui.HienManHinhBatDau();
    }

    private void BatDauChoi()
    {
        XoaTatCaDoiTuongGame();

        diem = 0;
        soMoiMucTieu = 1;
        thoiGianBuocHienTai = thoiGianMoiBuoc;
        huongDi = Vector2Int.right;
        huongCho = Vector2Int.right;
        demThoiGian = 0f;
        dangChoi = true;
        daChet = false;
        dangTamDung = false;
        dangCham = false;
        Time.timeScale = 1f;

        TaoRanBanDau();
        CapNhatDoKhoTheoDiem();
        TaoDuSoMoi();
        CapNhatDiem();
        ui.HienManHinhChoi();
    }

    private void ChoiLai()
    {
        BatDauChoi();
    }

    private void DoiTrangThaiPause()
    {
        if (!dangChoi || daChet)
            return;

        dangTamDung = !dangTamDung;
        Time.timeScale = dangTamDung ? 0f : 1f;
        ui.DatTrangThaiPause(dangTamDung);
    }

    private void XoaTatCaDoiTuongGame()
    {
        hinhDotRan.Clear();
        cacDotRan.Clear();
        hinhMoi.Clear();
        cacViTriMoi.Clear();
        boardRenderer.XoaRanVaMoi();
    }

    private void TaoRanBanDau()
    {
        Vector2Int dau = new Vector2Int(soCot / 2, soHang / 2);
        cacDotRan.Add(dau);
        cacDotRan.Add(dau + Vector2Int.left);
        cacDotRan.Add(dau + Vector2Int.left * 2);

        for (int i = 0; i < cacDotRan.Count; i++)
        {
            GameObject dot = boardRenderer.TaoDotRan(i == 0 ? "Đầu rắn" : "Thân rắn", cacDotRan[i], i == 0);
            hinhDotRan.Add(dot);
        }
    }

    private void TaoDuSoMoi()
    {
        while (hinhMoi.Count < soMoiMucTieu)
        {
            if (!TaoMotMoi())
                break;
        }
    }

    private bool TaoMotMoi()
    {
        for (int lanThu = 0; lanThu < 500; lanThu++)
        {
            Vector2Int oMoi = new Vector2Int(Random.Range(0, soCot), Random.Range(0, soHang));
            if (cacDotRan.Contains(oMoi) || cacViTriMoi.Contains(oMoi))
                continue;

            GameObject objMoi = boardRenderer.TaoMoi(oMoi, hinhMoi.Count + 1);
            cacViTriMoi.Add(oMoi);
            hinhMoi.Add(objMoi);
            return true;
        }

        return false;
    }

    private void DiChuyenRan()
    {
        huongDi = huongCho;
        Vector2Int dauCu = cacDotRan[0];
        Vector2Int dauMoi = dauCu + huongDi;

        int chiSoMoiAn = LayChiSoMoiTaiViTri(dauMoi);
        bool anMoi = chiSoMoiAn >= 0;

        if (DauChamTuong(dauMoi) || DauCanThan(dauMoi, anMoi))
        {
            KetThucTroChoi();
            return;
        }

        cacDotRan.Insert(0, dauMoi);
        GameObject dauMoiObj = boardRenderer.TaoDotRan("Đầu rắn", dauMoi, true);
        hinhDotRan.Insert(0, dauMoiObj);
        boardRenderer.DoiDauCuThanhThan(hinhDotRan);

        if (anMoi)
        {
            diem += diemMoiLanAn;
            audioPlayer.PhatAmThanh(assets.AmAnMoi, amLuongAnMoi);
            XoaMoiDaAn(chiSoMoiAn);
            CapNhatDoKhoTheoDiem();
            TaoDuSoMoi();
            CapNhatDiem();
        }
        else
        {
            int cuoi = cacDotRan.Count - 1;
            cacDotRan.RemoveAt(cuoi);
            Destroy(hinhDotRan[cuoi]);
            hinhDotRan.RemoveAt(cuoi);
        }
    }

    private int LayChiSoMoiTaiViTri(Vector2Int viTri)
    {
        for (int i = 0; i < cacViTriMoi.Count; i++)
        {
            if (cacViTriMoi[i] == viTri)
                return i;
        }

        return -1;
    }

    private void XoaMoiDaAn(int chiSo)
    {
        if (chiSo < 0 || chiSo >= hinhMoi.Count)
            return;

        if (hinhMoi[chiSo] != null)
            Destroy(hinhMoi[chiSo]);

        hinhMoi.RemoveAt(chiSo);
        cacViTriMoi.RemoveAt(chiSo);
    }

    private void CapNhatDoKhoTheoDiem()
    {
        int capTocDo = moiBaoNhieuDiemTangToc > 0 ? diem / moiBaoNhieuDiemTangToc : 0;
        thoiGianBuocHienTai = Mathf.Max(thoiGianBuocNhanhNhat, thoiGianMoiBuoc - capTocDo * 0.012f);

        int capSoMoi = moiBaoNhieuDiemTangMoi > 0 ? diem / moiBaoNhieuDiemTangMoi : 0;
        soMoiMucTieu = Mathf.Clamp(1 + capSoMoi, 1, soMoiToiDa);
    }

    private bool DauChamTuong(Vector2Int dau)
    {
        return dau.x < 0 || dau.x >= soCot || dau.y < 0 || dau.y >= soHang;
    }

    private bool DauCanThan(Vector2Int dauMoi, bool anMoi)
    {
        int gioiHanKiemTra = anMoi ? cacDotRan.Count : cacDotRan.Count - 1;
        for (int i = 0; i < gioiHanKiemTra; i++)
        {
            if (cacDotRan[i] == dauMoi)
                return true;
        }

        return false;
    }

    private void KetThucTroChoi()
    {
        if (daChet)
            return;

        dangChoi = false;
        daChet = true;
        dangTamDung = false;
        Time.timeScale = 1f;
        LuuDiemCaoNhatNeuCan();
        CapNhatDiem();
        audioPlayer.PhatAmThanh(assets.AmChet, amLuongChet);
        ui.HienManHinhKetThuc(diem, diemCaoNhat);
    }

    private void LuuDiemCaoNhatNeuCan()
    {
        if (diem <= diemCaoNhat)
            return;

        diemCaoNhat = diem;
        PlayerPrefs.SetInt(KhoaLuuDiemCaoNhat, diemCaoNhat);
        PlayerPrefs.Save();
    }

    private void CapNhatDiem()
    {
        LuuDiemCaoNhatNeuCan();

        float tocDo = thoiGianBuocHienTai > 0f ? thoiGianMoiBuoc / thoiGianBuocHienTai : 1f;
        ui.CapNhatDiem(diem, diemCaoNhat, tocDo, soMoiMucTieu);
    }
}
