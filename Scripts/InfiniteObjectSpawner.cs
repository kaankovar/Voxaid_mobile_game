using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InfiniteObjectSpawner : MonoBehaviour
{
    [Header("Ayarlar")]
    public Transform player;
    public float chunkSize = 20f; 
    public int viewDistance = 2;  
    public float updateRate = 0.5f; 

    [Header("Varsayılan Tasarımlar")]
    public GameObject[] tilePrefabs; 

    
    private Dictionary<Vector2Int, GameObject> activeChunks = new Dictionary<Vector2Int, GameObject>();
    
    
    
    private Queue<GameObject> chunkPool = new Queue<GameObject>();

    private Vector2Int lastChunkCoord;
    private bool isSpawning = false; 
    
    public static InfiniteObjectSpawner Instance;
    public bool isInitialized = false; 

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        if (player == null && PlayerHealth.Instance != null) 
        {
            player = PlayerHealth.Instance.transform;
        }

        if (LevelManager.Instance != null && LevelManager.Instance.currentPlanet != null)
        {
            GameObject[] planetTiles = LevelManager.Instance.currentPlanet.planetTilePrefabs;
            if (planetTiles != null && planetTiles.Length > 0)
            {
                tilePrefabs = planetTiles;
                ClearPool(); 
            }
        }

        
        StartCoroutine(InitialSpawn());
    }

    IEnumerator InitialSpawn()
    {
        
        if (PoolManager.Instance != null)
        {
            while (!PoolManager.Instance.isInitialized) yield return null;
        }
        
        
        yield return null; 
        yield return null;

        isSpawning = true;
        
        Vector2Int currentChunk = new Vector2Int(
             Mathf.RoundToInt(player.position.x / chunkSize),
             Mathf.RoundToInt(player.position.z / chunkSize)
         );
        lastChunkCoord = currentChunk;

        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int y = -viewDistance; y <= viewDistance; y++)
            {
                Vector2Int targetCoord = lastChunkCoord + new Vector2Int(x, y);
                if (!activeChunks.ContainsKey(targetCoord))
                {
                    
                    
                    yield return null; 
                    CreateNewChunk(targetCoord);
                }
            }
        }
        
        isSpawning = false;
        isInitialized = true;
        StartCoroutine(CheckChunksRoutine());
    }

    
    IEnumerator CheckChunksRoutine()
    {
        while (true)
        {
            if (player != null && !isSpawning)
            {
                Vector2Int currentChunkCoord = new Vector2Int(
                    Mathf.RoundToInt(player.position.x / chunkSize),
                    Mathf.RoundToInt(player.position.z / chunkSize)
                );

                if (currentChunkCoord != lastChunkCoord)
                {
                    lastChunkCoord = currentChunkCoord;
                    
                    yield return StartCoroutine(UpdateChunksOptimizedRoutine());
                }
            }
            yield return new WaitForSeconds(updateRate);
        }
    }

    IEnumerator UpdateChunksOptimizedRoutine() 
    {
        
        List<Vector2Int> chunksToRemove = new List<Vector2Int>();
        foreach (var item in activeChunks)
        {
            if (Mathf.Abs(item.Key.x - lastChunkCoord.x) > viewDistance ||
                Mathf.Abs(item.Key.y - lastChunkCoord.y) > viewDistance)
            {
                chunksToRemove.Add(item.Key);
            }
        }

        foreach (var coord in chunksToRemove)
        {
            GameObject objToRemove = activeChunks[coord];
            objToRemove.SetActive(false);
            chunkPool.Enqueue(objToRemove);
            activeChunks.Remove(coord);
        }

        
        int spawnCounter = 0;
        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int y = -viewDistance; y <= viewDistance; y++)
            {
                Vector2Int targetCoord = lastChunkCoord + new Vector2Int(x, y);

                if (!activeChunks.ContainsKey(targetCoord))
                {
                    SpawnChunk(targetCoord);
                    spawnCounter++;

                    
                    if (spawnCounter % 2 == 0) yield return null; 
                }
            }
        }
    }

    void SpawnChunk(Vector2Int coord)
    {
        Vector3 spawnPosition = new Vector3(coord.x * chunkSize, 0, coord.y * chunkSize);

        
        if (chunkPool.Count > 0)
        {
            
            GameObject recycledChunk = chunkPool.Dequeue();
            
            recycledChunk.transform.position = spawnPosition;
            recycledChunk.transform.rotation = Quaternion.identity;
            recycledChunk.SetActive(true);
            
            activeChunks.Add(coord, recycledChunk);
        }
        else
        {
            
            CreateNewChunk(coord);
        }
    }

    void CreateNewChunk(Vector2Int coord)
    {
        if (tilePrefabs == null || tilePrefabs.Length == 0) return;

        int randomIndex = Random.Range(0, tilePrefabs.Length);
        GameObject prefab = tilePrefabs[randomIndex];
        Vector3 spawnPosition = new Vector3(coord.x * chunkSize, 0, coord.y * chunkSize);

        GameObject newChunk = Instantiate(prefab, spawnPosition, Quaternion.identity);
        newChunk.transform.SetParent(transform);
        
        activeChunks.Add(coord, newChunk);
    }

    
    void ClearPool()
    {
        foreach (GameObject obj in chunkPool)
        {
            Destroy(obj);
        }
        chunkPool.Clear();
    }
}