using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake INSTANCE;

    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.1f;

    private Vector3 originalPos;
    private float shakeTime = 0f;

    void Awake()
    {
        INSTANCE = this;
    }

    void Start()
    {
        originalPos = transform.localPosition;
    }

    void Update()
    {
        if (shakeTime > 0)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeMagnitude;
            shakeTime -= Time.deltaTime;
        }
        else
        {
            shakeTime = 0f;
            transform.localPosition = originalPos;
        }
    }

    public void TriggerShake(float duration = 0.2f, float magnitude = 0.1f)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
        shakeTime = shakeDuration;
    }
}
