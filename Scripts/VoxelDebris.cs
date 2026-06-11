using UnityEngine;

public class VoxelDebris : MonoBehaviour
{
    public float lifeTime = 1.5f;
    private float disableTime;

    void OnEnable()
    {
        disableTime = Time.time + lifeTime;
    }

    void Update() 
    {
        if (Time.time >= disableTime) gameObject.SetActive(false);
    }
}