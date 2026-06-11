using UnityEngine;

public class SurpriseBoxShake : MonoBehaviour
{
    [Header("Titreşim Ayarları")]
    public float shakeAmount = 5f;  
    public float shakeSpeed = 20f;  
    
    [Header("Canlılık Ayarları")]
    public bool useScalePulse = true; 
    public float scaleAmount = 0.05f; 
    public float scaleSpeed = 5f;     

    [Header("Durum")]
    public bool isShaking = true; 

    private Quaternion initialRotation;
    private Vector3 initialScale;
    private float timeOffset; 

    void Start()
    {
        initialRotation = transform.localRotation;
        initialScale = transform.localScale;
        timeOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        if (isShaking)
        {
            PerformShake();
        }
        else
        {
            
            transform.localRotation = Quaternion.Lerp(transform.localRotation, initialRotation, Time.unscaledDeltaTime * 5f);
            transform.localScale = Vector3.Lerp(transform.localScale, initialScale, Time.unscaledDeltaTime * 5f);
        }
    }

    void PerformShake()
    {
        
        float zAngle = Mathf.Sin((Time.unscaledTime + timeOffset) * shakeSpeed) * shakeAmount;
        transform.localRotation = initialRotation * Quaternion.Euler(0, 0, zAngle);

        if (useScalePulse)
        {
            
            float scaleDelt = Mathf.Sin((Time.unscaledTime + timeOffset) * scaleSpeed) * scaleAmount;
            transform.localScale = initialScale + (Vector3.one * scaleDelt);
        }
    }

    public void SetShaking(bool state)
    {
        isShaking = state;
    }
}