using UnityEngine;
using UnityEngine.EventSystems;

public class MobileJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Görsel Referanslar")]
    
    public RectTransform joystickBackground; 
    
    
    public RectTransform handle;             
    
    [Header("Ayarlar")]
    [Tooltip("Merkezden kaç piksel uzaklaşınca %100 güç versin? (Örn: 100)")]
    public float joystickRadius = 100f; 
    
    public bool isDynamic = true; 
    
    
    public Vector2 InputDirection { get; private set; }

    private Vector2 defaultPos; 
    private RectTransform zoneRect; 

    void Start()
    {
        
        zoneRect = GetComponent<RectTransform>();
        
        
        if(joystickBackground != null)
            defaultPos = joystickBackground.anchoredPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isDynamic)
        {
            
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                zoneRect, 
                eventData.position, 
                eventData.pressEventCamera, 
                out localPoint))
            {
                
                joystickBackground.anchoredPosition = localPoint;
                
                
                handle.anchoredPosition = Vector2.zero;
            }
        }
        
        
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position;
        
        
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground, 
            eventData.position, 
            eventData.pressEventCamera, 
            out position))
        {
            
            
            Vector2 inputVector = position / joystickRadius;
            
            
            if (inputVector.magnitude > 1)
            {
                inputVector = inputVector.normalized;
            }

            
            InputDirection = inputVector;

            
            handle.anchoredPosition = InputDirection * joystickRadius;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
        InputDirection = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
        
        
        if(isDynamic)
        {
             joystickBackground.anchoredPosition = defaultPos;
        }
    }
    
    void OnDisable()
    {
        ResetJoystick();
    }

    
    public void ResetJoystick()
    {
        
        InputDirection = Vector2.zero;
        
        
        if (handle != null) 
        {
            handle.anchoredPosition = Vector2.zero;
        }
        
        
        if (isDynamic && joystickBackground != null)
        {
             joystickBackground.anchoredPosition = defaultPos;
        }
    }
}