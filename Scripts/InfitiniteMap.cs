using UnityEngine;

public class InfiniteMap : MonoBehaviour
{
    public Transform target; 
    
    [Header("Ayarlar")]
    public float textureSize = 10f; 
    public Vector2 tiling = new Vector2(100, 100); 
    
    private Renderer rend;
    private Vector3 offset;

    void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    void Start()
    {
        if(AudioManager.Instance != null) AudioManager.Instance.PlayMusic(SoundType.GameMusic);
        
        
        if (target == null && PlayerHealth.Instance != null) 
        {
            target = PlayerHealth.Instance.transform;
        }
        
        if(target != null)
            offset = transform.position - target.position;

        ApplyPlanetTexture();
    }

    void ApplyPlanetTexture()
    {
        if (LevelManager.Instance != null && LevelManager.Instance.currentPlanet != null)
        {
            Sprite planetSprite = LevelManager.Instance.currentPlanet.groundSprite;

            if (planetSprite != null && rend != null)
            {
                
                rend.material.mainTexture = planetSprite.texture;
                
                
                rend.material.mainTextureScale = tiling;

                
                if (rend.material.mainTexture != null)
                {
                    rend.material.mainTexture.wrapMode = TextureWrapMode.Repeat;
                }
                rend.material.color = new Color32(200, 200, 200, 255);
                Debug.Log("Zemin Resmi ve Tiling Ayarlandı: " + planetSprite.name);
            }
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        
        Vector3 newPos = target.position + offset;
        newPos.y = transform.position.y;
        
        transform.position = newPos;

        
        float offsetX = transform.position.x / textureSize;
        float offsetZ = transform.position.z / textureSize;
        
        
        rend.material.mainTextureOffset = new Vector2(-offsetX, -offsetZ);
    }
}