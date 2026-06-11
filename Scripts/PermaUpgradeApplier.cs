using UnityEngine;

public class PermaUpgradeApplier : MonoBehaviour
{
    [Header("References")]
    public PlayerController playerMovement;
    public PlayerHealth playerHealth;
    public WeaponController weaponController;
    public UpgradeManager upgradeManager;

    void Start()
    {
        
        ApplyStrength();     
        ApplyHaste();        
        ApplyLethality();    
        ApplyImpact();       
        ApplyBarrage();      
        ApplyDrill();        
        ApplyBounce();       
        ApplyVelocity();     
        ApplyExtension();    
        ApplyGambler();      
        ApplyGiantism();     
        ApplyStability();    
        ApplyExecutioner();  
        ApplyDetonation();   
        ApplyHeadhunter();   

        
        ApplyVitality();     
        ApplyResilience();   
        ApplyAgility();      
        ApplyHardy();        
        ApplyEvasion();      
        ApplyGhost();        
        ApplyThorns();       
        ApplyRegeneration(); 
        ApplyVampirism();    
        ApplyScavenger();    
        ApplySecondChance(); 

        
        ApplyGreed();        
        ApplyIntelligence(); 
        ApplyLogistics();    
        ApplyMagnetism();    
        ApplyPrestigeBonus();
        
    }
    void ApplyPrestigeBonus()
    {
        float mult = DataManager.GetPrestigeMultiplier();
        
        
        if(weaponController != null) 
            weaponController.damageMultiplier *= mult;

        
        if(playerHealth != null)
        {
            playerHealth.goldMultiplier *= mult;
            playerHealth.expMultiplier *= mult;
        }    
        Debug.Log($"Prestij Bonusu Uygulandı: x{mult} daha güçlüsün!");
    }

    
    
    
    void ApplyStrength() { if(weaponController) weaponController.damageMultiplier += DataManager.GetUpgradeLevel("Strength") * 0.1f; } 
    void ApplyHaste() { if(weaponController) weaponController.fireRateMultiplier += DataManager.GetUpgradeLevel("Haste") * 0.1f; } 
    void ApplyLethality() { if(weaponController) weaponController.critChance += DataManager.GetUpgradeLevel("Lethality") * 0.05f; } 
    void ApplyImpact() { if(weaponController) weaponController.knockbackForce += DataManager.GetUpgradeLevel("Impact") * 1f; } 
    void ApplyBarrage() { if(weaponController) weaponController.extraBullets += DataManager.GetUpgradeLevel("Barrage"); } 
    void ApplyDrill() { if(weaponController) weaponController.piercingCount += DataManager.GetUpgradeLevel("Drill"); } 
    void ApplyBounce() { if(weaponController && DataManager.GetUpgradeLevel("Bounce") > 0) weaponController.enableRicochet = true; } 
    void ApplyVelocity() { if(weaponController) weaponController.projectileSpeedMult += DataManager.GetUpgradeLevel("Velocity") * 0.10f; } 
    void ApplyExtension() { if(weaponController) weaponController.projectileLifeTimeMult += DataManager.GetUpgradeLevel("Extension") * 0.15f; } 
    void ApplyGambler() { if(weaponController) weaponController.critDamageMult += DataManager.GetUpgradeLevel("Gambler") * 0.5f; } 
    void ApplyGiantism() { if(weaponController) weaponController.projectileScaleMult += DataManager.GetUpgradeLevel("Giantism") * 0.15f; } 
    void ApplyStability() { if(weaponController) weaponController.spreadReduction += DataManager.GetUpgradeLevel("Stability") * 2f; } 
    void ApplyExecutioner() { if(weaponController) weaponController.executionThreshold += DataManager.GetUpgradeLevel("Executioner") * 0.05f; } 
    void ApplyDetonation() 
    { 
        int level = DataManager.GetUpgradeLevel("Detonation");
        if(weaponController && level > 0) 
        {
            weaponController.canExplodeEnemies = true;
            weaponController.explosionRadius = 3f + (level * 0.5f); 
        }
    }   
    void ApplyHeadhunter() { if(weaponController) weaponController.eliteDamageBonus += DataManager.GetUpgradeLevel("Headhunter") * 0.15f; } 

    
    
    
    void ApplyVitality() 
    { 
        if(playerHealth) 
        {
            float extraHP = DataManager.GetUpgradeLevel("Vitality") * 20f; 
            if(extraHP > 0) playerHealth.IncreaseMaxHealth(extraHP);
        }
    }
    void ApplyResilience() 
    { 
        if(playerHealth) 
        {
            float extraArmor = DataManager.GetUpgradeLevel("Resilience") * 25f; 
            if(extraArmor > 0) playerHealth.ActivateArmor(extraArmor);
        }
    }
    void ApplyAgility() { if(playerMovement) playerMovement.moveSpeed += DataManager.GetUpgradeLevel("Agility") * 0.5f; } 
    void ApplyHardy() { if(playerHealth) playerHealth.damageReductionPct += DataManager.GetUpgradeLevel("Hardy") * 2f; } 
    void ApplyEvasion() { if(playerHealth) playerHealth.dodgeChance += DataManager.GetUpgradeLevel("Evasion") * 3f; } 
    void ApplyGhost() { if(playerHealth) playerHealth.invulnerabilityTime += DataManager.GetUpgradeLevel("Ghost") * 0.2f; } 
    void ApplyThorns() { if(playerHealth) playerHealth.thornsDamage += DataManager.GetUpgradeLevel("Thorns") * 5f; } 
    void ApplyRegeneration() { if(playerHealth) playerHealth.regenRate += DataManager.GetUpgradeLevel("Regeneration") * 0.5f; } 
    void ApplyVampirism() { if(playerHealth) playerHealth.vampirismChance += DataManager.GetUpgradeLevel("Vampirism") * 2f; } 
    void ApplyScavenger() { if(playerHealth) playerHealth.goldHealAmount += DataManager.GetUpgradeLevel("Scavenger") * 1f; } 
    void ApplySecondChance() 
    { 
        if(playerHealth) 
        {
            int level = DataManager.GetUpgradeLevel("SecondChance");
            if(level > 0) 
            {
                playerHealth.extraLives += level; 
                playerHealth.CheckVisuals(); 
            }
        }
    }

    
    
    
    void ApplyGreed() { if(playerHealth) playerHealth.goldMultiplier += DataManager.GetUpgradeLevel("Greed") * 0.10f; } 
    void ApplyIntelligence() { if(playerHealth) playerHealth.expMultiplier += DataManager.GetUpgradeLevel("Intelligence") * 0.10f; } 
    void ApplyLogistics() { if(upgradeManager) upgradeManager.rerollCount += DataManager.GetUpgradeLevel("Logistics"); } 
    void ApplyMagnetism() 
    { 
        if(playerMovement) 
        {
            SphereCollider col = playerMovement.GetComponent<SphereCollider>();
            
            if (col != null) col.radius += DataManager.GetUpgradeLevel("Magnetism") * 0.5f; 
        }
    }
}