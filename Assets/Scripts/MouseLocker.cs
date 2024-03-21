using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MouseLocker
{
    // Start is called before the first frame update
    public static void Lock()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public static void Unlock()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    public static void ToggleLock()
    {
        if (Cursor.lockState == CursorLockMode.Confined)
        {
            Lock();
        }
        else
        {
            Unlock();
        }
    }

    public static void SetLock(bool value)
    {
        if (value)
        {
            Lock();
        }
        else
        {
            Unlock();
        }
    }
}
