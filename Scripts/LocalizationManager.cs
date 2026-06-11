using System.Collections.Generic;
using UnityEngine;

public static class LocalizationManager
{
    
    public static int currentLanguage = 0;

    
    private static Dictionary<string, string[]> texts = new Dictionary<string, string[]>()
    {
        { "daily_reward_status", new string[] { "DAILY REWARD!", "GÜNLÜK ÖDÜL!" } },
        { "daily_reward_amount", new string[] { "+{0} GOLD", "+{0} ALTIN" } },
        { "watch_ad_gold", new string[] { "WATCH AD\n+{0} GOLD", "REKLAM İZLE\n+{0} ALTIN" } },
        { "prestige_info", new string[] {
            
            "<b>PRESTIGE READY</b>\nReset your planets to gain massive power!\n\n<b>Rewards:</b>\n<color=yellow>+ {0:N0} Gold</color>\n<color=green>Damage: x{1:F1} to x{2:F1}</color>\n\n<i>Warning: All planet progress will be lost. Proceed?</i>",
            
            
            "<b>PRESTİJ HAZIR</b>\nMuazzam bir güç kazanmak için gezegenlerini sıfırla!\n\n<b>Ödüller:</b>\n<color=yellow>+ {0:N0} Altın</color>\n<color=green>Hasar: x{1:F1} => x{2:F1}</color>\n\n<i>Uyarı: Tüm gezegen ilerlemen silinecek. Onaylıyor musun?</i>"
        }},
        { "volume", new string[] { "Volume", "Ses" } },
        { "dinamik", new string[] { "Dynamic Joystick", "Dinamik Kumanda Kolu" } },
        { "lefthand", new string[] { "Left-Handed Mode", "Solak Mod" } },
        { "pause", new string[] { "PAUSED", "DURAKLAT" } },
        { "loading", new string[] { "LOADING", "YÜKLENİYOR" } },
        { "connection", new string[] { "NO CONNECTION", "İNTERNET YOK" } },
        { "weapon", new string[] { "WEAPON", "SİLAH" } },
        { "survivour", new string[] { "SURVIVAL", "HAYATTA KALMA" } },
        { "meta", new string[] { "META", "META" } },
        { "gameover", new string[] { "GAME OVER", "YENİLGİ" } },
        { "victory", new string[] { "VICTORY", "ZAFER" } },
    };

    public static string GetText(string key)
    {
        if (texts.ContainsKey(key))
        {
            return texts[key][currentLanguage];
        }
        return key; 
    }

    public static void SetLanguage(int langIndex)
    {
        currentLanguage = langIndex;
        PlayerPrefs.SetInt("Language", langIndex);
        PlayerPrefs.Save();
    }

    public static void LoadLanguage()
    {
        
        currentLanguage = PlayerPrefs.GetInt("Language", 0); 
    }
}