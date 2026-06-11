using UnityEngine;
using System.Collections;

public class LightningVisual : MonoBehaviour
{
    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void Setup(Vector3 startPos, Vector3 endPos)
    {
        AudioManager.Instance.PlaySFX(SoundType.ChainLightning);
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
        StartCoroutine(DestroyRoutine());
    }

    IEnumerator DestroyRoutine()
    {
        yield return new WaitForSeconds(0.1f); 
        Destroy(gameObject); 
    }
}