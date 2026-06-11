using UnityEngine;

public class ExperienceGem : MonoBehaviour
{
    public int xpAmount = 10;
    public float moveSpeed = 10f;
    
    private Transform target;
    private bool isCollected = false;

    void OnEnable()
    {
        isCollected = false;
        target = null;
        LevelManager.OnBossSpawned += DisableSelf;
    }
    void OnDisable()
    {
        
        
        LevelManager.OnBossSpawned -= DisableSelf;
    }
    void DisableSelf()
    {
        
        gameObject.SetActive(false);
        
        
        
    }
    void Update()
    {
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            moveSpeed += 5f * Time.deltaTime; 
        }
    }

    public void StartCollecting(Transform playerTransform)
    {
        if (!isCollected)
        {
            isCollected = true;
            target = playerTransform;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance.PlaySFX(SoundType.PickupGem);
            LevelManager.Instance.AddExperience(xpAmount);
            gameObject.SetActive(false); 
        }
    }
}