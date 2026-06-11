using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public float healAmount = 20f; 
     public float moveSpeed = 10f;
    
    private Transform target;
    private bool isCollected = false;
    void FixedUpdate()
    {
        transform.Rotate(0,0,2f);
    }
    void OnTriggerEnter(Collider other)
    {
        AudioManager.Instance.PlaySFX(SoundType.PickupHealth);
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            
            if (health != null)
            {
                health.Heal(healAmount);
                
                gameObject.SetActive(false);
            }
        }
    }
    void OnEnable()
    {
        isCollected = false;
        target = null;
    }

    void Update()
    {
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            moveSpeed += 5f * Time.deltaTime; 
        }
    }

    public void StartCollecting(Transform playerTransform)
    {
        if (!isCollected)
        {
            isCollected = true;
            target = playerTransform;
        }
    }
}