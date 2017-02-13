using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    public static float Width { get; private set; }
    public static float Height { get; private set; }

    void Awake()
    {
        Width = transform.localScale.x * GetComponent<MeshFilter>().sharedMesh.bounds.size.x;
        Height = transform.localScale.z * GetComponent<MeshFilter>().sharedMesh.bounds.size.z;
    }

    /* Having the game be multiplayer, it doesn't make sense that the size of the arena is calculated depending on
     * the host's camera's aspect ratio. For this reason, the arena will now have a fixed square size.
     * However, due to all the references in the code base to this class (to get the width and height), it is
     * better to just keep the class than replace all the references.
     * 
    void Update()
    {
#if UNITY_EDITOR 
        if (!Application.isPlaying)
        {
            Calculate();
        }
#endif
    }

    public void Calculate()
    {
        if (Cam != null)
        {
            Height = CameraUtils.FrustumHeightAtDistance(Cam.farClipPlane - 1.0f, Cam.fieldOfView);
            Width = Height * Cam.aspect;
            transform.localScale = new Vector3(Width * 0.1f, 1.0f, Height * 0.1f);
        }
    }
    */
}
