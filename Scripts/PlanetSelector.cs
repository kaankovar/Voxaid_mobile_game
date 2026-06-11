using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class PlanetSelector : MonoBehaviour
{
    [Header("Settings")]
    public Transform gridParent; 
    public GameObject planetButtonPrefab; 
    public List<PlanetData> planets; 

    void Start()
    {
        RefreshPlanetList();
    }

    public void RefreshPlanetList()
    {
        
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        
        int unlockedPlanetIndex = DataManager.GetUnlockedPlanet(); 

        foreach (PlanetData planet in planets)
        {
            GameObject btnObj = Instantiate(planetButtonPrefab, gridParent);
            Button btn = btnObj.GetComponent<Button>();
            
            
            
            TextMeshProUGUI nameText = btnObj.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            Image iconImage = btnObj.transform.Find("IconImage")?.GetComponent<Image>();
            GameObject lockIcon = btnObj.transform.Find("LockIcon")?.gameObject; 

            
            if (nameText) nameText.text = planet.planetName;
            if (iconImage && planet.planetIcon) iconImage.sprite = planet.planetIcon;

            
            bool isUnlocked = planet.planetIndex <= unlockedPlanetIndex;

            if (isUnlocked)
            {
                
                btn.interactable = true;
                if(lockIcon) lockIcon.SetActive(false);
                
                
                btn.onClick.AddListener(() => OnPlanetClicked(planet));
            }
            else
            {
                
                btn.interactable = false; 
                if(lockIcon) lockIcon.SetActive(true); 
                
                
                if(iconImage) iconImage.color = Color.gray;
            }
        }
    }

    void OnPlanetClicked(PlanetData selectedPlanet)
    {
        
        DataManager.SelectedPlanetIndex = selectedPlanet.planetIndex;
        
        
        
        LoaderManager.Instance.LoadSceneAsync(selectedPlanet.sceneName);
    }
}