using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    public float defaultOrthoSize = 5.0f;              // What you used for 9:16
    public float defaultAspect = 9f / 16f;             // 9:16 portrait

    void Start()
    {
#if UNITY_ANDROID
        float targetAspect = defaultAspect;
        float currentAspect = (float)Screen.width / Screen.height;

        float scale = targetAspect / currentAspect;
        Camera.main.orthographicSize = defaultOrthoSize * scale;
#endif
    }
}
