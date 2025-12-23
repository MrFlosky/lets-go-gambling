using UnityEngine;

public class MouseLock : MonoBehaviour
{
    void Start()
    {
        LockCursor();
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
