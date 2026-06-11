using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI goldText;
    public GameObject mainPanel;
    public GameObject shopPanel;
    public GameObject settingsPanel;
    public GameObject loadingPanel;
    public GameObject planetPanel;
    public CanvasGroup logoGroup;
    public float fadeTime = 1.5f;
    void Awake()
    {
        LocalizationManager.LoadLanguage();
    }
    void Start()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMusicWithFade(SoundType.MainMenuMusic);
        if (logoGroup != null)
        {
            StartCoroutine(FadeOutRoutine(logoGroup,fadeTime));
        }
        UpdateUI();
    }
    public IEnumerator FadeOutRoutine(CanvasGroup targetGroup, float duration)
    {
        float elapsedTime = 0f;
        targetGroup.alpha = 1f; 

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            
            targetGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            yield return null; 
        }
        targetGroup.alpha = 0f;
    }
    void UpdateUI()
    {
        if (goldText != null)
            goldText.text = DataManager.GetGold().ToString();
    }

    public void PlayGame() 
    { 
        mainPanel.SetActive(false); 
        planetPanel.SetActive(true); 
    }
    public void ClosePlanet() 
    { 
        planetPanel.SetActive(false); 
        mainPanel.SetActive(true);
    }
    public void OpenShop() 
    { 
        mainPanel.SetActive(false); 
        shopPanel.SetActive(true); 
        UpdateUI(); 
    }

    public void CloseShop() 
    { 
        shopPanel.SetActive(false); 
        mainPanel.SetActive(true); 
        UpdateUI(); 
    }

    public void OpenSettings() 
    { 
        settingsPanel.SetActive(true); 
        mainPanel.SetActive(false); 
    }

    public void CloseSettings() 
    { 
        settingsPanel.SetActive(false); 
        mainPanel.SetActive(true); 
    }
}