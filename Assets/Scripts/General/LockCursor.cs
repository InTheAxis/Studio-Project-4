using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockCursor : MonoBehaviour
{
    public bool lockCursor = false;

    private void Awake()
    {
        Debug.LogError("LockCursor has been deprecated");
        //Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private void OnDestroy()
    {
        Debug.LogError("LockCursor has been deprecated");
        //Cursor.lockState = CursorLockMode.None;
    }

#if UNITY_EDITOR
    private void Update()
    {
#if !UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            lockCursor = !lockCursor;
        }
#endif
        Debug.LogError("LockCursor has been deprecated");
        //Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
    }
#endif
    }
