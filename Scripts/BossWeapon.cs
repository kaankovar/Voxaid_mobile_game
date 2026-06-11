using UnityEngine;

public class BossWeapon : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damageAmount = 50f; 

    
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount); 
            }
        }
    }
}