using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LoaderManager : MonoBehaviour
{
    public static LoaderManager Instance;

    [Header("UI References")]
    public GameObject loadingScreenPanel; 
    public Slider progressBar; 
    public TextMeshProUGUI progressText; 

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

    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    IEnumerator LoadSceneRoutine(string sceneName)
    {
        AudioListener.pause = true; 
        
        if (loadingScreenPanel) loadingScreenPanel.SetActive(true);
        if (Camera.main != null) loadingScreenPanel.GetComponent<Canvas>().worldCamera = Camera.main;
        
        if (progressBar) progressBar.value = 0;
        if (progressText) progressText.text = "%0";

        System.GC.Collect();
        yield return Resources.UnloadUnusedAssets(); 

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        float visualProgress = 0f;

        
        
        while (visualProgress < 0.8f)
        {
            
            float targetProgress = (operation.progress / 0.9f) * 0.8f; 
            
            visualProgress = Mathf.MoveTowards(visualProgress, targetProgress, Time.unscaledDeltaTime * 1.5f);
            
            if (progressBar) progressBar.value = visualProgress;
            if (progressText) progressText.text = "%" + Mathf.RoundToInt(visualProgress * 100).ToString();

            
            if (visualProgress >= 0.79f)
            {
                operation.allowSceneActivation = true;
                break;
            }

            yield return null;
        }

        
        operation.allowSceneActivation = true; 

        
        while (!operation.isDone) yield return null;

        
        while (PoolManager.Instance == null) yield return null;

        
        while (!PoolManager.Instance.isInitialized)
        {
            float targetProgress = 0.8f + (PoolManager.Instance.initProgress * 0.2f);
            visualProgress = Mathf.MoveTowards(visualProgress, targetProgress, Time.unscaledDeltaTime * 2f);
            
            if (progressBar) progressBar.value = visualProgress;
            if (progressText) progressText.text = "%" + Mathf.RoundToInt(visualProgress * 100).ToString();
            
            yield return null;
        
        }

        if (InfiniteObjectSpawner.Instance != null)
        {
            while (!InfiniteObjectSpawner.Instance.isInitialized) yield return null;
        }

        
        if (progressBar) progressBar.value = 1f;
        if (progressText) progressText.text = "%100";

        yield return new WaitForSecondsRealtime(0.1f);

        if (loadingScreenPanel) loadingScreenPanel.SetActive(false);
        AudioListener.pause = false; 
    }
}