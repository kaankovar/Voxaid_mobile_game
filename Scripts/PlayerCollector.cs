using TMPro;
using UnityEngine;

public class PlayerCollector : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ExperienceGem>(out ExperienceGem gem))
        {
            gem.StartCollecting(transform);
        }
        else if (other.TryGetComponent<GoldPickup>(out GoldPickup gpu))
        {
            gpu.StartCollecting(transform);
        }
        else if (other.TryGetComponent<HealthPickup>(out HealthPickup hpu))
        {
            hpu.StartCollecting(transform);
        }
    }
}