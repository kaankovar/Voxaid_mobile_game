using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))] 
public class LocalizedText : MonoBehaviour
{
    public string textKey; 
    private TextMeshProUGUI textComponent;

    void Awake()
    {
        
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        UpdateText();
    }

    
    void OnEnable()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        
        if (textComponent == null)
        {
            textComponent = GetComponent<TextMeshProUGUI>();
        }

        
        if (textComponent != null)
        {
            textComponent.text = LocalizationManager.GetText(textKey);
        }
    }
}