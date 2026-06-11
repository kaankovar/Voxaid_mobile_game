using UnityEngine;
using SDK; 

public class TikTokManager : MonoBehaviour
{
    [Header("iOS Ayarları")]
    [SerializeField] private string iosAppId = "id_numaran_buraya"; 
    [SerializeField] private string iosTikTokAppId = "ios_tiktok_id_buraya"; 

    [Header("Android Ayarları")]
    [SerializeField] private string androidAppId = "com.sirketin.oyunadin"; 
    [SerializeField] private string androidTikTokAppId = "android_tiktok_id_buraya"; 

    

    
    public void InitializeTikTok()
    {
        try 
        {
            TikTokConfig config = new TikTokConfig(iosAppId, iosTikTokAppId, androidAppId, androidTikTokAppId);
            TikTokBusinessSDK.InitializeSdk(config);
            Debug.Log("TikTok SDK Başarıyla Başlatıldı!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("TikTok SDK hatası: " + e.Message);
        }
    }
}