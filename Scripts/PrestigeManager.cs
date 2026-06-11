using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PrestigeManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject prestigeButton;
    public Transform starsContainer;
    public GameObject starIconPrefab;
    public Transform mainCanvas; 

    [Header("Prestige Info Panel (YENİ)")]
    public GameObject prestigeInfoPanel; 
    public TextMeshProUGUI prestigeInfoText; 

    [Header("Reward Animation References")]
    public GameObject goldRewardPrefab; 
    public Transform shopButtonTarget;  
    
    
    public GameObject multiplierPrefab; 
    public Transform multiplierTargetUI; 

    [Header("Animation Settings")]
    public float centerWaitTime = 1.5f; 
    public float moveDuration = 1.0f;   
    public float bigScaleMultiplier = 3f; 

    [Header("Settings")]
    public int requiredPlanetsToPrestige = 11;

    void Start()
    {
        CheckPrestigeAvailability();
        DrawPrestigeStars(); 
        
        
        if (prestigeInfoPanel != null) prestigeInfoPanel.SetActive(false);
    }

    void CheckPrestigeAvailability()
    {
        int unlockedPlanet = DataManager.GetUnlockedPlanet();
        prestigeButton.SetActive(unlockedPlanet >= requiredPlanetsToPrestige);
    }
    public void CloseInfoPanel()
    {
        prestigeInfoPanel.SetActive(false);
    }
    void DrawPrestigeStars(bool isAnimating = false)
    {
        int currentPrestige = DataManager.GetPrestigeLevel();

        foreach (Transform child in starsContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < currentPrestige; i++)
        {
            GameObject newStar = Instantiate(starIconPrefab, starsContainer);

            if (isAnimating && i == currentPrestige - 1)
            {
                CanvasGroup cg = newStar.AddComponent<CanvasGroup>();
                cg.alpha = 0f;
            }
        }
    }

    
    public void OpenPrestigePanel()
    {
        int currentPrestige = DataManager.GetPrestigeLevel();
        int nextPrestigeLevel = currentPrestige + 1;
        int goldReward = nextPrestigeLevel * 250000;
        
        
        float currentMultiplier = 1f + (currentPrestige * 0.1f); 
        float nextMultiplier = 1f + (nextPrestigeLevel * 0.1f);

        if (prestigeInfoText != null)
        {
            
            string localizedTemplate = LocalizationManager.GetText("prestige_info");
            prestigeInfoText.text = string.Format(localizedTemplate, goldReward, currentMultiplier, nextMultiplier);
        }

        prestigeInfoPanel.SetActive(true);
    }

    
    public void ConfirmPrestige()
    {
        prestigeInfoPanel.SetActive(false); 
        prestigeButton.SetActive(false); 

        RewardAnimationManager.Instance.EnableUIBlocker();
        AudioManager.Instance.PlaySFX(SoundType.Prestige);

        DataManager.DoPrestige(); 

        DrawPrestigeStars(isAnimating: true);

        StartCoroutine(PrestigeAnimationRoutine());

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(SoundType.LevelUp);
    }

    IEnumerator PrestigeAnimationRoutine()
    {
        Transform targetStar = starsContainer.GetChild(starsContainer.childCount - 1);
        RectTransform targetRect = targetStar.GetComponent<RectTransform>();

        GameObject animStar = Instantiate(starIconPrefab, mainCanvas);
        RectTransform animRect = animStar.GetComponent<RectTransform>();

        animStar.transform.SetAsLastSibling();

        animRect.anchorMin = new Vector2(0.5f, 0.5f);
        animRect.anchorMax = new Vector2(0.5f, 0.5f);
        animRect.pivot = new Vector2(0.5f, 0.5f);
        animRect.sizeDelta = targetRect.rect.size;
        
        if (animRect.sizeDelta.x <= 0.1f) 
        {
            animRect.sizeDelta = new Vector2(100f, 100f); 
        }

        animRect.anchoredPosition = Vector2.zero; 
        Vector3 fixedLocalPos = animRect.localPosition;
        fixedLocalPos.z = 0f;
        animRect.localPosition = fixedLocalPos;

        animRect.localScale = Vector3.one * bigScaleMultiplier;

        yield return new WaitForSecondsRealtime(centerWaitTime);

        Vector3 startPos = animRect.position;
        Vector3 startScale = animRect.localScale;
        Vector3 targetScale = Vector3.one;

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.unscaledDeltaTime; 
            float t = elapsed / moveDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            animRect.position = Vector3.Lerp(startPos, targetStar.position, t);
            animRect.localScale = Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }

        Destroy(animStar);

        CanvasGroup cg = targetStar.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 1f; 
            Destroy(cg);   
        }
        
        CheckPrestigeAvailability();

        
        StartCoroutine(GoldAnimationRoutine());
    }

    IEnumerator GoldAnimationRoutine()
    {
        int earnedGold = DataManager.GetPrestigeLevel() * 500000;
        
        
        RewardAnimationManager.Instance.PlayGoldRewardAnimation(earnedGold);
        
        
        yield return new WaitForSecondsRealtime(1.5f);

        
        StartCoroutine(MultiplierAnimationRoutine());
    }

    
   
    
    IEnumerator MultiplierAnimationRoutine()
    {
        if (multiplierPrefab == null || multiplierTargetUI == null) yield break;

        
        GameObject tempBlocker = new GameObject("TempUIBlocker");
        tempBlocker.transform.SetParent(mainCanvas, false);
        tempBlocker.transform.SetAsLastSibling(); 
        
        Image blockerImage = tempBlocker.AddComponent<Image>();
        blockerImage.color = new Color(0, 0, 0, 0); 
        blockerImage.raycastTarget = true; 
        
        RectTransform blockerRect = tempBlocker.GetComponent<RectTransform>();
        blockerRect.anchorMin = Vector2.zero;
        blockerRect.anchorMax = Vector2.one; 
        blockerRect.sizeDelta = Vector2.zero;
        

        GameObject animObj = Instantiate(multiplierPrefab, mainCanvas);
        animObj.transform.SetAsLastSibling(); 

        TextMeshProUGUI animText = animObj.GetComponentInChildren<TextMeshProUGUI>();
        if (animText != null)
        {
            int newPrestigeLevel = DataManager.GetPrestigeLevel(); 
            float currentMultiplier = 1f + (newPrestigeLevel * 0.1f);
            
            animText.color = Color.green; 
            animText.text = $"x{currentMultiplier:F1}";
        }

        RectTransform animRect = animObj.GetComponent<RectTransform>();
        animRect.anchorMin = new Vector2(0.5f, 0.5f);
        animRect.anchorMax = new Vector2(0.5f, 0.5f);
        animRect.pivot = new Vector2(0.5f, 0.5f);
        animRect.anchoredPosition = Vector2.zero; 
        animRect.localScale = Vector3.one * bigScaleMultiplier;

        yield return new WaitForSecondsRealtime(centerWaitTime);

        Vector3 startPos = animRect.position;
        Vector3 startScale = animRect.localScale;
        Vector3 targetScale = Vector3.one;

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / moveDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            animRect.position = Vector3.Lerp(startPos, multiplierTargetUI.position, t);
            animRect.localScale = Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }

        Destroy(animObj);
        AudioManager.Instance.PlaySFX(SoundType.LevelUp); 

        
        Destroy(tempBlocker);
        
    }
}