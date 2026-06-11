using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class DailyAdGoldManager : MonoBehaviour
{
    [Header("UI References")]
    public Button adGoldButton;           
    public TextMeshProUGUI statusText;    
    public CanvasGroup panelCanvasGroup;  

    [Header("Settings")]
    public int goldAmount = 3000;         

    private DateTime lastClaimTime;
    private bool isReady = false;

    void Start()
    {
        lastClaimTime = DataManager.GetLastAdGoldTime();
        CheckStatus();
    }

    void Update()
    {
        if (!isReady)
        {
            UpdateTimer();
        }
    }
    void OnEnable()
    {
        
        CheckStatus(); 
    }
    void CheckStatus()
    {
        TimeSpan diff = DateTime.Now - lastClaimTime;

        if (diff.TotalHours >= 24)
        {
            isReady = true;
            if(adGoldButton) adGoldButton.interactable = true;
            
            
            if(statusText) 
            {
                string template = LocalizationManager.GetText("watch_ad_gold");
                statusText.text = string.Format(template, goldAmount);
            }
        }
        else
        {
            isReady = false;
            if(adGoldButton) adGoldButton.interactable = false;
        }
    }

    void UpdateTimer()
    {
        DateTime targetTime = lastClaimTime.AddHours(24);
        TimeSpan remaining = targetTime - DateTime.Now;

        if (remaining.TotalSeconds > 0)
        {
            
            if(statusText) 
                statusText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", remaining.Hours, remaining.Minutes, remaining.Seconds);
        }
        else
        {
            CheckStatus(); 
        }
    }

    public void WatchAdForGold()
    {
        if (!isReady) return;

        if(adGoldButton) adGoldButton.interactable = false;
        if(panelCanvasGroup) panelCanvasGroup.interactable = false;

        AdManager.Instance.ShowRewardedAd(
            onAdComplete: () => {
                DataManager.AddGold(goldAmount);
                DataManager.SetLastAdGoldTime(DateTime.Now);
                lastClaimTime = DateTime.Now;
                
                if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SoundType.LevelUp);
                RewardAnimationManager.Instance.PlayGoldRewardAnimation(3000);
                CheckStatus(); 
                
                if(panelCanvasGroup) panelCanvasGroup.interactable = true;
            },
            onAdFailed: () => {
                Debug.Log("Günlük altın reklamı iptal edildi.");
                if(adGoldButton) adGoldButton.interactable = true;
                if(panelCanvasGroup) panelCanvasGroup.interactable = true;
            }
        );
    }
}