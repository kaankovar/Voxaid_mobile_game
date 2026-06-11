using TMPro;
using UnityEngine;

public class GoldPickup : MonoBehaviour
{
    public int goldAmount = 10;
     public float moveSpeed = 10f;
    private Transform target;
    private bool isCollected = false;
    void FixedUpdate()
    {
        transform.Rotate(0,0,2f);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int gold = PlayerHealth.Instance.gold; 
            AudioManager.Instance.PlaySFX(SoundType.PickupGold);
            int pi = DataManager.SelectedPlanetIndex;
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.AddGold(goldAmount * pi);
            }
            else
            {
                DataManager.AddGold(goldAmount * pi);
            }
            gold += goldAmount * pi;
            PlayerHealth.Instance.gold = gold;
            PlayerHealth.Instance.goldtext.text = gold.ToString();
            gameObject.SetActive(false);
        }
    }
    void OnEnable()
    {
        isCollected = false;
        target = null;
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
}