using UnityEngine;
using UnityEngine.UI;

public class BossIndicator : MonoBehaviour
{
    [Header("Ayarlar")]
    public RectTransform pointerRectTransform;
    public float borderPadding = 50f;

    private Transform bossTransform;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        if (pointerRectTransform != null) pointerRectTransform.gameObject.SetActive(false);
    }

    void Update()
    {

        if (bossTransform == null)
        {
            GameObject boss = GameObject.FindGameObjectWithTag("Boss");
            if (boss != null)
            {
                bossTransform = boss.transform;
            }
            else
            {

                if (pointerRectTransform.gameObject.activeSelf)
                    pointerRectTransform.gameObject.SetActive(false);
                return;
            }
        }


        Vector3 screenPos = mainCamera.WorldToScreenPoint(bossTransform.position);


        bool isOffScreen = screenPos.x <= 0 || screenPos.x >= Screen.width ||
                           screenPos.y <= 0 || screenPos.y >= Screen.height || screenPos.z < 0;

        if (isOffScreen)
        {
            if (!pointerRectTransform.gameObject.activeSelf)
                pointerRectTransform.gameObject.SetActive(true);


            if (screenPos.z < 0)
            {
                screenPos *= -1;
            }


            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);


            Vector3 direction = (screenPos - screenCenter).normalized;


            screenPos.x = Mathf.Clamp(screenPos.x, borderPadding, Screen.width - borderPadding);
            screenPos.y = Mathf.Clamp(screenPos.y, borderPadding, Screen.height - borderPadding);
            pointerRectTransform.position = screenPos;


            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;


            pointerRectTransform.localEulerAngles = new Vector3(0, 0, angle - 90f);
        }
        else
        {

            if (pointerRectTransform.gameObject.activeSelf)
                pointerRectTransform.gameObject.SetActive(false);
        }
    }
}