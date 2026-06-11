using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [Header("VFX Settings")]
    public GameObject levelUpVFXPrefab;
    public GameObject pausebutton; 
    public Transform playerTransform; 

    [Header("UI References")]
    public GameObject upgradePanel; 
    public CanvasGroup upgradePanelCanvasGroup; 
    public Transform cardsContainer;  
    
    [Header("Reroll UI Settings")]
    public Button rerollButton;
    public TextMeshProUGUI rerollText;
    public Image rerollButtonIcon;       
    public Sprite freeRerollSprite;      
    public Sprite adRerollSprite;        

    [Header("Game References")]
    public PlayerController playerMovement;
    public PlayerHealth playerHealth;
    public WeaponController weaponController;

    [Header("Data")]
    public List<UpgradeData> allUpgrades; 
    public int rerollCount = 0; 

    
    private List<UpgradeData> currentShownCards = new List<UpgradeData>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (upgradePanel != null) upgradePanel.SetActive(false);
    }

    
    public void Reroll()
    {
        if(rerollCount > 0)
        {
            
            rerollCount--;
            UpdateRerollUI(); 
            PerformReroll();  
        }
        else
        {
            
            
            if (upgradePanelCanvasGroup != null) upgradePanelCanvasGroup.interactable = false;

            AdManager.Instance.ShowRewardedAd(
                onAdComplete: () => 
                {
                    
                    PerformReroll();
                    if (upgradePanelCanvasGroup != null) upgradePanelCanvasGroup.interactable = true;
                },
                onAdFailed: () => 
                {
                    
                    Debug.Log("Reroll için reklam izlenmedi veya iptal edildi.");
                    if (upgradePanelCanvasGroup != null) upgradePanelCanvasGroup.interactable = true;
                }
            );
        }
    }

    private void PerformReroll()
    {
        ShowUpgradeOptions(); 
        AudioManager.Instance.PlaySFX(SoundType.Reroll);
    }

    
    private void UpdateRerollUI()
    {
        if(rerollText != null) rerollText.text = rerollCount.ToString();

        if (rerollButtonIcon != null)
        {
            if (rerollCount > 0)
            {
                if (freeRerollSprite != null) rerollButtonIcon.sprite = freeRerollSprite;
            }
            else
            {
                if (adRerollSprite != null) rerollButtonIcon.sprite = adRerollSprite;
            }
        }
    }

    
    public void ShowUpgradeOptions()
    {
        Time.timeScale = 0; 
        if (upgradePanel != null) upgradePanel.SetActive(true);
        if (CameraShake.Instance != null) CameraShake.Instance.StopShake();
        pausebutton.SetActive(false);

        
        if (upgradePanelCanvasGroup != null) upgradePanelCanvasGroup.interactable = true;

        List<UpgradeData> validCards = GetValidUpgrades();
        
        
        List<UpgradeData> poolForSelection = new List<UpgradeData>(validCards);

        
        if (poolForSelection.Count - currentShownCards.Count >= 3)
        {
            foreach (var card in currentShownCards)
            {
                poolForSelection.Remove(card);
            }
        }

        List<UpgradeData> selectedCards = PickUniqueRandomCards(poolForSelection, 3);
        currentShownCards = new List<UpgradeData>(selectedCards); 
        

        UpgradeButton[] availableButtons = cardsContainer.GetComponentsInChildren<UpgradeButton>(true);

        for (int i = 0; i < availableButtons.Length; i++)
        {
            if (i < selectedCards.Count)
            {
                availableButtons[i].gameObject.SetActive(true);
                availableButtons[i].Setup(selectedCards[i]);
            }
            else
            {
                availableButtons[i].gameObject.SetActive(false);
            }
        }

        
        UpdateRerollUI();
    }

   List<UpgradeData> GetValidUpgrades()
    {
        List<UpgradeData> filteredList = new List<UpgradeData>();

        foreach (var card in allUpgrades)
        {
            
            if (card.type == UpgradeType.NewWeapon)
            {
                if (weaponController.currentWeapon == card.weaponData) continue; 
            }

            
            if (card.type == UpgradeType.Armor && playerHealth.IsArmorActive)
            {
                continue;
            }

            
            if (card.type == UpgradeType.Ricochet && weaponController.enableRicochet)
            {
                continue; 
            }

            filteredList.Add(card);
        }
        return filteredList;
    }

    List<UpgradeData> PickUniqueRandomCards(List<UpgradeData> sourceList, int count)
    {
        List<UpgradeData> uniquePicks = new List<UpgradeData>();
        List<UpgradeData> tempPool = new List<UpgradeData>(sourceList);

        int iterations = Mathf.Min(count, tempPool.Count);

        for (int i = 0; i < iterations; i++)
        {
            int randomIndex = Random.Range(0, tempPool.Count);
            uniquePicks.Add(tempPool[randomIndex]);
            tempPool.RemoveAt(randomIndex);
        }
        return uniquePicks;
    }

    public void ApplyUpgrade(UpgradeData data)
    {
        if (IsOneTimeUpgrade(data.type) && allUpgrades.Contains(data))
        {
            allUpgrades.Remove(data);
        }
        switch (data.type)
        {
            case UpgradeType.MoveSpeed:
                playerMovement.moveSpeed += data.value;
                break;

            case UpgradeType.MaxHealth:
                playerHealth.IncreaseMaxHealth(data.value);
                break;
            
            case UpgradeType.Heal:
                playerHealth.Heal(data.value);
                break;

            case UpgradeType.MagnetRange:
                SphereCollider col = playerMovement.GetComponent<SphereCollider>();
                if(col != null) col.radius += data.value;
                break;

            case UpgradeType.Damage:
                weaponController.damageMultiplier += data.value;
                break;

            case UpgradeType.FireRate:
                 weaponController.fireRateMultiplier += data.value;
                break;

            case UpgradeType.Multishot:
                weaponController.extraBullets += (int)data.value;
                break;

            case UpgradeType.Piercing:
                weaponController.piercingCount += (int)data.value;
                break;

            case UpgradeType.Ricochet:
                weaponController.enableRicochet = true;
                break;

            case UpgradeType.NewWeapon:
                weaponController.ChangeWeapon(data.weaponData);
                break;
            
            case UpgradeType.CriticalChance:
                weaponController.critChance += data.value;
                break;

            case UpgradeType.ProjectileSize:
                weaponController.projectileScaleMult += data.value;
                break;

            case UpgradeType.Knockback:
                weaponController.knockbackForce += data.value;
                break;

            case UpgradeType.Armor:
                
                playerHealth.ActivateArmor(data.value); 
                break;

            case UpgradeType.Revive:
                playerHealth.extraLives += (int)data.value;
                playerHealth.CheckVisuals();
                break;

            case UpgradeType.ExpGain:
                playerHealth.expMultiplier += data.value;
                break;

            case UpgradeType.InstantNuke:
                TriggerNuke();
                break;

            case UpgradeType.InstantFreeze:
                TriggerFreeze();
                break;
            case UpgradeType.PoisonAmmo:
            weaponController.hasPoison = true;
            break;
            case UpgradeType.FireAmmo:
                weaponController.hasFire = true;
                break;
            case UpgradeType.RearShot:
                weaponController.hasRearShot = true;
                break;
            case UpgradeType.SplitShot:
                weaponController.hasSplitShot = true;
                break;
            case UpgradeType.Homing:
                weaponController.hasHoming = true;
                break;
            case UpgradeType.CorpseExplosion:
                weaponController.hasCorpseExplosion = true;
                break;
            
            
            case UpgradeType.OrbitalBlade:
                weaponController.hasOrbital = true; 
                
                break;

            
            case UpgradeType.GlassCannon:
                weaponController.damageMultiplier += 0.5f; 
                playerHealth.maxHealth *= 0.7f; 
                playerHealth.currentHealth = Mathf.Min(playerHealth.currentHealth, playerHealth.maxHealth);
                break;
            case UpgradeType.Landmine:
                weaponController.hasLandmine = true;
                break;
            case UpgradeType.ChainLightning:
                weaponController.hasChainLightning = true;
                break;

            case UpgradeType.Thorns:
                
                playerHealth.thornsDamage += data.value;
                break;

            case UpgradeType.Dodge:
                
                playerHealth.dodgeChance += data.value;
                break;
        }

        
        currentShownCards.Clear();

        upgradePanel.SetActive(false);
        pausebutton.SetActive(true);
        Time.timeScale = 1;
        AudioManager.Instance.PlaySFX(SoundType.LevelUp);
        PlayLevelUpEffect();
    }

    private bool IsOneTimeUpgrade(UpgradeType type)
    {
        switch (type)
        {
            
            case UpgradeType.NewWeapon:
            case UpgradeType.Ricochet:
            case UpgradeType.PoisonAmmo:
            case UpgradeType.FireAmmo:
            case UpgradeType.RearShot:
            case UpgradeType.SplitShot:
            case UpgradeType.Homing:
            case UpgradeType.CorpseExplosion:
            case UpgradeType.OrbitalBlade:
            case UpgradeType.GlassCannon:
            case UpgradeType.Landmine:
            case UpgradeType.ChainLightning:
                return true; 

            
            default:
                return false; 
        }
    }

    void TriggerNuke()
    {
        AudioManager.Instance.PlaySFX(SoundType.Explosion);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(var obj in enemies)
        {
            var enemy = obj.GetComponent<EnemyController>();
            if(enemy != null)
            {
                enemy.TakeDamage(99999, true);
            }
        }
        if(CameraShake.Instance != null) CameraShake.Instance.Shake(1f, 1f);
    }

    void TriggerFreeze()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        AudioManager.Instance.PlaySFX(SoundType.Freeze);
        foreach(var obj in enemies)
        {
            var enemy = obj.GetComponent<EnemyController>();
            if(enemy != null)
            {
                enemy.Freeze(5f);
            }
        }
    }

    void PlayLevelUpEffect()
    {
        if (levelUpVFXPrefab != null && playerTransform != null)
        {
            Vector3 pos = new Vector3(playerTransform.position.x,playerTransform.position.y - 1f,playerTransform.position.z);
            GameObject vfx = Instantiate(levelUpVFXPrefab, pos, Quaternion.identity);
            vfx.transform.SetParent(playerTransform); 
        }
    }
    
}