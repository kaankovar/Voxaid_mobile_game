using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform target; 

    [Header("References")]
    public MobileJoystick lookJoystick; 

    [Header("Settings")]
    public float smoothSpeed = 5f;
    public Vector3 baseOffset; 
    
    [Header("Peaking (Kaydırma)")]
    public float peakAmount = 4f; 
    public float peakSmooth = 10f; 

    private Vector3 currentPeakOffset; 

    void Start()
    {
        if (target != null)
        {
            Vector3 alignedPos = new Vector3(target.position.x, transform.position.y, transform.position.z);
            transform.position = alignedPos;

            baseOffset = transform.position - target.position;
            
            transform.position = target.position + baseOffset;
        }
        
        if(lookJoystick == null)
        {
            GameObject joyObj = GameObject.Find("LookJoystick");
            if (joyObj != null) lookJoystick = joyObj.GetComponent<MobileJoystick>();
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 joystickDir = new Vector3(lookJoystick.InputDirection.x, 0, lookJoystick.InputDirection.y);
        
        Vector3 targetPeakOffset = joystickDir * peakAmount;

        currentPeakOffset = Vector3.Lerp(currentPeakOffset, targetPeakOffset, peakSmooth * Time.deltaTime);

        Vector3 finalPosition = target.position + baseOffset + currentPeakOffset;

        transform.position = Vector3.Lerp(transform.position, finalPosition, smoothSpeed * Time.deltaTime);
    }
}