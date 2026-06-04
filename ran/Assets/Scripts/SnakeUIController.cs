using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class SnakeUIController : MonoBehaviour
{
    [Header("Màn hình UI trong Hierarchy")]
    [SerializeField] private GameObject manHinhBatDau;
    [SerializeField] private GameObject manHinhKetThuc;
    [SerializeField] private GameObject manHinhTamDung;
    [SerializeField] private GameObject bangDiemTrongManChoi;
    [SerializeField] private GameObject nutPauseObj;

    [Header("Chữ hiển thị")]
    [SerializeField] private Text chuDiem;
    [SerializeField] private Text chuKetThuc;
    [SerializeField] private Text chuNutPause;

    [Header("Các nút bấm")]
    [SerializeField] private Button nutBatDau;
    [SerializeField] private Button nutChoiLai;
    [SerializeField] private Button nutPause;
    [SerializeField] private Button nutTiepTuc;

    private Action khiBamBatDau;
    private Action khiBamChoiLai;
    private Action khiBamPause;

    public void KhoiTao(Action onStart, Action onReplay, Action onPause)
    {
        TuDongTimThanhPhanNeuThieu();
        DamBaoEventSystem();

        khiBamBatDau = onStart;
        khiBamChoiLai = onReplay;
        khiBamPause = onPause;

        if (nutBatDau != null)
        {
            nutBatDau.onClick.RemoveAllListeners();
            nutBatDau.onClick.AddListener(() => khiBamBatDau?.Invoke());
        }

        if (nutChoiLai != null)
        {
            nutChoiLai.onClick.RemoveAllListeners();
            nutChoiLai.onClick.AddListener(() => khiBamChoiLai?.Invoke());
        }

        if (nutPause != null)
        {
            nutPause.onClick.RemoveAllListeners();
            nutPause.onClick.AddListener(() => khiBamPause?.Invoke());
        }

        if (nutTiepTuc != null)
        {
            nutTiepTuc.onClick.RemoveAllListeners();
            nutTiepTuc.onClick.AddListener(() => khiBamPause?.Invoke());
        }
    }

    public void HienManHinhBatDau()
    {
        BatTat(manHinhBatDau, true);
        BatTat(manHinhKetThuc, false);
        BatTat(manHinhTamDung, false);
        BatTat(bangDiemTrongManChoi, false);
        BatTat(nutPauseObj, false);
    }

    public void HienManHinhChoi()
    {
        BatTat(manHinhBatDau, false);
        BatTat(manHinhKetThuc, false);
        BatTat(manHinhTamDung, false);
        BatTat(bangDiemTrongManChoi, true);
        BatTat(nutPauseObj, true);
        DatTrangThaiPause(false);
    }

    public void HienManHinhKetThuc(int diem, int diemCaoNhat)
    {
        BatTat(manHinhBatDau, false);
        BatTat(manHinhKetThuc, true);
        BatTat(manHinhTamDung, false);
        BatTat(bangDiemTrongManChoi, true);
        BatTat(nutPauseObj, false);

        if (chuKetThuc != null)
        {
            chuKetThuc.text = "GAME OVER\n\nĐIỂM: " + diem +
                              "\nCAO NHẤT: " + diemCaoNhat +
                              "\n\nBấm CHƠI LẠI hoặc nhấn R / SPACE";
        }
    }

    public void DatTrangThaiPause(bool dangTamDung)
    {
        BatTat(manHinhTamDung, dangTamDung);

        if (chuNutPause != null)
            chuNutPause.text = dangTamDung ? "TIẾP TỤC" : "PAUSE";
    }

    public void CapNhatDiem(int diem, int diemCaoNhat, float tocDo, int soMoiMucTieu)
    {
        if (chuDiem == null)
            return;

        chuDiem.text = "ĐIỂM: " + diem + "    CAO NHẤT: " + diemCaoNhat +
                       "\nTỐC ĐỘ: x" + tocDo.ToString("0.0") + "    SỐ MỒI: " + soMoiMucTieu;
    }

    private void BatTat(GameObject obj, bool trangThai)
    {
        if (obj != null)
            obj.SetActive(trangThai);
    }

    private void DamBaoEventSystem()
    {
        EventSystem eventSystem = FindFirstObjectByType<EventSystem>(FindObjectsInactive.Include);

        if (eventSystem == null)
        {
            GameObject obj = new GameObject("EventSystem");
            Transform nhomUI = transform.parent != null ? transform.parent : transform;
            obj.transform.SetParent(nhomUI, false);
            eventSystem = obj.AddComponent<EventSystem>();
        }

        StandaloneInputModule oldModule = eventSystem.GetComponent<StandaloneInputModule>();
        if (oldModule != null)
            Destroy(oldModule);

        if (eventSystem.GetComponent<InputSystemUIInputModule>() == null)
            eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
    }

    private void TuDongTimThanhPhanNeuThieu()
    {
        if (manHinhBatDau == null)
            manHinhBatDau = TimCon("Màn hình bắt đầu");
        if (manHinhKetThuc == null)
            manHinhKetThuc = TimCon("Màn hình kết thúc");
        if (manHinhTamDung == null)
            manHinhTamDung = TimCon("Màn hình tạm dừng");
        if (bangDiemTrongManChoi == null)
            bangDiemTrongManChoi = TimCon("Bảng điểm trong màn chơi");
        if (nutPauseObj == null)
            nutPauseObj = TimCon("Nút Pause góc trên");

        if (chuDiem == null && bangDiemTrongManChoi != null)
            chuDiem = bangDiemTrongManChoi.GetComponent<Text>();
        if (chuKetThuc == null)
        {
            GameObject objChuKetThuc = TimCon("Chữ kết thúc");
            if (objChuKetThuc != null)
                chuKetThuc = objChuKetThuc.GetComponent<Text>();
        }
        if (chuNutPause == null && nutPauseObj != null)
            chuNutPause = nutPauseObj.GetComponentInChildren<Text>(true);

        if (nutBatDau == null)
        {
            GameObject obj = TimCon("Nút bắt đầu");
            if (obj != null)
                nutBatDau = obj.GetComponent<Button>();
        }
        if (nutChoiLai == null)
        {
            GameObject obj = TimCon("Nút chơi lại");
            if (obj != null)
                nutChoiLai = obj.GetComponent<Button>();
        }
        if (nutPause == null && nutPauseObj != null)
            nutPause = nutPauseObj.GetComponent<Button>();
        if (nutTiepTuc == null)
        {
            GameObject obj = TimCon("Nút tiếp tục");
            if (obj != null)
                nutTiepTuc = obj.GetComponent<Button>();
        }
    }

    private GameObject TimCon(string ten)
    {
        Transform[] tatCaCon = GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < tatCaCon.Length; i++)
        {
            if (tatCaCon[i].name == ten)
                return tatCaCon[i].gameObject;
        }

        return null;
    }
}
