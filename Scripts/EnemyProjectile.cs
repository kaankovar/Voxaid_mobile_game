using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float damage = 15f;
    public float speed = 10f;
    public float lifeTime = 3f;

    private float disableTime;
    private Rigidbody rb; 

    void Awake()
    {
        
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        
        disableTime = Time.time + lifeTime;
    }

    void Update()
    {
        
        if (Time.time >= disableTime)
        {
            DisableBullet();
        }
    }

    void DisableBullet()
    {
        gameObject.SetActive(false);
    }

    public void Launch(Vector3 direction)
    {
        transform.rotation = Quaternion.LookRotation(direction);
        
        
        if (rb != null) 
        {
            rb.linearVelocity = direction * speed;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Wall")) DisableBullet();
        
        if (other.CompareTag("Player"))
        {
            
            if (other.TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(damage);
                
                if(CameraShake.Instance != null) 
                    CameraShake.Instance.Shake(0.2f, 0.2f); 
            }
            DisableBullet();
        }
    }
}