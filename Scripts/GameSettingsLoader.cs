using UnityEngine;

public class GameSettingsLoader : MonoBehaviour
{
    [Header("Referanslar")]
    public PlayerController playerController;
    public MobileJoystick leftJoystickScript;  
    public MobileJoystick rightJoystickScript; 

    void Start()
    {
        
        ApplySettings();
        
        
        AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume", 1f);
    }

    public void ApplySettings()
    {
        
        bool isDynamic = PlayerPrefs.GetInt("JoystickDynamic", 1) == 1;
        
        if(leftJoystickScript) leftJoystickScript.isDynamic = isDynamic;
        if(rightJoystickScript) rightJoystickScript.isDynamic = isDynamic;

        
        bool isSwapped = PlayerPrefs.GetInt("JoystickSwap", 0) == 1;

        if (playerController != null)
        {
            if (isSwapped)
            {
                
                playerController.moveJoystick = rightJoystickScript;
                playerController.lookJoystick = leftJoystickScript;
            }
            else
            {
                
                playerController.moveJoystick = leftJoystickScript;
                playerController.lookJoystick = rightJoystickScript;
            }
        }
    }
}