using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class UpgradeGenerator : EditorWindow
{
    [MenuItem("Tools/Generate All Upgrades (VOXAID)")]
    public static void Generate()
    {
        string savePath = "Assets/Resources/Upgrades"; 
        
        
        string iconPath = "Assets/Icons"; 

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

        
        List<UpgradeInfo> allUpgrades = new List<UpgradeInfo>
        {
            
            new UpgradeInfo("Poison Ammo", "Zehirli Mermi", "Poison enemies over time.", "Düşmanları zamanla zehirler.", UpgradeType.PoisonAmmo, 0f),
            new UpgradeInfo("Fire Ammo", "Ateş Mermisi", "Burn enemies with fire.", "Düşmanları ateşe verir.", UpgradeType.FireAmmo, 0f),
            new UpgradeInfo("Chain Lightning", "Zincirleme Yıldırım", "Shock enemies with lightning arcs.", "Düşmanları yıldırımla şok eder.", UpgradeType.ChainLightning, 0f),
            new UpgradeInfo("Split Shot", "Bölünen Atış", "Bullets split into smaller ones on impact.", "Mermiler çarpma anında parçalara bölünür.", UpgradeType.SplitShot, 0f),
            new UpgradeInfo("Rear Guard", "Arka Koruma", "Fires an extra bullet behind you.", "Arkana ekstra mermi ateşler.", UpgradeType.RearShot, 0f),
            new UpgradeInfo("Homing Tech", "Güdümlü Teknoloji", "Bullets automatically chase enemies.", "Mermiler düşmanları otomatik takip eder.", UpgradeType.Homing, 0f),
            new UpgradeInfo("Orbital Saw", "Yörünge Testeresi", "A deadly blade spins around you.", "Etrafında sürekli dönen ölümcül bir testere.", UpgradeType.OrbitalBlade, 0f),
            new UpgradeInfo("Landmine", "Mayın", "Drop explosive mines as you walk.", "Yürürken patlayıcı mayınlar bırakırsın.", UpgradeType.Landmine, 0f),
            new UpgradeInfo("Blast Radius", "Patlama Etkisi", "Enemies explode when they die.", "Düşmanlar öldüğünde patlar.", UpgradeType.CorpseExplosion, 0f),
            new UpgradeInfo("Glass Cannon", "Cam Top", "+50% Damage but -30% Max Health.", "+%50 Hasar ama -%30 Maksimum Can.", UpgradeType.GlassCannon, 0f),
            new UpgradeInfo("Ricochet", "Seken Mermi", "Bullets bounce off walls.", "Mermiler duvarlardan seker.", UpgradeType.Ricochet, 0f),
            new UpgradeInfo("New Weapon", "Yeni Silah", "Equips a new weapon (Assign manually).", "Yeni bir silah kuşanır (Manuel ata).", UpgradeType.NewWeapon, 0f),

            
            
            
            new UpgradeInfo("Might", "Kudret", "+15% Base Damage.", "+%15 Temel Hasar.", UpgradeType.Damage, 0.15f),
            new UpgradeInfo("Haste", "Sürat", "+10% Fire Rate.", "+%10 Atış Hızı.", UpgradeType.FireRate, 0.10f),
            new UpgradeInfo("Lethality", "Ölümcüllük", "+5% Critical Hit Chance.", "+%5 Kritik Vuruş Şansı.", UpgradeType.CriticalChance, 5f),
            new UpgradeInfo("Giantism", "Devleşme", "+20% Projectile Size.", "+%20 Mermi Boyutu.", UpgradeType.ProjectileSize, 0.20f),
            new UpgradeInfo("Intelligence", "Zeka", "+15% Experience Gain.", "+%15 Kazanılan XP.", UpgradeType.ExpGain, 0.15f),
            new UpgradeInfo("Dodge Master", "Kaçınma Ustası", "+3% Chance to dodge attacks.", "+%3 Hasardan Kaçınma Şansı.", UpgradeType.Dodge, 3f),
            new UpgradeInfo("Vitality", "Canlılık", "+25 Maximum Health.", "+25 Maksimum Can.", UpgradeType.MaxHealth, 25f),
            new UpgradeInfo("Thorns", "Dikenler", "Reflects 10 damage to attackers.", "Saldıranlara 10 diken hasarı yansıtır.", UpgradeType.Thorns, 10f),
            
            
            new UpgradeInfo("Speed Boost", "Hız Takviyesi", "Increases movement speed.", "Hareket hızını artırır.", UpgradeType.MoveSpeed, 0.5f),
            new UpgradeInfo("Magnet", "Mıknatıs", "Increases gold pickup range.", "Altın toplama menzilini genişletir.", UpgradeType.MagnetRange, 1.5f),
            new UpgradeInfo("Heavy Impact", "Ağır Darbe", "Increases knockback force.", "Düşmanları itme gücünü artırır.", UpgradeType.Knockback, 1f),
            new UpgradeInfo("Barrage", "Yaylım Ateşi", "Fires an extra bullet per shot.", "Atış başına ekstra mermi ateşler.", UpgradeType.Multishot, 1f),
            new UpgradeInfo("Drill Ammo", "Delici Mermi", "Bullets pierce through additional enemies.", "Mermilerin ekstra düşmanları delip geçmesini sağlar.", UpgradeType.Piercing, 1f),
            new UpgradeInfo("Second Chance", "İkinci Şans", "Grants an Extra Life (Revive).", "Ekstra Can (Dirilme) verir.", UpgradeType.Revive, 1f),

            
            new UpgradeInfo("First Aid", "İlk Yardım", "Instantly recovers 50 Health.", "Mevcut canı anında 50 iyileştirir.", UpgradeType.Heal, 50f),
            new UpgradeInfo("Plating", "Zırh Kaplama", "Grants 50 temporary Armor points.", "50 geçici zırh puanı verir.", UpgradeType.Armor, 50f),
            new UpgradeInfo("Tactical Nuke", "Taktik Nükleer", "Kills all enemies on screen.", "Ekrandaki tüm düşmanları yok eder.", UpgradeType.InstantNuke, 0f),
            new UpgradeInfo("Time Freeze", "Zaman Dondurma", "Freezes all enemies temporarily.", "Tüm düşmanları dondurur.", UpgradeType.InstantFreeze, 0f)
        };

        foreach (var info in allUpgrades)
        {
            UpgradeData asset = ScriptableObject.CreateInstance<UpgradeData>();
            
            asset.upgradeNames = new string[2];
            asset.upgradeNames[0] = info.nameEN;
            asset.upgradeNames[1] = info.nameTR;

            asset.descriptions = new string[2];
            asset.descriptions[0] = info.descEN;
            asset.descriptions[1] = info.descTR;
            
            asset.type = info.type;
            asset.value = info.val;

            
            string searchName = info.nameEN.ToLower().Replace(" ", "");
            
            if (spriteDictionary.ContainsKey(searchName))
            {
                asset.icon = spriteDictionary[searchName];
            }
            else
            {
                Debug.LogWarning($"❌ İkon Bulunamadı: '{info.nameEN}'. Sprite Sheet içindeki dilim adını kontrol et!");
            }

            string cleanName = info.nameEN.Replace(" ", "");
            string fileName = cleanName + ".asset";
            string fullPath = savePath + "/" + fileName;
            string uniquePath = AssetDatabase.GenerateUniqueAssetPath(fullPath);
            
            AssetDatabase.CreateAsset(asset, uniquePath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"<color=green>İŞLEM TAMAM!</color> Oyun içi UpgradeData objeleri kusursuz metinlerle oluşturuldu.");
    }

    private class UpgradeInfo
    {
        public string nameEN, nameTR, descEN, descTR;
        public UpgradeType type;
        public float val;

        public UpgradeInfo(string nEN, string nTR, string dEN, string dTR, UpgradeType t, float v)
        {
            nameEN = nEN; nameTR = nTR; descEN = dEN; descTR = dTR; type = t; val = v;
        }
    }
}