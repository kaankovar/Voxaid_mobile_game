using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Image iconImage;
    public Button myButton;

    private UpgradeData currentData;

    public void Setup(UpgradeData data)
    {
        currentData = data;

        if(titleText != null) titleText.text = data.GetLocalizedName();
        if(descriptionText != null) descriptionText.text = data.GetLocalizedDescription();
        
        if (data.icon != null)
        {
            iconImage.sprite = data.icon;
            iconImage.gameObject.SetActive(true);
        }
        else if (iconImage != null)
        {
            iconImage.gameObject.SetActive(false);
        }

        myButton.onClick.RemoveAllListeners();
        myButton.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        
        if (UpgradeManager.Instance.upgradePanelCanvasGroup != null)
        {
            UpgradeManager.Instance.upgradePanelCanvasGroup.interactable = false;
        }

        if (currentData.type == UpgradeType.Homing || currentData.type == UpgradeType.Revive|| currentData.type == UpgradeType.InstantNuke)
        {
            AdManager.Instance.ShowRewardedAd(
                onAdComplete: () => 
                {
                    
                    
                    UpgradeManager.Instance.ApplyUpgrade(currentData);
                },
                onAdFailed: () => 
                {
                    
                    if (UpgradeManager.Instance.upgradePanelCanvasGroup != null)
                    {
                        UpgradeManager.Instance.upgradePanelCanvasGroup.interactable = true;
                    }
                    Debug.Log("Reklam iptal edildi, oyuncu başka seçim yapabilir.");
                }
            );
        }
        else
        {
            
            UpgradeManager.Instance.ApplyUpgrade(currentData);
        }
    }
}