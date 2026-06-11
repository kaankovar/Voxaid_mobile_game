#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif
using UnityEngine;
using System.Collections; 

public class IOSPermissionHandler : MonoBehaviour
{
    void Start()
    {
        #if UNITY_IOS
        
        var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();

        if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {
            
            ATTrackingStatusBinding.RequestAuthorizationTracking();
            
            
            StartCoroutine(WaitForATTAnswerAndInitTikTok());
        }
        else
        {
            
            InitTikTok();
        }
        #else
        
        InitTikTok();
        #endif
    }

    #if UNITY_IOS
    private IEnumerator WaitForATTAnswerAndInitTikTok()
    {
        
        yield return new WaitUntil(() => ATTrackingStatusBinding.GetAuthorizationTrackingStatus() != ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED);

        
        InitTikTok();
    }
    #endif

    
    private void InitTikTok()
    {
        TikTokManager tikTokManager = FindObjectOfType<TikTokManager>();
        
        if (tikTokManager != null)
        {
            tikTokManager.InitializeTikTok();
            Debug.Log("İzin aşaması geçildi, TikTok başlatılıyor...");
        }
        else
        {
            Debug.LogError("Sahnede TikTokManager bulunamadı! Lütfen bir objeye eklediğinden emin ol.");
        }
    }
}