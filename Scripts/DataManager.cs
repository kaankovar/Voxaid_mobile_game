using UnityEngine;

public static class DataManager
{
    public static int GetGold()
    {
        return PlayerPrefs.GetInt("TotalGold", 0); 
    }

    public static void AddGold(int amount)
    {
        int current = GetGold();
        PlayerPrefs.SetInt("TotalGold", current + amount);
        PlayerPrefs.Save();
    }

    public static bool SpendGold(int amount)
    {
        int current = GetGold();
        if (current >= amount)
        {
            PlayerPrefs.SetInt("TotalGold", current - amount);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }
    
    public static int GetUpgradeLevel(string upgradeName)
    {
        return PlayerPrefs.GetInt("Upgrade_" + upgradeName, 0);
    }

    public static void IncreaseUpgradeLevel(string upgradeName)
    {
        int current = GetUpgradeLevel(upgradeName);
        PlayerPrefs.SetInt("Upgrade_" + upgradeName, current + 1);
        PlayerPrefs.Save();
    }
    public static void ReduceUpgradeLevel(string upgradeName)
    {
        int current = GetUpgradeLevel(upgradeName);
        PlayerPrefs.SetInt("Upgrade_" + upgradeName, current - 1);
        PlayerPrefs.Save();
    }
    public static void SetUpgradeLevel(string upgradeName , int amount)
    {
        PlayerPrefs.SetInt("Upgrade_" + upgradeName, amount);
        PlayerPrefs.Save();
    }

    
    public static void SetLastRewardTime(System.DateTime date)
    {
        PlayerPrefs.SetString("LastRewardTime", date.ToBinary().ToString());
        PlayerPrefs.Save();
    }

    public static System.DateTime GetLastRewardTime()
    {
        string temp = PlayerPrefs.GetString("LastRewardTime", "");
        if (!string.IsNullOrEmpty(temp))
        {
            long binary = System.Convert.ToInt64(temp);
            return System.DateTime.FromBinary(binary);
        }
        else
        {
            return System.DateTime.Now.AddDays(-2); 
        }
    }

    public static int GetDailyStreak()
    {
        return PlayerPrefs.GetInt("DailyStreak", 0);
    }

    public static void SetDailyStreak(int streak)
    {
        PlayerPrefs.SetInt("DailyStreak", streak);
        PlayerPrefs.Save();
    }

    public static int GetUnlockedPlanet()
    {
        return PlayerPrefs.GetInt("UnlockedPlanet", 1);
    }

    public static void UnlockNextPlanet(int currentPlanetIndex)
    {
        int unlocked = GetUnlockedPlanet();
        if (currentPlanetIndex >= unlocked)
        {
            PlayerPrefs.SetInt("UnlockedPlanet", currentPlanetIndex + 1);
            PlayerPrefs.Save();
        }
    }

    
    public static int GetPrestigeLevel()
    {
        return PlayerPrefs.GetInt("PrestigeLevel", 0);
    }
    public static void DoPrestige()
    {
        int nextPrestige = GetPrestigeLevel() + 1;
        
        
        int savedGold = Mathf.RoundToInt(GetGold() * 0.20f); 
        
        int dailyStreak = GetDailyStreak();
        string lastRewardTime = PlayerPrefs.GetString("LastRewardTime", "");

        
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        int savedDynamic = PlayerPrefs.GetInt("JoystickDynamic", 0);
        int savedSwap = PlayerPrefs.GetInt("JoystickSwap", 0);
        int savedLanguage = PlayerPrefs.GetInt("Language", 0); 

        
        PlayerPrefs.DeleteAll(); 

        
        PlayerPrefs.SetInt("PrestigeLevel", nextPrestige);
        PlayerPrefs.SetInt("DailyStreak", dailyStreak);
        PlayerPrefs.SetString("LastRewardTime", lastRewardTime);
        PlayerPrefs.SetInt("UnlockedPlanet", 1);
        
        
        PlayerPrefs.SetFloat("MasterVolume", savedVolume);
        PlayerPrefs.SetInt("JoystickDynamic", savedDynamic);
        PlayerPrefs.SetInt("JoystickSwap", savedSwap);
        PlayerPrefs.SetInt("Language", savedLanguage); 
        
        
        int startingGold = savedGold + (nextPrestige * 250000);
        PlayerPrefs.SetInt("TotalGold", startingGold);
        
        PlayerPrefs.Save();

        
        LocalizationManager.currentLanguage = savedLanguage;
    }
    public static float GetPrestigeMultiplier()
    {
        return 1f + (GetPrestigeLevel() * 0.10f);
    }
    
    public static int SelectedPlanetIndex = 1;
    public static void SetLastAdGoldTime(System.DateTime date)
    {
        PlayerPrefs.SetString("LastAdGoldTime", date.ToBinary().ToString());
        PlayerPrefs.Save();
    }

    public static System.DateTime GetLastAdGoldTime()
    {
        string temp = PlayerPrefs.GetString("LastAdGoldTime", "");
        if (!string.IsNullOrEmpty(temp))
        {
            long binary = System.Convert.ToInt64(temp);
            return System.DateTime.FromBinary(binary);
        }
        else
        {
            
            return System.DateTime.Now.AddDays(-2); 
        }
    }
}