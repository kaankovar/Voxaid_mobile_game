using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BossController : MonoBehaviour, IDamageable
{
    [Header("Boss Stats")]
    private bool isSpawningPhase = true;
    private Vector3 originalScale;
    public float maxHealth = 5000f;
    private float currentHealth;
    public float rotationSpeed = 2f;
    public float moveSpeed = 1f;

    [Header("Attack Settings")]
    public float attackRange = 20f;
    public float attackCooldown = 6f;
    public float warningDuration = 2f;
    public float laserDuration = 1.5f;

    [Header("Visuals")]
    public GameObject warningVisual;
    public GameObject damageZone;


    private Slider bossHealthBar;
    private Animator myanim;
    private Transform player;
    private Rigidbody rb;
    private bool isAttacking = false;
    private bool isDead = false;

    void Awake()
    {

        if (LevelManager.Instance != null && LevelManager.Instance.currentPlanet != null)
        {
            float hpMult = LevelManager.Instance.currentPlanet.enemyHealthMultiplier;
            if (hpMult < 1) hpMult = 1;
            maxHealth *= hpMult;
            attackCooldown = Mathf.Max(1.5f, attackCooldown - (LevelManager.Instance.currentPlanet.planetIndex * 0.25f));
        }

        myanim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();


        currentHealth = maxHealth;
    }

    void Start()
    {

        if (LevelManager.Instance != null && LevelManager.Instance.bossHealthBar != null)
        {
            bossHealthBar = LevelManager.Instance.bossHealthBar;
            bossHealthBar.gameObject.SetActive(true);
            bossHealthBar.maxValue = maxHealth;
            bossHealthBar.value = currentHealth;
        }

        GameObject pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj != null) player = pObj.transform;

        if (warningVisual) warningVisual.SetActive(false);
        if (damageZone) damageZone.SetActive(false);
        originalScale = transform.localScale;
        StartCoroutine(SpawnSequence());
    }
    IEnumerator SpawnSequence()
    {
        isSpawningPhase = true;


        if (myanim != null) myanim.speed = 0f;
        if (damageZone) damageZone.SetActive(false);

        float spawnDuration = 3f;
        float timer = 0f;


        while (timer < spawnDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / spawnDuration;




            transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, progress);
            yield return null;
        }


        transform.localScale = originalScale;
        isSpawningPhase = false;
        if (myanim != null) myanim.speed = 1f;


        StartCoroutine(AttackRoutine());
    }


    void FixedUpdate()
    {
        if (player == null || isDead || isAttacking || isSpawningPhase)
        {

            if (rb != null) rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }


        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > 0.1f)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;


            rb.linearVelocity = new Vector3(direction.x * moveSpeed, rb.linearVelocity.y, direction.z * moveSpeed);
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }


    void Update()
    {
        if (player == null || isDead || isAttacking || isSpawningPhase) return;


        Vector3 direction = player.position - transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);
        }
    }


    void OnCollisionStay(Collision other)
    {
        if (isSpawningPhase) return;
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(50f);
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            AudioManager.Instance.PlaySFX(SoundType.Explosion);
            for (int i = 0; i < 30; i++)
            {
                Vector3 randomPos = other.gameObject.transform.position + Random.insideUnitSphere * 0.5f;
                randomPos.y = 1f;

                if (PoolManager.Instance != null)
                {
                    GameObject debris = PoolManager.Instance.SpawnFromPool("Debris", randomPos, Random.rotation);

                    if (debris != null)
                    {
                        Rigidbody debrisRb = debris.GetComponent<Rigidbody>();
                        if (debrisRb != null)
                        {
                            debrisRb.linearVelocity = Vector3.zero;
                            Vector3 explosionDir = (debris.transform.position - other.gameObject.transform.position).normalized;
                            debrisRb.AddForce(explosionDir * 10f + Vector3.up * 5f, ForceMode.Impulse);
                        }
                    }
                }
            }
            other.gameObject.SetActive(false);
        }
    }

    IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(3f);

        Renderer warningRenderer = null;
        if (warningVisual != null)
        {
            warningRenderer = warningVisual.GetComponent<Renderer>();
        }

        while (!isDead && player != null)
        {

            isAttacking = true;
            myanim.SetTrigger("shoot");
            AudioManager.Instance.PlaySFX(SoundType.BossWarning);

            if (warningVisual)
            {
                warningVisual.SetActive(true);
                if (warningRenderer != null)
                    warningRenderer.material.color = new Color(1f, 1f, 0f, 0.5f);
            }

            yield return new WaitForSeconds(warningDuration);


            if (warningVisual && warningRenderer != null)
            {
                warningRenderer.material.color = new Color(1f, 0f, 0f, 0.5f);
            }

            if (damageZone) damageZone.SetActive(true);
            AudioManager.Instance.PlaySFX(SoundType.BossLaser);
            yield return new WaitForSeconds(laserDuration);


            if (warningVisual) warningVisual.SetActive(false);
            if (damageZone) damageZone.SetActive(false);
            isAttacking = false;

            yield return new WaitForSeconds(attackCooldown);
        }
    }

    public void TakeDamage(float amount) { TakeDamage(amount, false); }

    public void TakeDamage(float amount, bool isCrit)
    {
        if (isDead || isSpawningPhase) return;

        currentHealth -= amount;

        if (bossHealthBar) bossHealthBar.value = currentHealth;

        if (DamageTextManager.Instance != null)
            DamageTextManager.Instance.CreatePopup(transform.position + Vector3.up * 5, Mathf.RoundToInt(amount), isCrit);

        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        if (isDead) return;
        AudioManager.Instance.PlaySFX(SoundType.Explosion);
        isDead = true;
        StopAllCoroutines();

        if (bossHealthBar) bossHealthBar.gameObject.SetActive(false);

        if (LevelManager.Instance != null)
            LevelManager.Instance.PlanetComplete();

        Destroy(gameObject);
    }
}