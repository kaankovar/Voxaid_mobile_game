using UnityEngine;

public class OrbitalBlade : MonoBehaviour
{
    [Header("Settings")]
    public float rotateSpeed = 2f; 
    public float selfRotationSpeed = 500f; 
    public float damage = 20f;
    public float knockback = 5f;
    public float distance = 3f; 

    private Transform player;
    private float currentAngle = 0f; 

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        
        
        if (player != null)
        {
            UpdatePosition();
        }
    }

    void Update()
    {
        if (player != null)
        {
            
            currentAngle += rotateSpeed * Time.deltaTime;

            
            UpdatePosition();
            
            
            transform.Rotate(Vector3.up * selfRotationSpeed * Time.deltaTime);
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    void UpdatePosition()
    {
        
        float x = Mathf.Cos(currentAngle) * distance;
        float z = Mathf.Sin(currentAngle) * distance;

        
        Vector3 targetPos = new Vector3(player.position.x + x, player.position.y + 0.5f, player.position.z + z);
        
        transform.position = targetPos;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                enemy.ApplyKnockback(player.position, knockback);
                AudioManager.Instance.PlaySFX(SoundType.OrbitalBlade);
            }
        }
    }
}