using UnityEngine;

public class Landmine : MonoBehaviour
{
    private Collider[] mineColliders = new Collider[30]; 
    public float damage = 50f;
    public float explosionRadius = 3f;
    public float triggerDelay = 0.5f; 
    public GameObject explosionVFX; 

    private bool isArmed = false;

    void Start()
    {
        Invoke("ArmMine", triggerDelay);
    }

    void ArmMine()
    {
        isArmed = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isArmed) return;

        if (other.CompareTag("Enemy"))
        {
            Explode();
        }
    }

    void Explode()
    {
        AudioManager.Instance.PlaySFX(SoundType.Explosion);
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, mineColliders);
        
        for (int i = 0; i < hitCount; i++)
        {
            Collider hit = mineColliders[i];
            if (hit.CompareTag("Enemy"))
            {
                hit.GetComponent<EnemyController>()?.TakeDamage(damage);
            }
        }

        
        if (explosionVFX != null) Instantiate(explosionVFX, transform.position, Quaternion.identity);
        
        Destroy(gameObject); 
    }
}