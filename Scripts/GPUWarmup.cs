using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections;

public class GPUWarmup : MonoBehaviour
{
    [Header("Isıtılacak Ağır Objeler")]
    public GameObject[] heavyPrefabs; 

    IEnumerator Start()
    {
        
        Vector3 hiddenPosition = new Vector3(0, -5000f, 0);

        foreach (GameObject prefab in heavyPrefabs)
        {
            
            GameObject tempObj = Instantiate(prefab, hiddenPosition, Quaternion.identity);

            
            yield return null;
            yield return null;

            
            Destroy(tempObj);
        }

        
        
        SceneManager.LoadScene("MainMenu"); 
    }
}