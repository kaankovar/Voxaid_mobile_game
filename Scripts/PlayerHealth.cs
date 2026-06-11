using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public static PlayerHealth Instance;
    public TextMeshProUGUI goldtext;
    public int gold;
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth; 
    [Header("Stats")]
    public float maxArmor = 0f;
    [Header("UI Settings")]
    public float smoothSpeed = 5f; 
    public Slider healthSlider; 
    public Slider armorSlider; 
    public GameObject shieldBubbleObj; 
    public GameObject armorIcon; 
    public GameObject extraLifeUIParent; 
    public TextMeshProUGUI extraLifeCounterText; 
    public GameObject reviveVFXPrefab; 

    [Header("Stats")]
    public float armorValue = 0f;
    public int extraLives = 0;
    public float expMultiplier = 1f;
    public float regenRate = 0f; 

    [Header("PERMA Stats")]
    public float dodgeChance = 0f;          
    public float damageReductionPct = 0f;   
    public float thornsDamage = 0f;         
    public float vampirismChance = 0f;      
    public float goldHealAmount = 0f;       
    public float invulnerabilityTime = 1f;  
    public float goldMultiplier = 1f;       

    private float lastDamageTime = 0f;

    void Awake() 
    { 
        Instance = this; 
         if (armorSlider) 
        {
            armorSlider.minValue = 0f;
            armorSlider.maxValue = 1f; 
            armorSlider.value = 0f;
            armorSlider.gameObject.SetActive(false);
        }
    }
    
    void Start() 
    { 
        currentHealth = maxHealth; 
        
        
        if (healthSlider) 
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        
       

        UpdateExtraLifeUI(); 
        CheckVisuals(); 
    }

    void Update()
    {
        
        if (regenRate > 0 && currentHealth < maxHealth && currentHealth > 0)
            Heal(regenRate * Time.deltaTime);

        
        if (healthSlider != null)
        {
            
            healthSlider.maxValue = maxHealth;
            healthSlider.value = Mathf.Lerp(healthSlider.value, currentHealth, Time.deltaTime * smoothSpeed);
        }

        
if (armorSlider != null)
{
    
    if (armorValue > 0 && !armorSlider.gameObject.activeSelf)
    {
        armorSlider.gameObject.SetActive(true);
    }

    
    if (maxArmor > 0)
    {
        armorSlider.maxValue = maxArmor;
    }

    
    float visibleArmor = Mathf.Lerp(armorSlider.value, armorValue, Time.deltaTime * smoothSpeed);
    armorSlider.value = visibleArmor;

    
    if (armorValue <= 0 && visibleArmor <= 0.1f)
    {
        if (armorSlider.gameObject.activeSelf) 
            armorSlider.gameObject.SetActive(false);
            
        maxArmor = 0; 
    }
}
    }

    public void TakeDamage(float amount)
    {
        
        if (Time.time < lastDamageTime + invulnerabilityTime) return;

        
        if (dodgeChance > 0 && Random.value < (dodgeChance / 100f)) return; 

        
        float reducedAmount = amount * (1f - (damageReductionPct / 100f));

        
        if (armorValue > 0)
        {
            armorValue -= reducedAmount;

            if (armorValue < 0) 
            {
                currentHealth -= Mathf.Abs(armorValue); 
                armorValue = 0; 
                AudioManager.Instance.PlaySFX(SoundType.ArmorBreak);
            }
        }
        else
        {
            currentHealth -= reducedAmount;
        }
        
        lastDamageTime = Time.time; 
        
        
        
        CheckVisuals(); 

        
        if (thornsDamage > 0)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, 3f);
            foreach(var h in hits) 
                if(h.CompareTag("Enemy")) h.GetComponent<EnemyController>()?.TakeDamage(thornsDamage, false);
        }

        if (currentHealth <= 0)
        {
            if (extraLives > 0) ReviveRoutine();
            else {healthSlider.value = 0;Die();}
        }else
        {
            AudioManager.Instance.PlaySFX(SoundType.PlayerHurt);
        }
    }

    
   public void ActivateArmor(float amount) 
{ 
    maxArmor = amount; 
    armorValue = amount;

    if (armorSlider != null)
    {
        
        armorSlider.gameObject.SetActive(true); 
        
        
        armorSlider.maxValue = maxArmor;
        armorSlider.value = maxArmor; 
    }

    CheckVisuals(); 
}

    
    public void OnEnemyKilled() { if (vampirismChance > 0 && Random.value < (vampirismChance / 100f)) Heal(maxHealth * 0.05f); }
    public void OnGoldPicked() { if (goldHealAmount > 0) Heal(goldHealAmount); }
    public bool IsArmorActive { get { return armorValue > 0; } }

    public void Heal(float amount) 
    { 
        currentHealth += amount; 
        if(currentHealth > maxHealth) currentHealth = maxHealth; 
    }

    public void IncreaseMaxHealth(float amount) 
    { 
        maxHealth += amount; 
        currentHealth += amount; 
    }

    public void CheckVisuals() 
    { 
        
        if(shieldBubbleObj) shieldBubbleObj.SetActive(armorValue > 0);
        if(armorIcon) armorIcon.SetActive(armorValue > 0); 
        UpdateExtraLifeUI(); 
    }

    void UpdateExtraLifeUI() { if(extraLifeUIParent) extraLifeUIParent.SetActive(extraLives > 0); if(extraLifeCounterText) extraLifeCounterText.text = "x" + extraLives; }
    void ReviveRoutine() {AudioManager.Instance.PlaySFX(SoundType.Revive); extraLives--; UpdateExtraLifeUI(); currentHealth = maxHealth * 0.5f;if(healthSlider) healthSlider.value = currentHealth;lastDamageTime = Time.time + 2f; if(reviveVFXPrefab) Instantiate(reviveVFXPrefab, transform.position, Quaternion.identity); }
    
    void Die() 
    { 
        AudioManager.Instance.PlaySFX(SoundType.PlayerDeath);
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in renderers) r.enabled = false;
        
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        if (GetComponent<Rigidbody>()) GetComponent<Rigidbody>().isKinematic = true;
        
        this.enabled = false; 
        MonoBehaviour pc = GetComponent("PlayerController") as MonoBehaviour;
        if(pc) pc.enabled = false;

        LevelManager.Instance.TriggerGameOver();
    }
    
    public void ReviveFromAd()
    {
        
        currentHealth = maxHealth;
        if(healthSlider) healthSlider.value = currentHealth;

        
        lastDamageTime = Time.time + 2f;

        
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers) r.enabled = true;
        
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = true;

        if (GetComponent<Rigidbody>()) GetComponent<Rigidbody>().isKinematic = false;
        
        this.enabled = true; 
        MonoBehaviour pc = GetComponent("PlayerController") as MonoBehaviour;
        if(pc) pc.enabled = true;

        
        lastDamageTime = Time.time + (3f - invulnerabilityTime);

        Debug.Log("Oyuncu Reklam İzleyerek Dirildi!");
    }
}