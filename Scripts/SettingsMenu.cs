using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class SettingsMenu : MonoBehaviour
{
    [Header("UI Bileşenleri")]
    public GameObject panelObj; 
    public Slider volumeSlider;
    public Toggle dynamicToggle;
    public Toggle swapToggle;

    [Header("Oyun İçi Referanslar (Sadece Oyun Sahnesinde Dolu Olacak)")]
    public GameSettingsLoader gameLoader; 

    void Start()
    {
        
        float savedVol = PlayerPrefs.GetFloat("MasterVolume", 1f);
        volumeSlider.value = savedVol;
        
        AudioListener.volume = savedVol; 

        
        bool isDynamic = PlayerPrefs.GetInt("JoystickDynamic", 0) == 0; 
        dynamicToggle.isOn = isDynamic;

        bool isSwapped = PlayerPrefs.GetInt("JoystickSwap", 0) == 1;
        swapToggle.isOn = isSwapped;

        
        volumeSlider.onValueChanged.AddListener(SetVolume);
        dynamicToggle.onValueChanged.AddListener(SetDynamic);
        swapToggle.onValueChanged.AddListener(SetSwap);
    }

    public void OpenSettings()
    {
        panelObj.SetActive(true);
        if (SceneManager.GetActiveScene().name != "MainMenu") 
        {
            Time.timeScale = 0;
        }
    }

    public void CloseSettings()
    {
        panelObj.SetActive(false);
        Time.timeScale = 1;
    }

    public void SetVolume(float value)
    {
        
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
    }

    public void SetDynamic(bool value)
    {
        
        PlayerPrefs.SetInt("JoystickDynamic", value ? 0 : 1);
        PlayerPrefs.Save();

        if (gameLoader != null) gameLoader.ApplySettings();
    }

    public void SetSwap(bool value)
    {
        PlayerPrefs.SetInt("JoystickSwap", value ? 1 : 0);
        PlayerPrefs.Save();

        if (gameLoader != null) gameLoader.ApplySettings();
    }
}