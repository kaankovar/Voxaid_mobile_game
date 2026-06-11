using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Slider xpSlider;
    public TextMeshProUGUI levelText;

    void Start()
    {
        LevelManager.OnExperienceChanged += UpdateXPBar;
        LevelManager.OnLevelUp += UpdateLevelText;
        UpdateXPBar(0,100);
    }

    void OnDestroy()
    {
        LevelManager.OnExperienceChanged -= UpdateXPBar;
        LevelManager.OnLevelUp -= UpdateLevelText;
    }

    void UpdateXPBar(int current, int max)
    {
        xpSlider.maxValue = max;
        xpSlider.value = current;
    }

    void UpdateLevelText(int newLevel)
    {
        levelText.text = "LEVEL " + newLevel;
    }
}