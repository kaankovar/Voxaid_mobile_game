using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class DailyRewardManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dailyRewardPanel; 
    public CanvasGroup panelCanvasGroup; 
    public Button claimButton;          
    public Button doubleClaimButton;    
    
    public TextMeshProUGUI statusText;  
    public TextMeshProUGUI rewardAmountText; 
    public GameObject mainPanel;

    [Header("Day Visuals")]
    public GameObject[] dayBorders; 
    
    [Header("Lock Visuals")]
    public Image[] dayImages;       
    public GameObject[] lockIcons;  
    public Color lockedColor = new Color(0.5f, 0.5f, 0.5f, 0.7f); 
    public Color unlockedColor = Color.white;                    

    [Header("Settings")]
    public int[] goldRewards = { 100, 200, 350, 500, 750, 1000, 2000 };

    private int currentStreak;
    private DateTime lastClaimTime;
    private bool isRewardReady = false;

    void Start()
    {
        LoadData();
        CheckRewardStatus();
        UpdateUI();
    }

    void Update()
    {
        if (!isRewardReady)
        {
            UpdateTimerDisplay();
        }
    }

    void LoadData()
    {
        lastClaimTime = DataManager.GetLastRewardTime();
        currentStreak = DataManager.GetDailyStreak();
    }

    void CheckRewardStatus()
    {
        DateTime now = DateTime.Now;
        TimeSpan diff = now - lastClaimTime;

        if (diff.TotalHours < 24)
        {
            isRewardReady = false;
            SetButtonsInteractable(false);
        }
        else if (diff.TotalHours >= 24 && diff.TotalHours < 48)
        {
            isRewardReady = true;
            SetButtonsInteractable(true);
            
            
            statusText.text = LocalizationManager.GetText("daily_reward_status");
        }
        else
        {
            isRewardReady = true;
            SetButtonsInteractable(true);
            currentStreak = 0; 
            
            
            statusText.text = LocalizationManager.GetText("daily_reward_status");
        }
        
        if (currentStreak >= 7) currentStreak = 0;
    }

    void SetButtonsInteractable(bool state)
    {
        if(claimButton != null) claimButton.interactable = state;
        if(doubleClaimButton != null) doubleClaimButton.interactable = state;
    }

    void UpdateTimerDisplay()
    {
        DateTime now = DateTime.Now;
        DateTime targetTime = lastClaimTime.AddHours(24);
        TimeSpan remaining = targetTime - now;

        if (remaining.TotalSeconds > 0)
        {
            statusText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", 
                remaining.Hours, remaining.Minutes, remaining.Seconds);
        }
        else
        {
            CheckRewardStatus(); 
        }
    }
    void OnEnable()
    {
        
        CheckRewardStatus(); 
        UpdateUI();          
    }
    public void ClaimRewardNormal()
    {
        ProcessReward(1); 
    }

    public void ClaimRewardDouble()
    {
        if (!isRewardReady) return;

        SetButtonsInteractable(false);
        if (panelCanvasGroup != null) panelCanvasGroup.interactable = false;

        AdManager.Instance.ShowRewardedAd(
            onAdComplete: () => {
                ProcessReward(2); 
                if (panelCanvasGroup != null) panelCanvasGroup.interactable = true;
            },
            onAdFailed: () => {
                ProcessReward(1); 
                if (panelCanvasGroup != null) panelCanvasGroup.interactable = true;
            }
        );
    }

    private void ProcessReward(int multiplier)
    {
        if (!isRewardReady) return;

        SetButtonsInteractable(false);
        isRewardReady = false;

        int baseReward = goldRewards[currentStreak];
        int finalReward = baseReward * multiplier;

        DataManager.AddGold(finalReward);
        
        if(AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(SoundType.LevelUp);

        lastClaimTime = DateTime.Now; 
        DataManager.SetLastRewardTime(lastClaimTime);
        
        currentStreak++;
        if(currentStreak >= 7) currentStreak = 0;
        DataManager.SetDailyStreak(currentStreak);

        CheckRewardStatus();
        UpdateUI();
        UpdateTimerDisplay();
    }

    void UpdateUI()
    {
        int displayStreak = currentStreak;
        if (displayStreak >= 7) displayStreak = 0;
        
        
        string template = LocalizationManager.GetText("daily_reward_amount");
        rewardAmountText.text = string.Format(template, goldRewards[displayStreak]);

        for(int i = 0; i < dayBorders.Length; i++)
        {
            if(dayBorders[i] != null)
            {
                if (i == displayStreak)
                {
                    dayBorders[i].transform.localScale = Vector3.one * 1.2f;
                    if (i < dayImages.Length && dayImages[i] != null) dayImages[i].color = unlockedColor;
                    if (i < lockIcons.Length && lockIcons[i] != null) lockIcons[i].SetActive(false); 
                }
                else if (i < displayStreak)
                {
                    dayBorders[i].transform.localScale = Vector3.one;
                    if (i < dayImages.Length && dayImages[i] != null) dayImages[i].color = lockedColor; 
                    if (i < lockIcons.Length && lockIcons[i] != null) lockIcons[i].SetActive(false); 
                }
                else
                {
                    dayBorders[i].transform.localScale = Vector3.one;
                    if (i < dayImages.Length && dayImages[i] != null) dayImages[i].color = lockedColor;
                    if (i < lockIcons.Length && lockIcons[i] != null) lockIcons[i].SetActive(true); 
                }
            }
        }
    }

    public void OpenPanel() 
    { 
        dailyRewardPanel.SetActive(true); 
        mainPanel.SetActive(false); 
        if (panelCanvasGroup != null) panelCanvasGroup.interactable = true;
        
        
        CheckRewardStatus();
        UpdateUI(); 
    }
    
    public void ClosePanel() 
    { 
        dailyRewardPanel.SetActive(false); 
        mainPanel.SetActive(true); 
    }
}