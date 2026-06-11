using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class BootManager : MonoBehaviour
{
    [Header("Görsel Ayarlar")]
    public CanvasGroup logoGroup;
    public float fadeSpeed = 1.5f;
    public string nextSceneName = "MainMenu";

    void Start()
    {

        StartCoroutine(BootSequence());
    }

    IEnumerator BootSequence()
    {

        float timer = 0f;
        while (timer < fadeSpeed)
        {
            timer += Time.deltaTime;
            logoGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeSpeed);
            yield return null;
        }
        logoGroup.alpha = 1f;

        yield return new WaitForSeconds(0.5f);


        timer = 0f;
        while (timer < fadeSpeed)
        {
            timer += Time.deltaTime;
            logoGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeSpeed);
            yield return null;
        }
        logoGroup.alpha = 0f;


        SceneManager.LoadScene(nextSceneName);
    }
}