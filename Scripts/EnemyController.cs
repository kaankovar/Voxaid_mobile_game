using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour, IDamageable
{
    private static Collider[] aoeColliders = new Collider[50];
    
    [Header("Status Effects")]
    private bool isPoisoned = false;
    private bool isBurning = false;
    private bool isFrozen = false;
    
    [Header("VFX Prefabs")]
    public GameObject poisonVFXPrefab; 
    public GameObject fireVFXPrefab;
    private GameObject activePoisonVFX;
    private GameObject activeFireVFX;
    
    [Header("Base Stats")]
    public float baseMoveSpeed = 4f; 
    public float baseHealth = 30f;   
    public float maxHealth = 100f;  
    public float currentHealth;
    public bool isElite = false;
    
    [Header("Scaling")]
    public float speedPerLevel = 0.2f;
    public float healthMultiplier = 0.1f; 
    private float speedModifier = 1f;
    private float currentMoveSpeed;

    [Header("References")]
    private Transform playerTarget;
    private Rigidbody rb;

    [Header("Voxel FX")]
    public int debrisCount = 8;
    public string debrisTag = "Debris";
    public enum EnemyType { Normal, Kamikaze, Ranged }
    
    [Header("Type Settings")]
    public EnemyType enemyType = EnemyType.Normal; 
    public float explosionRange = 3f; 
    public float explosionDamage = 40f; 
    private bool isExploding = false;
    
    [Header("Ranged Settings")]
    public float attackRange = 8f;
    public float fireRate = 2f;   
    public string bulletTag = "EnemyBullet"; 
    Animator myanim;
    private float nextAttackTime = 0f;

    [Header("Attack Settings")]
    public float collisionDamage = 5f; 
    public float damageCooldown = 0.1f; 
    private float lastAttackTime = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        myanim = GetComponentInChildren<Animator>();
        
    }

    void OnEnable()
    {
        
        if (PlayerHealth.Instance != null)
        {
            playerTarget = PlayerHealth.Instance.transform;
        }
        else 
        {
            
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerTarget = p.transform;
        }

        
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        
        isFrozen = false;
        isExploding = false;
        isPoisoned = false;
        isBurning = false;
        speedModifier = 1f;

        int currentLevel = 1;
        if (LevelManager.Instance != null) 
        {
            currentLevel = LevelManager.Instance.currentLevel;
        }

        currentMoveSpeed = baseMoveSpeed + ((currentLevel - 1) * speedPerLevel);
        float difficultyFactor = 1f + ((currentLevel - 1) * healthMultiplier);
        maxHealth = baseHealth * difficultyFactor; 
        currentHealth = maxHealth;
    }

    void FixedUpdate()
    {
        if (isFrozen) return;

        
        
        if (playerTarget == null) 
        {
            if (PlayerHealth.Instance != null) playerTarget = PlayerHealth.Instance.transform;
            return; 
        }

        if (enemyType == EnemyType.Kamikaze && isExploding) return;

        float distance = Vector3.Distance(transform.position, playerTarget.position);

        if (enemyType == EnemyType.Ranged)
        {
            transform.LookAt(playerTarget);

            if (distance <= attackRange)
            {
                if(myanim) myanim.SetBool("Shooting", true);
                rb.linearVelocity = Vector3.zero; 
                
                if (Time.time >= nextAttackTime)
                {
                    ShootAtPlayer();
                    nextAttackTime = Time.time + fireRate;
                }
                return; 
            }
        }
        
        if (enemyType == EnemyType.Kamikaze)
        {
            if (distance <= explosionRange)
            {
                StartCoroutine(ExplodeRoutine());
                return; 
            }
        }

        transform.LookAt(playerTarget); 
        
        if (enemyType == EnemyType.Ranged)
        {
             if(myanim) myanim.SetBool("Shooting", false);
        }

        float finalSpeed = currentMoveSpeed * speedModifier; 
        Vector3 direction = (playerTarget.position - transform.position).normalized;
        
        rb.MovePosition(rb.position + direction * finalSpeed * Time.fixedDeltaTime);
    }
    void OnDisable()
    {
        isExploding = false;
        StopAllCoroutines(); 
        
        if (activePoisonVFX != null) Destroy(activePoisonVFX);
        if (activeFireVFX != null) Destroy(activeFireVFX);
    }
    public void ApplyDoT(float damagePerTick, float duration, string type)
    {
        if (!gameObject.activeInHierarchy) return;
        
        
        if (type == "Poison" && isPoisoned) return;
        if (type == "Fire" && isBurning) return;

        StartCoroutine(DoTRoutine(damagePerTick, duration, type));
    }

    IEnumerator DoTRoutine(float dmg, float duration, string type)
    {
        if (type == "Poison")
        {
            isPoisoned = true;
            if (poisonVFXPrefab != null && activePoisonVFX == null)
                activePoisonVFX = Instantiate(poisonVFXPrefab, transform.position, Quaternion.identity, transform);
        }
        else if (type == "Fire")
        {
            isBurning = true;
            if (fireVFXPrefab != null && activeFireVFX == null)
                activeFireVFX = Instantiate(fireVFXPrefab, transform.position, Quaternion.identity, transform);
        }

        float timer = 0;
        while (timer < duration)
        {
            yield return new WaitForSeconds(1f);
            TakeDamage(dmg, false);
            timer++;
        }

        if (type == "Poison")
        {
            isPoisoned = false;
            if (activePoisonVFX != null) Destroy(activePoisonVFX);
        }
        else if (type == "Fire")
        {
            isBurning = false;
            if (activeFireVFX != null) Destroy(activeFireVFX);
        }
    }

    void ShootAtPlayer()
    {
        Vector3 spawnPos = transform.position + transform.forward * 1f;
        GameObject bulletObj = PoolManager.Instance.SpawnFromPool(bulletTag, spawnPos, Quaternion.identity);
        
        if (bulletObj != null)
        {
            EnemyProjectile projectile = bulletObj.GetComponent<EnemyProjectile>();
            if (projectile != null)
            {
                Vector3 shootDir = (playerTarget.position - transform.position).normalized;
                projectile.Launch(shootDir);
            }
        }
    }

    public void Freeze(float duration)
    {
        if (!gameObject.activeInHierarchy) return;
        StopCoroutine("FreezeRoutine");
        StartCoroutine(FreezeRoutine(duration));
    }

    IEnumerator FreezeRoutine(float duration)
    {
        isFrozen = true;
        
        if(myanim != null) myanim.speed = 0;
        if(rb != null) rb.linearVelocity = Vector3.zero;

        yield return new WaitForSeconds(duration);

        isFrozen = false;
        if(myanim != null) myanim.speed = 1;
    }

    public void ApplyKnockback(Vector3 sourcePos, float force)
    {
        if (rb == null || isFrozen) return;
        Vector3 direction = (transform.position - sourcePos).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

    public void TakeDamage(float amount)
    {
        TakeDamage(amount, false);
    }

    public void TakeDamage(float amount, bool isCrit = false, bool fromExplosion = false)
    {
        if (currentHealth <= 0) return; 

        currentHealth -= amount;
        
        if(DamageTextManager.Instance != null)
            DamageTextManager.Instance.CreatePopup(transform.position + Vector3.up, Mathf.RoundToInt(amount), isCrit);

        if (currentHealth <= 0)
        {
            Die(fromExplosion); 
        }
        else
        {
            StopCoroutine("ApplySlowEffect"); 
            StartCoroutine("ApplySlowEffect");
        }
    }

    IEnumerator ExplodeRoutine()
    {
        isExploding = true;
        rb.linearVelocity = Vector3.zero;
        if(myanim != null) myanim.SetBool("Shooting",true); 

        float timer = 0.5f; 
        while (timer > 0)
        {
            yield return new WaitForSeconds(0.1f);
            timer -= 0.1f;
        }
        Explode();
    }

    void Explode()
    {
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, explosionRange, aoeColliders);
        
        for (int i = 0; i < hitCount; i++)
        {
            Collider hitCollider = aoeColliders[i]; 
            
            if (hitCollider.CompareTag("Player"))
            {
                var playerHealth = hitCollider.GetComponent<PlayerHealth>();
                if (playerHealth != null) playerHealth.TakeDamage(explosionDamage);

                var playerRb = hitCollider.GetComponent<Rigidbody>();
                if (playerRb != null)
                {
                    Vector3 pushDir = (hitCollider.transform.position - transform.position).normalized;
                    playerRb.AddForce(pushDir * 20f, ForceMode.Impulse);
                }
            }
        }
        SpawnDebris(); 
        if(AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SoundType.Explosion);
        if(CameraShake.Instance != null) CameraShake.Instance.Shake(0.3f, 0.5f);
        gameObject.SetActive(false);
    }

    IEnumerator ApplySlowEffect()
    {
        speedModifier = 0.4f;
        yield return new WaitForSeconds(0.15f); 
        speedModifier = 1f; 
    }

    void Die(bool fromExplosion)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(fromExplosion ? SoundType.Explosion : SoundType.EnemyDeath);
        }
        
        if (!fromExplosion && WeaponController.Instance != null && WeaponController.Instance.hasCorpseExplosion)
        {
            float radius = WeaponController.Instance.explosionRadius;
            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, radius, aoeColliders);
            
            for (int i = 0; i < hitCount; i++)
            {
                Collider h = aoeColliders[i];
                if(h.CompareTag("Enemy") && h.gameObject != gameObject)
                {
                    EnemyController enemyScript = h.GetComponent<EnemyController>();
                    if (enemyScript != null)
                    {
                        enemyScript.TakeDamage(WeaponController.Instance.damageMultiplier * 20f, false, true);
                    }
                }
            }
        }

        SpawnDebris();
        
        if(PoolManager.Instance != null)
        {
            PoolManager.Instance.SpawnFromPool("Gem", transform.position, Quaternion.identity);
            
            if (Random.value <= 0.05f)
            {
                Vector3 spawnPos = new Vector3(transform.position.x, 0.5f, transform.position.z);        
                PoolManager.Instance.SpawnFromPool("HealthPotion", spawnPos, Quaternion.identity);
            }
            if (Random.value <= 0.5f) 
            {
                Vector3 spawnPos = new Vector3(transform.position.x, 0.5f, transform.position.z);        
                PoolManager.Instance.SpawnFromPool("GoldCoin", spawnPos, Quaternion.identity);
            }
        }
        
        if (PlayerHealth.Instance != null && PlayerHealth.Instance.vampirismChance > 0)
        {
             PlayerHealth.Instance.OnEnemyKilled();
        }
        
        gameObject.SetActive(false);
    }

    void SpawnDebris()
    {
        for (int i = 0; i < debrisCount; i++)
        {
            Vector3 randomPos = transform.position + Random.insideUnitSphere * 0.5f;
            randomPos.y = 1f;
            
            if(PoolManager.Instance != null)
            {
                GameObject debris = PoolManager.Instance.SpawnFromPool(debrisTag, randomPos, Random.rotation);
                
                if (debris != null)
                {
                    Rigidbody debrisRb = debris.GetComponent<Rigidbody>();
                    if (debrisRb != null)
                    {
                        debrisRb.linearVelocity = Vector3.zero;
                        Vector3 explosionDir = (debris.transform.position - transform.position).normalized;
                        debrisRb.AddForce(explosionDir * 10f + Vector3.up * 5f, ForceMode.Impulse);
                    }
                }
            }
        }
    }
    
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= lastAttackTime + damageCooldown)
            {
                PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
                
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(collisionDamage);
                    lastAttackTime = Time.time; 
                }
            }
        }
    }
}