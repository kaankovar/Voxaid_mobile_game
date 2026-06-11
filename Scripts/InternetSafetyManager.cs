using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class InternetSafetyManager : MonoBehaviour
{
    public static InternetSafetyManager Instance { get; private set; }

    [Header("Prefab Ayarları")]
    public GameObject noInternetPrefab; 
    private GameObject spawnedPanel;    
    private Button retryButton;

    [Header("Ayarlar")]
    public float checkInterval = 5f; 
    private bool isPausedByNetwork = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
{
    if (Application.internetReachability == NetworkReachability.NotReachable)
    {
        HandleNetworkLoss();
    }
    
    StartCoroutine(ContinuousConnectionCheck());
}

    IEnumerator ContinuousConnectionCheck()
    {
        while (true)
        {
            if (!isPausedByNetwork)
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    HandleNetworkLoss();
                }
            }
            yield return new WaitForSeconds(checkInterval);
        }
    }

    void HandleNetworkLoss()
    {
        isPausedByNetwork = true;
        
        
        PlayerPrefs.Save(); 
        Debug.Log("Bağlantı koptu, veriler güvene alındı.");

        Time.timeScale = 0f;

        if (spawnedPanel == null)
        {
            CreatePanelFromPrefab();
        }
        else
        {
            spawnedPanel.SetActive(true);
        }
    }

    void CreatePanelFromPrefab()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) return;

        
        spawnedPanel = Instantiate(noInternetPrefab, canvas.transform);
        
        
        RectTransform rect = spawnedPanel.GetComponent<RectTransform>();
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;

        
        retryButton = spawnedPanel.GetComponentInChildren<Button>();
        
        if (retryButton != null)
        {
            
            retryButton.onClick.RemoveAllListeners();
            
            retryButton.onClick.AddListener(AttemptReconnect);
            
            Debug.Log("Prefab butonu başarıyla koda bağlandı!");
        }
        else
        {
            Debug.LogError("Prefab içinde buton bulunamadı!");
        }
    }

    public void AttemptReconnect()
    {
        StartCoroutine(PingCheck());
    }

    IEnumerator PingCheck()
    {
        if(retryButton != null) retryButton.interactable = false;

        UnityWebRequest request = UnityWebRequest.Get("https://www.google.com");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ResumeGame();
        }
        else
        {
            Debug.LogWarning("İnternet hala yok.");
        }

        if(retryButton != null) retryButton.interactable = true;
    }

    void ResumeGame()
    {
        isPausedByNetwork = false;
        if (spawnedPanel != null) spawnedPanel.SetActive(false);
        Time.timeScale = 1f; 
    }
}