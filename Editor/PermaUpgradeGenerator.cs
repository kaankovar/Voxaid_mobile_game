using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class PermaUpgradeGenerator : EditorWindow
{
    [MenuItem("Tools/Generate All Perma Upgrades")]
    public static void Generate()
    {
        string savePath = "Assets/Resources/PermaUpgrades"; 
        
        
        string iconPath = "Assets/UI&Mattexture"; 

        if (!Directory.Exists(savePath)) { Directory.CreateDirectory(savePath); }
        if (!Directory.Exists(iconPath)) { Directory.CreateDirectory(iconPath); }
        
        AssetDatabase.Refresh();

        
        Dictionary<string, Sprite> spriteDictionary = new Dictionary<string, Sprite>();
        string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D t:Sprite", new[] { iconPath });
        
        foreach (string guid in textureGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path); 
            
            foreach (Object asset in assets)
            {
                if (asset is Sprite sprite)
                {
                    string cleanSpriteName = sprite.name.ToLower().Replace(" ", "");
                    if (!spriteDictionary.ContainsKey(cleanSpriteName))
                    {
                        spriteDictionary.Add(cleanSpriteName, sprite);
                    }
                }
            }
        }

        
        
        List<PermaInfo> allUpgrades = new List<PermaInfo>
        {
            
            new PermaInfo("Strength", "Strength", "Kuvvet", "+10% Base Damage.", "+%10 Temel Hasar.", 10, 150, 1.5f),
            new PermaInfo("Haste", "Haste", "Sürat", "+10% Fire Rate.", "+%10 Atış Hızı.", 10, 150, 1.5f),
            new PermaInfo("Lethality", "Lethality", "Ölümcüllük", "+5% Critical Hit Chance.", "+%5 Kritik Vuruş Şansı.", 10, 200, 1.6f),
            new PermaInfo("Impact", "Impact", "Darbe", "+1 Knockback Force.", "+1 Geri İtme Gücü.", 5, 120, 1.4f),
            new PermaInfo("Barrage", "Barrage", "Yaylım Ateşi", "+1 Extra Bullet per shot.", "Atış başına +1 Ekstra Mermi.", 3, 500, 2.0f),
            new PermaInfo("Drill", "Drill", "Delici", "Bullets pierce +1 additional enemy.", "Mermiler +1 düşmanı delip geçer.", 3, 400, 2.0f),
            new PermaInfo("Bounce", "Bounce", "Sekme", "Bullets bounce off walls and enemies.", "Mermiler duvarlardan ve düşmanlardan seker.", 1, 600, 1.0f),
            new PermaInfo("Velocity", "Velocity", "Mermi Hızı", "+10% Projectile Speed.", "+%10 Mermi Hızı.", 5, 100, 1.4f),
            new PermaInfo("Extension", "Extension", "Genişleme", "+15% Projectile Range and Lifetime.", "+%15 Mermi Menzili ve Ömrü.", 5, 150, 1.5f),
            new PermaInfo("Gambler", "Gambler", "Kumarbaz", "+50% Critical Hit Damage.", "+%50 Kritik Hasar Çarpanı.", 5, 300, 1.8f),
            new PermaInfo("Giantism", "Giantism", "Devleşme", "+15% Projectile Size.", "+%15 Mermi Boyutu.", 5, 150, 1.5f),
            new PermaInfo("Stability", "Stability", "İstikrar", "Reduces weapon spread.", "Silah sapmasını azaltır.", 5, 100, 1.4f),
            new PermaInfo("Executioner", "Executioner", "Cellat", "+5% Execution Threshold for low HP enemies.", "Canı az olan düşmanlar için +%5 İnfaz sınırı.", 5, 250, 1.6f),
            new PermaInfo("Detonation", "Detonation", "İnfilak", "Enemies violently explode upon death.", "Düşmanlar öldüklerinde patlar.", 5, 300, 1.8f),
            new PermaInfo("Headhunter", "Headhunter", "Kelle Avcısı", "+15% Damage against Elite and Boss enemies.", "Elit ve Boss düşmanlara karşı +%15 Hasar.", 5, 300, 1.6f),

            
            new PermaInfo("Vitality", "Vitality", "Canlılık", "+20 Maximum Health.", "+20 Maksimum Can.", 10, 100, 1.5f),
            new PermaInfo("Resilience", "Resilience", "Direnç", "+25 Starting Armor.", "+25 Başlangıç Zırhı.", 10, 150, 1.5f),
            new PermaInfo("Agility", "Agility", "Çeviklik", "Increases overall movement speed.", "Hareket hızını artırır.", 5, 150, 1.5f),
            new PermaInfo("Hardy", "Hardy", "Dayanıklı", "+2% Damage Reduction.", "+%2 Hasar Düşürme.", 5, 200, 1.6f),
            new PermaInfo("Evasion", "Evasion", "Kaçınma", "+3% Dodge Chance.", "+%3 Kaçınma Şansı.", 5, 300, 1.8f),
            new PermaInfo("Ghost", "Ghost", "Hayalet", "Grants brief invulnerability after taking damage.", "Hasar alındıktan sonra kısa süreli ölümsüzlük sağlar.", 5, 250, 1.8f),
            new PermaInfo("Thorns", "Thorns", "Dikenler", "Reflects +5 Damage to attackers.", "Saldıranlara +5 Hasar yansıtır.", 5, 150, 1.5f),
            new PermaInfo("Regeneration", "Regeneration", "Yenilenme", "Slowly regenerates health over time.", "Zamanla yavaşça can yeniler.", 5, 300, 2.0f),
            new PermaInfo("Vampirism", "Vampirism", "Vampirlik", "+2% Chance to heal when defeating enemies.", "Düşman öldürüldüğünde +%2 İyileşme Şansı.", 5, 400, 2.0f),
            new PermaInfo("Scavenger", "Scavenger", "Yağmacı", "Restores +1 Health when picking up gold.", "Altın toplandığında +1 Can iyileştirir.", 5, 200, 1.6f),
            new PermaInfo("SecondChance", "SecondChance", "İkinci Şans", "+1 Extra Life (Revive).", "+1 Ekstra Can (Dirilme).", 1, 1500, 1.0f),

            
            new PermaInfo("Greed", "Greed", "Açgözlülük", "+10% Gold Multiplier.", "+%10 Altın Çarpanı.", 10, 200, 1.8f),
            new PermaInfo("Intelligence", "Intelligence", "Zeka", "+10% XP Multiplier.", "+%10 XP Çarpanı.", 10, 200, 1.8f),
            new PermaInfo("Logistics", "Logistics", "Lojistik", "+1 Reroll chance.", "+1 Seçenek Yenileme (Reroll) hakkı.", 5, 300, 2.0f),
            new PermaInfo("Magnetism", "Magnetism", "Manyetizma", "Increases item and experience pickup range.", "Eşya ve tecrübe toplama menzilini artırır.", 10, 100, 1.5f)
        };

        foreach (var info in allUpgrades)
        {
            PermaUpgrade asset = ScriptableObject.CreateInstance<PermaUpgrade>();
            
            asset.saveID = info.id;
            asset.upgradeNames = new string[] { info.nameEN, info.nameTR };
            asset.descriptions = new string[] { info.descEN, info.descTR };
            asset.maxLevel = info.maxLvl;
            asset.baseCost = info.baseCost;
            asset.costMultiplier = info.costMult;

            
            string searchName = info.nameEN.ToLower().Replace(" ", "");

            if (spriteDictionary.ContainsKey(searchName))
            {
                asset.icon = spriteDictionary[searchName];
            }
            else
            {
                Debug.LogWarning($"❌ İkon Bulunamadı: '{info.nameEN}'. Sprite Sheet içindeki dilim adını veya klasör yolunu kontrol et!");
            }

            string fileName = info.id + ".asset"; 
            string fullPath = savePath + "/" + fileName;
            string uniquePath = AssetDatabase.GenerateUniqueAssetPath(fullPath);
            AssetDatabase.CreateAsset(asset, uniquePath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("<color=cyan>VOXAID PERMA UPGRADES OLUŞTURULDU!</color> Sayısal değerler koda göre eklendi, göze batan ondalıklar temizlendi.");
    }

    private class PermaInfo
    {
        public string id, nameEN, nameTR, descEN, descTR;
        public int maxLvl, baseCost;
        public float costMult;

        public PermaInfo(string i, string nEN, string nTR, string dEN, string dTR, int mLvl, int bCost, float cMult)
        {
            id = i; nameEN = nEN; nameTR = nTR; descEN = dEN; descTR = dTR; maxLvl = mLvl; baseCost = bCost; costMult = cMult;
        }
    }
}