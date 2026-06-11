using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSlotUI : MonoBehaviour
{
    [Header("Veri")]
    public PermaUpgrade upgradeData; 

    [Header("Görsel Referanslar")]
    public TextMeshProUGUI titleText; 
    public Image img;
    

    private Button myButton;

    void OnEnable()
    {
        myButton = GetComponent<Button>();
        
        
        if (upgradeData != null)
        {
            
            titleText.text = upgradeData.GetLocalizedName();
            img.sprite = upgradeData.icon;
        }

        
        myButton.onClick.AddListener(OnSlotClicked);
    }
    void OnSlotClicked()
    {
        
        ShopPopupManager.Instance.OpenPopup(upgradeData);
    }
    
}