using UnityEngine;

public enum UpgradeType
{
    PoisonAmmo,
    FireAmmo,
    ChainLightning,
    SplitShot,
    RearShot,
    Homing,
    OrbitalBlade,
    Landmine,
    Thorns,
    Dodge,
    CorpseExplosion,
    GlassCannon,
    MoveSpeed,
    MaxHealth,
    Heal,
    MagnetRange,
    Damage,
    FireRate,
    Multishot,
    Piercing,
    Ricochet,
    NewWeapon,
    CriticalChance,
    ProjectileSize,
    Knockback,
    Armor,
    Revive,
    ExpGain,
    InstantNuke,
    InstantFreeze
}

[CreateAssetMenu(menuName = "Weapon System/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    [Header("Çeviriler (0: EN, 1: TR)")]
    public string[] upgradeNames = new string[2]; 
    
    [TextArea(3, 5)] 
    public string[] descriptions = new string[2];

    public Sprite icon; 
    
    public UpgradeType type;
    public float value; 

    [Header("Only for NewWeapon Type")]
    public WeaponData weaponData;

    
    public string GetLocalizedName()
    {
        return upgradeNames[LocalizationManager.currentLanguage];
    }

    
    public string GetLocalizedDescription()
    {
        return descriptions[LocalizationManager.currentLanguage];
    }
}