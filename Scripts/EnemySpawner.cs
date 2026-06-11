using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public struct EnemyWeight
    {
        public string tag;
        public int weight;
        public int minLevel;
    }

    public EnemyWeight[] enemies;
    
    [Header("Dynamic Spawning")]
    public Transform player;        
    public float minDistance = 10f; 
    public float maxDistance = 15f; 

    [Header("Difficulty")]
    public float baseSpawnRate = 1.8f;
    public float minSpawnRate = 0.2f;
    public float rateDecreasePerLevel = 0.15f;
    public bool isSpawning = true;

    void Start()
    {
        if (player == null && GameObject.FindGameObjectWithTag("Player"))
            player = GameObject.FindGameObjectWithTag("Player").transform;

        
        if (LevelManager.Instance != null && LevelManager.Instance.currentPlanet != null)
        {
            
            baseSpawnRate *= LevelManager.Instance.currentPlanet.spawnRateMultiplier;
        }

        StartCoroutine(SpawnRoutine());
    }

    
    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();
        
        
        GameObject[] activeEnemies = GameObject.FindGameObjectsWithTag("Enemy"); 
        foreach (var enemy in activeEnemies)
        {
            
            enemy.SetActive(false); 
        }
    }

    public void SpawnBoss()
    {
        StopSpawning(); 
        
        if (LevelManager.Instance.currentPlanet.bossPrefab != null)
        {
            
            Vector2 randomCircle = Random.insideUnitCircle.normalized; 
            Vector3 spawnOffset = new Vector3(randomCircle.x, 0, randomCircle.y) * 20f;
            Vector3 spawnPos = player.position + spawnOffset;

            Instantiate(LevelManager.Instance.currentPlanet.bossPrefab, spawnPos, Quaternion.identity);
            
            Debug.Log("BOSS GELDİ!");
        }
    }

    IEnumerator SpawnRoutine()
    {
        
        yield return new WaitUntil(() => PoolManager.Instance != null && PoolManager.Instance.isInitialized);

        while (isSpawning)
        {
            if (player != null) SpawnProgressionEnemy();

            int currentLevel = 1;
            if (LevelManager.Instance != null) currentLevel = LevelManager.Instance.currentLevel;
            
            float currentRate = baseSpawnRate - ((currentLevel - 1) * rateDecreasePerLevel);
            if (currentRate < minSpawnRate) currentRate = minSpawnRate;

            yield return new WaitForSeconds(currentRate);
        }
    }

    void SpawnProgressionEnemy()
    {
        
        
        
        
        
        
        int currentLevel = 1;
        if (LevelManager.Instance != null) currentLevel = LevelManager.Instance.currentLevel;

        List<EnemyWeight> validEnemies = new List<EnemyWeight>();
        int totalWeight = 0;

        foreach (var enemy in enemies)
        {
            if (currentLevel >= enemy.minLevel)
            {
                validEnemies.Add(enemy);
                totalWeight += enemy.weight;
            }
        }

        if (validEnemies.Count == 0 && enemies.Length > 0)
        {
            validEnemies.Add(enemies[0]);
            totalWeight = enemies[0].weight;
        }

        int randomValue = Random.Range(0, totalWeight);
        int cursor = 0;
        string selectedTag = validEnemies[0].tag;

        foreach (var enemy in validEnemies)
        {
            cursor += enemy.weight;
            if (randomValue < cursor)
            {
                selectedTag = enemy.tag;
                break;
            }
        }

        Vector2 randomCircle = Random.insideUnitCircle.normalized; 
        float randomDist = Random.Range(minDistance, maxDistance);
        Vector3 spawnOffset = new Vector3(randomCircle.x, 0, randomCircle.y) * randomDist;
        Vector3 finalPos = player.position + spawnOffset;

        PoolManager.Instance.SpawnFromPool(selectedTag, finalPos, Quaternion.identity);
    }
}