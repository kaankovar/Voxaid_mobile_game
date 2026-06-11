using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopPopupManager : MonoBehaviour
{
    public static ShopPopupManager Instance; 

    [Header("UI Bileşenleri")]
    public GameObject popupPanel;          
    public TextMeshProUGUI titleText; 
    public TextMeshProUGUI descText;  
    public TextMeshProUGUI levelText; 
    public TextMeshProUGUI priceText;
    public List<TextMeshProUGUI>  gold; 
    public Button buyButton;          
    
    private PermaUpgrade currentUpgrade; 

    void Awake()
    {
        Instance = this;
        ClosePopup(); 
    }

    
    public void OpenPopup(PermaUpgrade upgrade)
    {
        currentUpgrade = upgrade;
        UpdateUI();
        popupPanel.SetActive(true);
        
        
        
    }

    public void ClosePopup()
    {
        popupPanel.SetActive(false);
    }

    private void UpdateUI()
    {
        if (currentUpgrade == null) return;

        int currentLvl = DataManager.GetUpgradeLevel(currentUpgrade.saveID);
        int playerGold = DataManager.GetGold();

        titleText.text = currentUpgrade.GetLocalizedName();
        descText.text = currentUpgrade.GetLocalizedDescription();
        gold[0].text = playerGold.ToString();
        gold[1].text = playerGold.ToString();

        
        if (currentLvl >= currentUpgrade.maxLevel) 
        {
            levelText.text = "MAX LEVEL";
            priceText.text = "MAX";
            priceText.color = Color.yellow;
            buyButton.gameObject.SetActive(false); 
        }
        else 
        {
            levelText.text = "Level: " + currentLvl + " / " + currentUpgrade.maxLevel;
            buyButton.gameObject.SetActive(true); 
            
            int cost = currentUpgrade.GetCurrentCost(currentLvl);
            priceText.text = cost.ToString();

            if (playerGold >= cost) { buyButton.interactable = true; priceText.color = Color.white; }
            else { buyButton.interactable = false; priceText.color = Color.red; }
        }
    }

    public void BuyUpgrade()
    {
        if (currentUpgrade == null) return;

        int currentLvl = DataManager.GetUpgradeLevel(currentUpgrade.saveID);
        
        if (currentLvl >= currentUpgrade.maxLevel) return;

        int cost = currentUpgrade.GetCurrentCost(currentLvl);

        if (DataManager.SpendGold(cost)) 
        {
            DataManager.IncreaseUpgradeLevel(currentUpgrade.saveID);
            UpdateUI(); 
            
            Debug.Log("Satın alma başarılı: " + currentUpgrade.GetLocalizedName());
            AudioManager.Instance.PlaySFX(SoundType.Buying);
        }
        else
        {
            Debug.Log("Para yetersiz!");
            AudioManager.Instance.PlaySFX(SoundType.NotEnoughMoney);
        }
    }
}