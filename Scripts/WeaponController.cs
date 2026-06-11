using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour
{
    public static WeaponController Instance; 

    [Header("Weapon Settings")]
    public WeaponData currentWeapon; 
    public Transform firePoint;
    public float baseProjectileSize = 0.15f; 
    private float nextFireTime = 0f;

    [Header("Game Stats (Temp Upgrades)")]
    public float damageMultiplier = 1f;
    public float fireRateMultiplier = 1f;
    public int extraBullets = 0;
    public int piercingCount = 0;
    public bool enableRicochet = false;
    public float critChance = 0f;
    public float knockbackForce = 0f;

    [Header("PERMA Stats (New Features)")]
    public float projectileSpeedMult = 1f;    
    public float projectileLifeTimeMult = 1f; 
    public float projectileScaleMult = 1f;    
    public float critDamageMult = 73f;         
    public float spreadReduction = 0f;        
    public float executionThreshold = 0f;     
    public bool canExplodeEnemies = false;    
    public float eliteDamageBonus = 0f;       
    public float lowHealthDamageBonus = 0f;   
    public float explosionRadius = 3f; 
    
    [Header("Special Abilities")]
    public bool hasPoison = false;
    public bool hasChainLightning = false;
    public bool hasFire = false;
    public bool hasSplitShot = false;
    public bool hasHoming = false;
    public bool hasRearShot = false;
    public bool hasCorpseExplosion = false;

    
    public float pistolParallelSpacing = 0.2f; 
    [Range(0f, 1f)] public float damageReductionPerExtraBullet = 0.1f;

    [Header("NEW Mechanics")]
    public bool hasLandmine = false;
    public bool hasOrbital = false;
    private GameObject currentOrbital;
    public GameObject orbitalPrefab;
    public GameObject landminePrefab;

    
    private Rigidbody playerRb;

    void Awake() 
    { 
        Instance = this; 
        playerRb = GetComponent<Rigidbody>(); 
    }

    void Start()
    {
        StartCoroutine(LandmineRoutine());
    }

    IEnumerator LandmineRoutine()
    {
        yield return new WaitUntil(() => hasLandmine);

        while (true)
        {
            yield return new WaitForSeconds(3f);

            if (hasLandmine && landminePrefab != null && playerRb != null)
            {
                
                if (playerRb.linearVelocity.sqrMagnitude > 0.01f)
                {
                    Vector3 spawnPos = transform.position;
                    spawnPos.y = 0.5f;
                    
                    Instantiate(landminePrefab, spawnPos, Quaternion.identity);
                }
            }
        }
    }

    void Update()
    {
        if (currentWeapon == null) return;
        
        float currentFireRate = currentWeapon.fireRate * fireRateMultiplier;
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + (1f / currentFireRate);
        }
        
        if (hasOrbital && currentOrbital == null && orbitalPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.right * 2f;
            currentOrbital = Instantiate(orbitalPrefab, spawnPos, Quaternion.identity);
        }
    }

    void Shoot()
    {
        int totalBullets = currentWeapon.bulletsPerShot + extraBullets;
        float balanceMultiplier = Mathf.Max(0.4f, 1f - (extraBullets * damageReductionPerExtraBullet));
        bool isPrecisionWeapon = currentWeapon.spreadAngle <= 2f; 

        float adrenalineMult = 1f;
        if (PlayerHealth.Instance != null && PlayerHealth.Instance.currentHealth <= PlayerHealth.Instance.maxHealth * 0.3f)
        {
            adrenalineMult = 1f + lowHealthDamageBonus; 
        }
        
        float finalSpread = Mathf.Max(0, currentWeapon.spreadAngle - spreadReduction);

        if (isPrecisionWeapon) 
            SpawnParallelBullets(totalBullets, balanceMultiplier * adrenalineMult);
        else 
            SpawnSpreadBullets(totalBullets, balanceMultiplier * adrenalineMult, finalSpread);
        
        if (hasRearShot)
        {
            Quaternion rearRotation = firePoint.rotation * Quaternion.Euler(0, 180, 0);
            CreateBullet(firePoint.position, rearRotation, balanceMultiplier * adrenalineMult);
        }
        
        AudioManager.Instance.PlaySFX(SoundType.Shoot);
    }

    public void ConfigureBullet(BulletController bullet)
    {
        bullet.isPoison = hasPoison;
        bullet.isFire = hasFire;
        bullet.isSplit = hasSplitShot;
        bullet.isHoming = hasHoming;
        bullet.isChainLightning = hasChainLightning;
    }

    void SpawnParallelBullets(int bulletCount, float dmgMult)
    {
        float startOffset = -((bulletCount - 1) * pistolParallelSpacing) / 2f;
        for (int i = 0; i < bulletCount; i++)
        {
            Vector3 spawnPosition = firePoint.position + (firePoint.right * (startOffset + (i * pistolParallelSpacing)));
            CreateBullet(spawnPosition, firePoint.rotation, dmgMult);
        }
    }

    void SpawnSpreadBullets(int bulletCount, float dmgMult, float spread)
    {
        for (int i = 0; i < bulletCount; i++)
        {
            float randomAngle = Random.Range(-spread, spread);
            Quaternion rotation = Quaternion.Euler(0, firePoint.eulerAngles.y + randomAngle, 0);
            CreateBullet(firePoint.position, rotation, dmgMult);
        }
    }

    void CreateBullet(Vector3 pos, Quaternion rot, float dmgMult)
    {
        GameObject bullet = PoolManager.Instance.SpawnFromPool(currentWeapon.bulletTag, pos, rot);
            
        if(bullet != null)
        {
            bullet.transform.localScale = Vector3.one * baseProjectileSize * projectileScaleMult;

            if (bullet.TryGetComponent(out Rigidbody rb))
            {
                rb.linearVelocity = (rot * Vector3.forward) * (currentWeapon.projectileSpeed * projectileSpeedMult);
            }

            bool isCrit = Random.value < critChance;
            float finalDmg = currentWeapon.damage * damageMultiplier * dmgMult * (isCrit ? critDamageMult : 1f);

            if (bullet.TryGetComponent(out BulletController bs))
            {
                float lifeTime = 2f * projectileLifeTimeMult;
                
                bs.Setup(piercingCount, finalDmg, knockbackForce, isCrit, lifeTime);
                
                bs.executionThreshold = executionThreshold;
                bs.canExplode = canExplodeEnemies;
                bs.eliteBonus = eliteDamageBonus;
                ConfigureBullet(bs);
            }
        }
    }

    public void ChangeWeapon(WeaponData newWeapon) 
    { 
        currentWeapon = newWeapon; 
        nextFireTime = Time.time; 
    }
}