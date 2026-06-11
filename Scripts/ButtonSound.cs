using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    void Start()
    {
        
        if (TryGetComponent<Button>(out Button btn))
        {
            btn.onClick.AddListener(() => 
                AudioManager.Instance.PlaySFX(SoundType.ButtonClick));
        }
        
        else if (TryGetComponent<Toggle>(out Toggle toggle))
        {
            
            
            toggle.onValueChanged.AddListener((isOn) => 
                AudioManager.Instance.PlaySFX(SoundType.ButtonClick));
        }
    }
}