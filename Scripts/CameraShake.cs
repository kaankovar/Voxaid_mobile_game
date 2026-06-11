using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private Vector3 initialLocalPos;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.0f;
    private float dampingSpeed = 1.0f;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        initialLocalPos = transform.localPosition;
    }

    void Update()
    {
        if (Time.timeScale == 0)
        {
            transform.localPosition = initialLocalPos;
            return;
        }

        if (shakeDuration > 0)
        {
            transform.localPosition = initialLocalPos + Random.insideUnitSphere * shakeMagnitude;
            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0f;
            transform.localPosition = initialLocalPos;
        }
    }

    public void Shake(float duration, float magnitude)
    {
        if (Time.timeScale == 0) return;

        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }
    
    public void StopShake()
    {
        shakeDuration = 0f;
        transform.localPosition = initialLocalPos;
    }
}