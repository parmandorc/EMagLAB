using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Class for fixing the aspect ratio of the screen to 1:1 (for gameplay reasons: map is squared).
 * For now, since the game doesn't allow resizing of the window, we only need to fix the ratio on Start.
 */ 
[RequireComponent(typeof(Camera))]
public class CameraAspectRatio : MonoBehaviour
{
    private Camera mCamera;

    void Start()
    {
        mCamera = GetComponent<Camera>();

        FixAspectRatio();
    }

    void FixAspectRatio()
    {
        int width = Screen.width;
        int height = Screen.height;

        if (width != 0 && height != 0)
        {
            float ratio = (float)height / width;

            if (ratio < 1.0f)
            {
                mCamera.rect = new Rect((1.0f - ratio) * 0.5f, 0.0f, ratio, 1.0f);
            }
            else if (ratio > 1.0f)
            {
                mCamera.rect = new Rect(0.0f, (1.0f - 1.0f / ratio) * 0.5f, 1.0f, 1.0f / ratio);
            }

        }
    }
}
