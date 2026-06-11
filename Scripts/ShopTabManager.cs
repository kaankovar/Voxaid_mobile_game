using UnityEngine;
using TMPro; 

public class ShopTabManager : MonoBehaviour
{
    [Header("Scroll Views (Paneller)")]
    public GameObject weaponScrollView;
    public GameObject survivalScrollView;
    public GameObject metaScrollView;

    [Header("Tab Butonları (Büyüme Efekti İçin)")]
    public Transform weaponButton;
    public Transform survivalButton;
    public Transform metaButton;

    [Header("Tab Metinleri (Renk Değişimi İçin)")]
    public TextMeshProUGUI weaponText;
    public TextMeshProUGUI survivalText;
    public TextMeshProUGUI metaText;

    [Header("Görsel Ayarlar")]
    public float selectedScale = 1.15f; 
    public float normalScale = 1.0f;    
    public Color selectedColor = Color.green; 
    public Color normalColor = Color.white;   

    
    void OnEnable()
    {
        OpenWeaponTab();
    }

    public void OpenWeaponTab()
    {
        weaponScrollView.SetActive(true);
        survivalScrollView.SetActive(false);
        metaScrollView.SetActive(false);

        
        UpdateVisuals(weaponButton, weaponText);
    }

    public void OpenSurvivalTab()
    {
        weaponScrollView.SetActive(false);
        survivalScrollView.SetActive(true);
        metaScrollView.SetActive(false);

        
        UpdateVisuals(survivalButton, survivalText);
    }

    public void OpenMetaTab()
    {
        weaponScrollView.SetActive(false);
        survivalScrollView.SetActive(false);
        metaScrollView.SetActive(true);

        
        UpdateVisuals(metaButton, metaText);
    }

    
    private void UpdateVisuals(Transform activeBtn, TextMeshProUGUI activeText)
    {
        
        Vector3 normalSize = Vector3.one * normalScale;
        
        if (weaponButton != null) weaponButton.localScale = normalSize;
        if (survivalButton != null) survivalButton.localScale = normalSize;
        if (metaButton != null) metaButton.localScale = normalSize;

        if (weaponText != null) weaponText.color = normalColor;
        if (survivalText != null) survivalText.color = normalColor;
        if (metaText != null) metaText.color = normalColor;

        
        if (activeBtn != null) 
        {
            activeBtn.localScale = Vector3.one * selectedScale;
        }

        if (activeText != null)
        {
            activeText.color = selectedColor;
        }
    }
}