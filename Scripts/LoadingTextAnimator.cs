using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LoadingTextAnimator : MonoBehaviour
{
    private TextMeshProUGUI textComponent;
    
    [Header("Localization")]
    [Tooltip("LocalizationManager içindeki anahtar kelimeyi yazın (örn: loading)")]
    public string textKey = "loading"; 

    [Header("Settings")]
    public float animationSpeed = 0.4f; 
    
    
    private string localizedBaseText; 

    void Awake()
    {
        
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        
        localizedBaseText = LocalizationManager.GetText(textKey);

        
        StartCoroutine(AnimateDots());
    }

    IEnumerator AnimateDots()
    {
        int dotCount = 1; 

        
        while (true)
        {
            
            string dots = new string('.', dotCount);
            
            
            textComponent.text = localizedBaseText + dots;

            
            dotCount++;
            if (dotCount > 3)
            {
                dotCount = 1; 
            }

            
            yield return new WaitForSeconds(animationSpeed);
        }
    }
}