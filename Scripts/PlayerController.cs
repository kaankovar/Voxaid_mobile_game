using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Controls")]
    public MobileJoystick moveJoystick;
    public MobileJoystick lookJoystick; 

    [Header("Settings")]
    public float moveSpeed = 8f;
    public float rotationSpeed = 15f; 
    
    private Rigidbody rb;
    private Vector3 moveInput;
    private Vector3 lookInput;
    
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        
        float moveX = moveJoystick.InputDirection.x;
        float moveZ = moveJoystick.InputDirection.y;
        moveInput = new Vector3(moveX, 0, moveZ);

        float lookX = lookJoystick.InputDirection.x;
        float lookZ = lookJoystick.InputDirection.y;
        lookInput = new Vector3(lookX, 0, lookZ);

        HandleAnimations();
    }

    void FixedUpdate()
    {

    
    Vector3 newVelocity = new Vector3(moveInput.x * moveSpeed, 0, moveInput.z * moveSpeed);
    rb.linearVelocity = newVelocity;
        if (lookInput.sqrMagnitude > 0.01f)
        {
            Vector3 direction = lookInput.normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            
            
            Quaternion nextRotation = Quaternion.Lerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(nextRotation);
        }
    }

    void HandleAnimations()
    {
        if (animator == null) return;

        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        if (isMoving)
        {
            
            Vector3 localMove = transform.InverseTransformDirection(moveInput);
            
            
            
            
            
            if (localMove.z < -0.1f) 
            {
                animator.SetBool("backwalk", true);
                animator.SetBool("directwalk", false);
            }
            else
            {
                animator.SetBool("backwalk", false);
                animator.SetBool("directwalk", true);
            }
        }
        else
        {
            animator.SetBool("backwalk", false);
            animator.SetBool("directwalk", false);
        }
    }
}