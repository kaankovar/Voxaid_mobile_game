using UnityEngine;
using UnityEngine.UI;

public class LanguageSettingsUI : MonoBehaviour
{
    [Header("Seçim İşaretleri (Yuvarlak Çerçeveler)")]
    public GameObject englishHighlight;
    public GameObject turkishHighlight;

    void Start()
    {
        
        UpdateVisuals(LocalizationManager.currentLanguage);
    }

    
    public void SelectEnglish()
    {
        if (LocalizationManager.currentLanguage == 0) return; 
        
        LocalizationManager.SetLanguage(0);
        UpdateVisuals(0);
        RefreshAllTexts();
    }

    
    public void SelectTurkish()
    {
        if (LocalizationManager.currentLanguage == 1) return; 

        LocalizationManager.SetLanguage(1);
        UpdateVisuals(1);
        RefreshAllTexts();
    }

    
    private void UpdateVisuals(int langIndex)
    {
        
        englishHighlight.SetActive(langIndex == 0);
        turkishHighlight.SetActive(langIndex == 1);
    }

    
    private void RefreshAllTexts()
    {
        
        LocalizedText[] allTexts = FindObjectsByType<LocalizedText>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        foreach (LocalizedText txt in allTexts)
        {
            txt.UpdateText();
        }
        
        Debug.Log("Dil değiştirildi ve ekrandaki tüm yazılar anında güncellendi!");
    }
}