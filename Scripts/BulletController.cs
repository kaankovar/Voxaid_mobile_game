using UnityEngine;

public class BulletController : MonoBehaviour
{
    [HideInInspector] public GameObject ignoredEnemy;
    private static Collider[] aoeColliders = new Collider[30]; 
    private float nextSearchTime = 0f;
    
    [Header("Bullet Stats")]
    public float damage = 10f;
    public float lifeTime = 2f;
    private int currentPierce = 0;
    private float knockback = 0f;
    private bool isCrit = false;
    
    [Header("VFX Prefabs")]
    public GameObject lightningPrefab;
    
    [Header("Special Abilities")]
    public float executionThreshold = 0f; 
    public bool canExplode = false;
    public float eliteBonus = 0f;
    public bool isPoison;
    public bool isFire;
    public bool isSplit;
    public bool isHoming;
    public bool isChainLightning; 

    private Transform targetEnemy;
    private Rigidbody rb;
    public bool hasRicocheted = false; 

    
    private float disableTime = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable() 
    { 
        hasRicocheted = false;
    }

    void DisableBullet() 
    { 
        gameObject.SetActive(false); 
    }

    public void Setup(int pierce, float dmg, float kb, bool crit, float life)
    {
        currentPierce = pierce; 
        damage = dmg; 
        knockback = kb; 
        isCrit = crit; 
        lifeTime = life;
        targetEnemy = null;
        ignoredEnemy = null; 
        
        if(rb == null) rb = GetComponent<Rigidbody>();
        
        
        disableTime = Time.time + lifeTime;
    }

    void Update()
    {
        
        if (Time.time >= disableTime)
        {
            DisableBullet();
        }
    }

    void FixedUpdate() 
    {
        if (isHoming)
        {
            if (targetEnemy == null || !targetEnemy.gameObject.activeInHierarchy)
            {
                if (Time.time >= nextSearchTime)
                {
                    targetEnemy = FindClosestEnemy();
                    nextSearchTime = Time.time + 0.2f; 
                }
            }
            
            if (targetEnemy != null)
            {
                Vector3 direction = (targetEnemy.position - transform.position).normalized;
                float rotateSpeed = 25f; 
                
                float currentSpeed = rb.linearVelocity.magnitude;
                Vector3 newVelocity = Vector3.RotateTowards(rb.linearVelocity, direction, rotateSpeed * Time.fixedDeltaTime, 0f);
                
                rb.linearVelocity = newVelocity.normalized * currentSpeed;
                transform.rotation = Quaternion.LookRotation(rb.linearVelocity); 
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Bullet") || other.CompareTag("Armor") || other.CompareTag("Item")) return;
        if (ignoredEnemy != null && other.gameObject == ignoredEnemy) return;
        
        if (other.CompareTag("Wall")) 
        {
            if (WeaponController.Instance != null && WeaponController.Instance.enableRicochet)
            {
                ReflectProjectile();
            }
            else
            {
                DisableBullet();
            }
            return;
        }
        
        
        if (other.TryGetComponent(out EnemyController enemy))
        {
            float hpPercent = enemy.currentHealth / enemy.maxHealth;
            if(executionThreshold > 0 && hpPercent <= executionThreshold)
            {
                enemy.TakeDamage(99999, true); 
            }
            else
            {
                float finalDmg = damage;
                if(eliteBonus > 0 && enemy.isElite) finalDmg *= (1f + eliteBonus);
                enemy.TakeDamage(finalDmg, isCrit);
            }

            if(knockback > 0) enemy.ApplyKnockback(transform.position, knockback);
            
            if (isPoison) enemy.ApplyDoT(damage * 0.2f, 3f, "Poison"); 
            if (isFire) enemy.ApplyDoT(damage * 0.1f, 5f, "Fire");

            if (isChainLightning)
            {
                TriggerChainLightning(other.transform.position, other.gameObject);
            } 

            if (isSplit)
            {
                isSplit = false; 
                Vector3 currentDir = rb.linearVelocity.normalized;
                Vector3 dir1 = Quaternion.Euler(0, 45, 0) * currentDir;
                Vector3 dir2 = Quaternion.Euler(0, -45, 0) * currentDir;

                SpawnSplitBullet(transform.position, dir1, other.gameObject);
                SpawnSplitBullet(transform.position, dir2, other.gameObject);
            }
            
            ProcessBounceAndPierce(other.transform);
        }
        else if (other.TryGetComponent(out IDamageable damageable)) 
        {
            damageable.TakeDamage(damage);
            ProcessBounceAndPierce(other.transform);
        }
    }

    private void ProcessBounceAndPierce(Transform hitTransform)
    {
        bool didBounce = false;
        if (WeaponController.Instance != null && WeaponController.Instance.enableRicochet)
        {
            didBounce = BounceToNextEnemy(hitTransform);
        }
        
        if (!didBounce)
        {
            if (currentPierce > 0) currentPierce--;
            else DisableBullet();
        }
    }

    void ReflectProjectile()
    {
        AudioManager.Instance.PlaySFX(SoundType.Ricochet);
        Vector3 currentDir = rb.linearVelocity.normalized;
        
        Vector3 rayStartPos = transform.position - (currentDir * 1.5f);

        if (Physics.Raycast(rayStartPos, currentDir, out RaycastHit hit, 5f)) 
        {
            Vector3 reflectDir = Vector3.Reflect(currentDir, hit.normal);
            
            if (reflectDir == -currentDir) reflectDir = Quaternion.Euler(0, 15f, 0) * reflectDir;

            
            reflectDir.y = 0f;
            reflectDir.Normalize();

            transform.rotation = Quaternion.LookRotation(reflectDir);
            
            
            rb.linearVelocity = reflectDir * rb.linearVelocity.magnitude;
            
            disableTime = Time.time + 1.5f; 
        }
        else
        {
            Vector3 reverseDir = -currentDir;
            
            
            reverseDir.y = 0f;
            reverseDir.Normalize();

            transform.rotation = Quaternion.LookRotation(reverseDir); 
            rb.linearVelocity = reverseDir * rb.linearVelocity.magnitude;
        }
    }

    bool BounceToNextEnemy(Transform hitEnemy)
    {
        if (currentPierce <= 0) return false; 

        Transform nextTarget = null;
        float closestSqrDist = Mathf.Infinity; 
        
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, 12f, aoeColliders); 

        for (int i = 0; i < hitCount; i++)
        {
            Collider hit = aoeColliders[i];
            if (hit.CompareTag("Enemy") && hit.transform != hitEnemy) 
            {
                
                float d = (hit.transform.position - transform.position).sqrMagnitude;
                if (d < closestSqrDist)
                {
                    closestSqrDist = d;
                    nextTarget = hit.transform;
                }
            }
        }

        if (nextTarget != null)
        {
            currentPierce--; 
            Vector3 bounceDir = (nextTarget.position - transform.position).normalized;
            
            rb.linearVelocity = bounceDir * rb.linearVelocity.magnitude; 
            transform.rotation = Quaternion.LookRotation(bounceDir); 
            
            disableTime = Time.time + 1.5f; 
            return true;
        }
        return false;
    }

    void TriggerChainLightning(Vector3 center, GameObject ignoreObj)
    {
        int hitCount = Physics.OverlapSphereNonAlloc(center, 6f, aoeColliders);
        
        Transform bestLightningTarget = null;
        float minLightningSqrDist = Mathf.Infinity; 

        for (int i = 0; i < hitCount; i++)
        {
            Collider n = aoeColliders[i];
            if(n.CompareTag("Enemy") && n.gameObject != ignoreObj)
            {
                
                float sqrDist = (n.transform.position - center).sqrMagnitude;
                if(sqrDist < minLightningSqrDist)
                {
                    minLightningSqrDist = sqrDist;
                    bestLightningTarget = n.transform;
                }
            }
        }

        if(bestLightningTarget != null)
        {
            
            if (bestLightningTarget.TryGetComponent(out IDamageable targetDamageable))
            {
                targetDamageable.TakeDamage(damage * 0.5f);
            }
            
            if (lightningPrefab != null)
            {
                
                GameObject vfx = Instantiate(lightningPrefab, Vector3.zero, Quaternion.identity);
                if (vfx.TryGetComponent(out LightningVisual arcScript))
                {
                    arcScript.Setup(center, bestLightningTarget.position);
                }
            }
        }
    }

    Transform FindClosestEnemy()
    {
        Transform bestTarget = null;
        float closestSqrDist = Mathf.Infinity; 
        
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, 15f, aoeColliders);
        for (int i = 0; i < hitCount; i++)
        {
            Collider hit = aoeColliders[i];
            if(hit.CompareTag("Enemy"))
            {
                float sqrDist = (hit.transform.position - transform.position).sqrMagnitude;
                if(sqrDist < closestSqrDist)
                {
                    closestSqrDist = sqrDist;
                    bestTarget = hit.transform;
                }
            }
        }
        return bestTarget;
    }
    
    void SpawnSplitBullet(Vector3 pos, Vector3 direction, GameObject hitEnemy)
    {
        if(WeaponController.Instance == null || WeaponController.Instance.currentWeapon == null) return;

        string poolName = WeaponController.Instance.currentWeapon.bulletTag;
        Quaternion visualRot = Quaternion.LookRotation(direction);
        
        GameObject b = PoolManager.Instance.SpawnFromPool(poolName, pos, visualRot);
        
        if(b != null)
        {
            if (b.TryGetComponent(out BulletController bc))
            {
                bc.Setup(0, damage * 0.5f, 0, false, 1f); 
                bc.ignoredEnemy = hitEnemy;                 
                WeaponController.Instance.ConfigureBullet(bc);
                bc.isSplit = false;
            }
            
            if (b.TryGetComponent(out Rigidbody brb)) 
            {
                float trueSpeed = WeaponController.Instance.currentWeapon.projectileSpeed * WeaponController.Instance.projectileSpeedMult;
                brb.linearVelocity = direction * trueSpeed;
            }
        }
    }
}