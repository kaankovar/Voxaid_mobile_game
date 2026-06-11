using UnityEngine;
using UnityEngine.Events; 
using System; 
using System.Collections; 
using UnityEngine.SceneManagement;
using TMPro; 
using System.Collections.Generic;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    
    [Header("Security")]
    public CanvasGroup globalUICanvasGroup; 

    [Header("Ad Buttons")]
    public Button reviveButton;     
    public Button doubleGoldButtonGameOver; 
    public Button doubleGoldButtonVictory;
    public static event Action OnBossSpawned; 
    private bool hasWatchedAdThisSession = false;

    [Header("Planet System")]
    public List<PlanetData> allPlanets; 
    public PlanetData currentPlanet;
    public GameObject victoryPanel; 
    public Image countdown;
    public GameObject victoryContainer;
    public GameObject gameovercontainer;

    [Header("Stats")]
    public int currentLevel = 1;
    public int currentExperience = 0;
    public int experienceToNextLevel = 100;
    public int baseExperience = 100;
    public int totalExperienceGained = 0; 
    public int goldCollectedInThisSession = 0; 

    [Header("Settings")]
    public float levelMultiplier = 1.2f; 
    public int bossLevel = 10; 

    public static event Action<int, int> OnExperienceChanged; 
    public static event Action<int> OnLevelUp; 
    [Header("UI References")]
    public GameObject gameOverPanel;
    public GameObject hudPanel;
    public GameObject pausePanel;
    public GameObject settingsPanel;
    public TextMeshProUGUI earnedGoldText; 
    public TextMeshProUGUI victoryGoldText;
    public GameObject levelslider;
    public Slider bossHealthBar;

    [Header("Revive System")]
    public GameObject revivePanel; 
    private Coroutine reviveCoroutine; 

    void Awake()
    {
        Instance = this;
        LoadCurrentPlanet();
    }
    
    void Start()
    {
        AudioManager.Instance.PlayMusicWithFade(SoundType.GameMusic);
        SetUIInteractable(true); 
    }
    
    void LoadCurrentPlanet()
    {
        int planetIndex = DataManager.SelectedPlanetIndex;
        currentPlanet = allPlanets.Find(p => p.planetIndex == planetIndex);
        
        if (currentPlanet == null && allPlanets.Count > 0)
            currentPlanet = allPlanets[0];
        if (currentPlanet != null)
        {
            
            experienceToNextLevel = Mathf.RoundToInt(baseExperience * currentPlanet.xpRequirementMultiplier);
        }
    }

    
    private void SetUIInteractable(bool state)
    {
        if (globalUICanvasGroup != null)
        {
            globalUICanvasGroup.interactable = state;
        }
    }

    public void AddExperience(int amount)
    {
        currentExperience += amount;
        totalExperienceGained += amount;
        if (currentExperience >= experienceToNextLevel) LevelUp();
        OnExperienceChanged?.Invoke(currentExperience, experienceToNextLevel);
    }

    public void AddGold(int amount)
    {
        goldCollectedInThisSession += amount;
        DataManager.AddGold(amount);
    }

    void LevelUp()
    {
        currentLevel++;
        currentExperience -= experienceToNextLevel; 
        experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * levelMultiplier);
        OnLevelUp?.Invoke(currentLevel);
        OnExperienceChanged?.Invoke(currentExperience, experienceToNextLevel);
        
        if (currentLevel == bossLevel) SpawnBossPhase();
        else if (currentLevel < bossLevel)
        {
            if(UpgradeManager.Instance != null) UpgradeManager.Instance.ShowUpgradeOptions();
        }
    }

    void SpawnBossPhase()
    {
        if(levelslider) levelslider.SetActive(false);
        OnBossSpawned?.Invoke(); 
        EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
        if (spawner != null) spawner.SpawnBoss();
    }

    public void PlanetComplete()
    {
        DataManager.UnlockNextPlanet(currentPlanet.planetIndex);
        if (hudPanel != null) hudPanel.SetActive(false);
        if (victoryPanel != null) 
        {
            victoryPanel.SetActive(true);
            if(victoryContainer) victoryContainer.SetActive(true);
            if(victoryGoldText) victoryGoldText.text = "+" + goldCollectedInThisSession;
        }
        Time.timeScale = 0; 
    }
    
    public void PauseGame() { if (pausePanel != null) pausePanel.SetActive(true); if (hudPanel != null) hudPanel.SetActive(false); Time.timeScale = 0; }
    public void OpenSettings() { if (settingsPanel != null) settingsPanel.SetActive(true); if (pausePanel != null) pausePanel.SetActive(false); Time.timeScale = 0; }
    public void CloseSettings() { if (settingsPanel != null) settingsPanel.SetActive(false); if (pausePanel != null) pausePanel.SetActive(true); Time.timeScale = 0; }
    public void Resumegame() { if (pausePanel != null) pausePanel.SetActive(false); if (hudPanel != null) hudPanel.SetActive(true); Time.timeScale = 1; }

    public void TriggerGameOver() 
    { 

        
        if (hudPanel != null) hudPanel.SetActive(false); 
        Time.timeScale = 0; 
        AudioManager.Instance.PlaySFX(SoundType.PlayerDeath); 

        if (revivePanel != null && !hasWatchedAdThisSession)
        {
            revivePanel.SetActive(true);
            reviveCoroutine = StartCoroutine(ReviveCountdownRoutine());
        }
        else
        {
            ShowFinalGameOverPanel();
        }
    }

    private IEnumerator ReviveCountdownRoutine()
    {
        float maxTime = 5f;
        float timer = maxTime;

        if (countdown != null) countdown.fillAmount = 1f;

        while (timer > 0)
        {
            timer -= Time.unscaledDeltaTime; 
            if (countdown != null) countdown.fillAmount = timer / maxTime; 
            yield return null;
        }

        SkipRevive();
    }

    public void SkipRevive()
    {
        if (reviveCoroutine != null) StopCoroutine(reviveCoroutine); 
        if (revivePanel != null) revivePanel.SetActive(false); 

        ShowFinalGameOverPanel(); 
    }

    private void ShowFinalGameOverPanel()
    {
        if (earnedGoldText != null) { gameovercontainer.SetActive(true); earnedGoldText.text = "+" + goldCollectedInThisSession; }
        if (gameOverPanel != null)
        { 
            gameOverPanel.SetActive(true);
        }
    }

    

    public void WatchAdToRevive()
    {
        SetUIInteractable(false); 
        if (reviveButton != null) reviveButton.interactable = false;
        if (reviveCoroutine != null) StopCoroutine(reviveCoroutine);

        AdManager.Instance.ShowRewardedAd(
            onAdComplete: () => 
            {
                hasWatchedAdThisSession = true;
                if (revivePanel != null) revivePanel.SetActive(false);
                if (hudPanel != null) hudPanel.SetActive(true);
                
                PlayerHealth.Instance.ReviveFromAd();
                SetUIInteractable(true); 

                Time.timeScale = 1; 
            },
            onAdFailed: () => 
            {
                Debug.Log("Reklam izlenemedi veya atlandı.");
                SetUIInteractable(true); 
                SkipRevive(); 
            }
        );
    }

    public void WatchAdToDoubleGoldGameOver()
    {
        SetUIInteractable(false); 
        if (doubleGoldButtonGameOver != null) doubleGoldButtonGameOver.interactable = false;

        AdManager.Instance.ShowRewardedAd(
            onAdComplete: () => 
            {
                hasWatchedAdThisSession = true;
                int bonusGold = goldCollectedInThisSession;
                AddGold(bonusGold);

                if (victoryGoldText != null) victoryGoldText.text = "+" + goldCollectedInThisSession;
                if (earnedGoldText != null) earnedGoldText.text = "+" + goldCollectedInThisSession;

                if (doubleGoldButtonGameOver != null) doubleGoldButtonGameOver.gameObject.SetActive(false);
                SetUIInteractable(true); 
            },
            onAdFailed: () => 
            {
                if (doubleGoldButtonGameOver != null) doubleGoldButtonGameOver.interactable = true;
                SetUIInteractable(true); 
            }
        );
    }
    public void WatchAdToDoubleGoldVictory()
    {
        SetUIInteractable(false); 
        if (doubleGoldButtonVictory != null) doubleGoldButtonVictory.interactable = false;

        AdManager.Instance.ShowRewardedAd(
            onAdComplete: () => 
            {
                hasWatchedAdThisSession = true;
                int bonusGold = goldCollectedInThisSession;
                AddGold(bonusGold);

                if (victoryGoldText != null) victoryGoldText.text = "+" + goldCollectedInThisSession;
                if (earnedGoldText != null) earnedGoldText.text = "+" + goldCollectedInThisSession;

                if (doubleGoldButtonVictory != null) doubleGoldButtonVictory.gameObject.SetActive(false);
                SetUIInteractable(true); 
            },
            onAdFailed: () => 
            {
                if (doubleGoldButtonVictory != null) doubleGoldButtonVictory.interactable = true;
                SetUIInteractable(true); 
            }
        );
    }
    public void RestartGame() 
    { 
        SetUIInteractable(false); 

        if (!hasWatchedAdThisSession) 
        {
            AdManager.Instance.ShowInterstitialAd(() => { 
                SetUIInteractable(true); 
                DoRestart(); 
            });
        }
        else 
        {
            SetUIInteractable(true);
            DoRestart(); 
        }
    }
    
    private void DoRestart() 
    { 
        
        AudioManager.Instance.StopMusicWithFade(); 
        PoolManager.Instance.ResetAllPools();      
        
        Time.timeScale = 1; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    }
    public void LoadMainMenu() 
    { 
        SetUIInteractable(false); 

        if (!hasWatchedAdThisSession) 
        {
            AdManager.Instance.ShowInterstitialAd(() => { 
                SetUIInteractable(true);
                DoLoadMenu(); 
            });
        }
        else 
        {
            SetUIInteractable(true);
            DoLoadMenu(); 
        }
    }
    
    private void DoLoadMenu() { AudioManager.Instance.StopMusicWithFade();PoolManager.Instance.ResetAllPools(); Time.timeScale = 1; SceneManager.LoadScene("MainMenu"); }
}