using UnityEngine;
using UnityEngine.Advertisements;
using System;
using System.Collections;

public class AdManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static AdManager Instance;

    [Header("Unity Ads Ayarları")]
    [SerializeField] private string androidGameId = "BURAYA_ANDROID_GAME_ID_GELECEK";
    [SerializeField] private string iosGameId = "BURAYA_IOS_GAME_ID_GELECEK";
    [SerializeField] private bool testMode = true;

    private string gameId;
    private string rewardedAdUnitId;
    private string interstitialAdUnitId;

    private Action onRewardedAdComplete;
    private Action onRewardedAdFailed;
    private Action onInterstitialAdComplete;

    private bool isInitializing = false;
    private float retryDelay = 5f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); return; }

        SetAdUnitIds();
        StartCoroutine(TryInitializeAdsRoutine());
    }

    private void SetAdUnitIds()
    {
#if UNITY_IOS
        gameId = iosGameId;
        rewardedAdUnitId = "Rewarded_iOS";
        interstitialAdUnitId = "Interstitial_iOS";
#elif UNITY_ANDROID || UNITY_EDITOR
        gameId = androidGameId;
        rewardedAdUnitId = "Rewarded_Android";
        interstitialAdUnitId = "Interstitial_Android";
#endif
    }

    private IEnumerator TryInitializeAdsRoutine()
    {
        while (!Advertisement.isInitialized)
        {
            if (!isInitializing && Application.internetReachability != NetworkReachability.NotReachable)
            {
                if (Advertisement.isSupported)
                {
                    isInitializing = true;
                    Debug.Log("İnternet bağlantısı algılandı, Unity Ads başlatılıyor...");
                    Advertisement.Initialize(gameId, testMode, this);
                }
            }

            if (!Advertisement.isInitialized)
                yield return new WaitForSeconds(retryDelay);
            else
                yield break;
        }
    }

    public void OnInitializationComplete()
    {
        isInitializing = false;
        Debug.Log("Unity Ads Başlatıldı.");
        LoadRewardedAd();
        LoadInterstitialAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        isInitializing = false;
        Debug.Log($"Unity Ads Başlatılamadı: {error} - {message}. {retryDelay} saniye sonra tekrar denenecek.");
    }

    public void LoadRewardedAd()
    {
        if (Advertisement.isInitialized)
            Advertisement.Load(rewardedAdUnitId, this);
    }

    public void LoadInterstitialAd()
    {
        if (Advertisement.isInitialized)
            Advertisement.Load(interstitialAdUnitId, this);
    }

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log($"{adUnitId} başarıyla yüklendi.");
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Reklam Yüklenemedi: {adUnitId} - Hata: {error}. {retryDelay} saniye sonra tekrar denenecek.");

        if (adUnitId == rewardedAdUnitId && onRewardedAdFailed != null)
        {
            onRewardedAdFailed.Invoke();
            onRewardedAdFailed = null;
        }

        StartCoroutine(RetryLoadAdRoutine(adUnitId));
    }

    private IEnumerator RetryLoadAdRoutine(string adUnitId)
    {
        yield return new WaitForSeconds(retryDelay);

        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            Debug.Log($"{adUnitId} için reklam yüklemesi tekrar deneniyor...");
            Advertisement.Load(adUnitId, this);
        }
        else
        {
            StartCoroutine(RetryLoadAdRoutine(adUnitId));
        }
    }

    public void ShowRewardedAd(Action onAdComplete, Action onAdFailed)
    {
        onRewardedAdComplete = onAdComplete;
        onRewardedAdFailed = onAdFailed;
        Advertisement.Show(rewardedAdUnitId, this);
    }

    public void ShowInterstitialAd(Action onAdComplete)
    {
        onInterstitialAdComplete = onAdComplete;
        Advertisement.Show(interstitialAdUnitId, this);
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        if (adUnitId == rewardedAdUnitId && onRewardedAdFailed != null) onRewardedAdFailed.Invoke();
        if (adUnitId == interstitialAdUnitId && onInterstitialAdComplete != null) onInterstitialAdComplete.Invoke();
    }

    public void OnUnityAdsShowStart(string adUnitId) { AudioListener.pause = true; }
    public void OnUnityAdsShowClick(string adUnitId) { }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        AudioListener.pause = false;
        if (adUnitId.Equals(rewardedAdUnitId))
        {
            if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
                onRewardedAdComplete?.Invoke();
            else
                onRewardedAdFailed?.Invoke();

            onRewardedAdComplete = null;
            onRewardedAdFailed = null;
            LoadRewardedAd();
        }
        else if (adUnitId.Equals(interstitialAdUnitId))
        {
            onInterstitialAdComplete?.Invoke();
            onInterstitialAdComplete = null;
            LoadInterstitialAd();
        }
    }
}