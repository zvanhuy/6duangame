using UnityEngine;
using UnityEngine.InputSystem;

public static class SnakeInputReader
{
    public static bool KiemTraBamPause()
    {
        if (Keyboard.current == null)
            return false;

        return Keyboard.current.pKey.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame;
    }

    public static bool KiemTraBamChoiLai()
    {
        if (Keyboard.current == null)
            return false;

        return Keyboard.current.rKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame;
    }

    public static Vector2Int DocHuongDi(Vector2Int huongDi, Vector2Int huongCho, ref Vector2 viTriChamBatDau, ref bool dangCham)
    {
        if (Keyboard.current != null)
        {
            if ((Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame) && huongDi != Vector2Int.down)
                huongCho = Vector2Int.up;
            else if ((Keyboard.current.downArrowKey.wasPressedThisFrame || Keyboard.current.sKey.wasPressedThisFrame) && huongDi != Vector2Int.up)
                huongCho = Vector2Int.down;
            else if ((Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.aKey.wasPressedThisFrame) && huongDi != Vector2Int.right)
                huongCho = Vector2Int.left;
            else if ((Keyboard.current.rightArrowKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame) && huongDi != Vector2Int.left)
                huongCho = Vector2Int.right;
        }

        return DocVuotManHinh(huongDi, huongCho, ref viTriChamBatDau, ref dangCham);
    }

    private static Vector2Int DocVuotManHinh(Vector2Int huongDi, Vector2Int huongCho, ref Vector2 viTriChamBatDau, ref bool dangCham)
    {
        if (Touchscreen.current == null)
            return huongCho;

        var touch = Touchscreen.current.primaryTouch;
        if (touch.press.wasPressedThisFrame)
        {
            viTriChamBatDau = touch.position.ReadValue();
            dangCham = true;
        }

        if (dangCham && touch.press.wasReleasedThisFrame)
        {
            Vector2 ketThuc = touch.position.ReadValue();
            Vector2 lech = ketThuc - viTriChamBatDau;
            dangCham = false;

            if (lech.magnitude < 40f)
                return huongCho;

            if (Mathf.Abs(lech.x) > Mathf.Abs(lech.y))
            {
                if (lech.x > 0 && huongDi != Vector2Int.left)
                    huongCho = Vector2Int.right;
                else if (lech.x < 0 && huongDi != Vector2Int.right)
                    huongCho = Vector2Int.left;
            }
            else
            {
                if (lech.y > 0 && huongDi != Vector2Int.down)
                    huongCho = Vector2Int.up;
                else if (lech.y < 0 && huongDi != Vector2Int.up)
                    huongCho = Vector2Int.down;
            }
        }

        return huongCho;
    }
}
