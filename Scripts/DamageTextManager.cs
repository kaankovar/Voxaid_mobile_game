using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager Instance;
    
    public GameObject popupPrefab;

    void Awake()
    {
        Instance = this;
    }

    public void CreatePopup(Vector3 position, int damageAmount, bool isCriticalHit)
    {
        GameObject popupObj = Instantiate(popupPrefab, position, Quaternion.identity);
        
        DamagePopup popupScript = popupObj.GetComponent<DamagePopup>();
        popupScript.Setup(damageAmount, isCriticalHit);
    }
}