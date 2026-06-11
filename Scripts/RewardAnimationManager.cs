using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class RewardAnimationManager : MonoBehaviour
{
    public static RewardAnimationManager Instance; 

    [Header("UI & Animation References")]
    public GameObject goldRewardPrefab; 
    public Transform mainCanvas;
    public Transform shopButtonTarget; 
    
    [Header("Settings")]
    public float centerWaitTime = 1.0f;
    public float moveDuration = 0.8f;

    private GameObject uiBlocker;

    void Awake()
    {
        Instance = this;
    }

    
    public void PlayGoldRewardAnimation(int amount)
    {
        EnableUIBlocker();
        StartCoroutine(GoldAnimationRoutine(amount));
    }

    IEnumerator GoldAnimationRoutine(int amount)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(SoundType.Jackpot);

        
        GameObject goldAnimObj = Instantiate(goldRewardPrefab, mainCanvas);
        RectTransform animRect = goldAnimObj.GetComponent<RectTransform>();

        
        TextMeshProUGUI tmpText = goldAnimObj.GetComponentInChildren<TextMeshProUGUI>();
        if (tmpText != null)
        {
            tmpText.text = "+" + amount.ToString();
        }

        goldAnimObj.transform.SetAsLastSibling();
        animRect.anchoredPosition = Vector2.zero; 

        yield return new WaitForSecondsRealtime(centerWaitTime);

        
        Vector3 startPos = animRect.position;
        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / moveDuration);

            animRect.position = Vector3.Lerp(startPos, shopButtonTarget.position, t);
            animRect.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.2f, t);

            yield return null;
        }

        Destroy(goldAnimObj);
        DisableUIBlocker();
    }

    
    public void EnableUIBlocker()
    {
        if (uiBlocker == null)
        {
            
            uiBlocker = new GameObject("Prestige_UI_Blocker");
            RectTransform rect = uiBlocker.AddComponent<RectTransform>();
            
            
            rect.SetParent(mainCanvas, false);
            
            
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;

            
            Image img = uiBlocker.AddComponent<Image>();
            img.color = new Color(0, 0, 0, 0); 
            img.raycastTarget = true; 
        }
        
        
        uiBlocker.transform.SetAsLastSibling();
        uiBlocker.SetActive(true);
    }
    public void DisableUIBlocker() 
    {
        if (uiBlocker != null)
        {
            uiBlocker.SetActive(false);
        } 
    }
}