using UnityEngine;

[CreateAssetMenu(fileName = "PermaUpgrade", menuName = "Scriptable Objects/PermaUpgrade")]
public class PermaUpgrade : ScriptableObject
{
    [Header("Kayıt Sistemi")]
    [Tooltip("DataManager key'i ile AYNI olmalı. Asla çevrilmeyecek sabit ID.")]
    public string saveID; 

    [Header("Çeviriler (0: EN, 1: TR)")]
    public string[] upgradeNames = new string[2]; 
    
    [TextArea(3, 5)] 
    public string[] descriptions = new string[2]; 

    public Sprite icon;            
    public int maxLevel = 10; 

    [Header("Ekonomi Ayarları")]
    public int baseCost = 100;     
    public float costMultiplier = 1.5f; 

    public int GetCurrentCost(int currentLevel)
    {
        return Mathf.RoundToInt(baseCost * Mathf.Pow(costMultiplier, currentLevel));
    }

    
    public string GetLocalizedName()
    {
        return upgradeNames[LocalizationManager.currentLanguage];
    }

    
    public string GetLocalizedDescription()
    {
        return descriptions[LocalizationManager.currentLanguage];
    }
}