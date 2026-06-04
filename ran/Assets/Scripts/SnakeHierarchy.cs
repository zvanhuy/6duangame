using UnityEngine;

public class SnakeHierarchy
{
    private readonly Transform quanLyChinh;

    public Transform NhomCamera { get; private set; }
    public Transform NhomBanChoi { get; private set; }
    public Transform NhomRan { get; private set; }
    public Transform NhomMoi { get; private set; }
    public Transform NhomGiaoDien { get; private set; }
    public Transform NhomAmThanh { get; private set; }
    public Transform NhomQuanLy { get; private set; }

    public SnakeHierarchy(Transform quanLyChinh)
    {
        this.quanLyChinh = quanLyChinh;
    }

    public void TaoCacNhomHierarchy()
    {
        NhomCamera = LayHoacTaoNhom("00_CAMERA");
        NhomBanChoi = LayHoacTaoNhom("01_BOARD_NEN_TUONG");
        NhomRan = LayHoacTaoNhom("02_SNAKE_RAN");
        NhomMoi = LayHoacTaoNhom("03_FOOD_MOI");
        NhomGiaoDien = LayHoacTaoNhom("04_UI_GIAO_DIEN");
        NhomAmThanh = LayHoacTaoNhom("05_AUDIO");
        NhomQuanLy = LayHoacTaoNhom("99_GAME_MANAGER");

        if (quanLyChinh.parent != NhomQuanLy)
            quanLyChinh.SetParent(NhomQuanLy, false);
    }

    public void CaiDatCamera()
    {
        Camera cameraChinh = Camera.main;
        if (cameraChinh == null)
        {
            GameObject objCamera = new GameObject("Camera Chính");
            objCamera.tag = "MainCamera";
            cameraChinh = objCamera.AddComponent<Camera>();
            objCamera.AddComponent<AudioListener>();
        }

        cameraChinh.transform.SetParent(NhomCamera, false);
        cameraChinh.orthographic = true;
        cameraChinh.orthographicSize = 5.1f;
        cameraChinh.backgroundColor = new Color(0.11f, 0.17f, 0.22f);
        cameraChinh.transform.localPosition = new Vector3(0f, 0f, -10f);
        cameraChinh.transform.localRotation = Quaternion.identity;
        cameraChinh.transform.localScale = Vector3.one;
    }

    private Transform LayHoacTaoNhom(string ten)
    {
        GameObject obj = GameObject.Find(ten);
        if (obj == null)
            obj = new GameObject(ten);

        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
        return obj.transform;
    }
}
