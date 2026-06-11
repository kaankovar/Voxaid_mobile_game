using UnityEngine;

[CreateAssetMenu(fileName = "NewPlanet", menuName = "Game/Planet Data")]
public class PlanetData : ScriptableObject
{
    [Header("Identity")]
    public string planetName;
    public int planetIndex;
    public Sprite planetIcon;
    public string sceneName = "GameScene";

    [Header("Visuals")]
    
    public Sprite groundSprite; 
    
    
    public GameObject[] planetTilePrefabs; 

    [Header("Difficulty")]
    public GameObject bossPrefab;
    public float enemyHealthMultiplier = 1f;
    public float enemyDamageMultiplier = 1f;
    public float spawnRateMultiplier = 1f;
    public float xpRequirementMultiplier = 1f;
}