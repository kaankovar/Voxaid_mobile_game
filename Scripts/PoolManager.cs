using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    [System.Serializable]
    public struct Pool
    {
        public string tag;
        public GameObject prefab;
        public int initialSize; 
        public int maxSize;     
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;
    private Dictionary<string, GameObject> prefabDictionary; 
    
    public bool isInitialized = false; 
    public float initProgress = 0f; 
    private Transform poolContainer;

   void Awake()
    {
        
            Instance = this;
            
            poolDictionary = new Dictionary<string, Queue<GameObject>>();
            prefabDictionary = new Dictionary<string, GameObject>();
        
    }

    IEnumerator Start() 
    {
        
        GameObject containerGo = new GameObject("PoolContainer");
        containerGo.transform.SetParent(this.transform); 
        poolContainer = containerGo.transform; 

        int totalInitialObjects = 0;
        foreach (Pool p in pools) totalInitialObjects += p.initialSize;
        if (totalInitialObjects == 0) totalInitialObjects = 1; 

        int currentSpawned = 0;
        int batchSize = 8; 

        
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            prefabDictionary.Add(pool.tag, pool.prefab);

            for (int i = 0; i < pool.initialSize; i++)
            {
                GameObject obj = CreatePrewarmedObject(pool.prefab);
                objectPool.Enqueue(obj);
                
                currentSpawned++;
                initProgress = (float)currentSpawned / totalInitialObjects; 
                
                if (i == 0 || i % batchSize == 0) yield return null; 
            }
            poolDictionary.Add(pool.tag, objectPool);
        }
        
        isInitialized = true; 
        
        StartCoroutine(BackgroundPoolFiller());
    }
    
    public void ResetAllPools()
{
    
    foreach (var kvp in poolDictionary)
    {
        Queue<GameObject> queue = kvp.Value;
        int count = queue.Count;

        for (int i = 0; i < count; i++)
        {
            GameObject obj = queue.Dequeue();

            
            if (obj != null) 
            {
                
                if (obj.activeInHierarchy)
                {
                    obj.SetActive(false);
                }

                
                if (obj.transform.parent != poolContainer)
                {
                    obj.transform.SetParent(poolContainer);
                }

                
                queue.Enqueue(obj);
            }
            
        }
    }
}

    private GameObject CreatePrewarmedObject(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, poolContainer);
        if (obj.TryGetComponent(out Animator anim))
        {
            anim.keepAnimatorStateOnDisable = true;
            anim.cullingMode = AnimatorCullingMode.CullCompletely; 
        }
        obj.SetActive(false);
        return obj;
    }

    IEnumerator BackgroundPoolFiller()
    {
        foreach (Pool pool in pools)
        {
            int currentCount = poolDictionary[pool.tag].Count;
            int neededObjects = pool.maxSize - currentCount;

            for (int i = 0; i < neededObjects; i++)
            {
                GameObject obj = CreatePrewarmedObject(pool.prefab);
                poolDictionary[pool.tag].Enqueue(obj);
                yield return null; 
            }
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag)) return null;

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        if (objectToSpawn.activeInHierarchy)
        {
            poolDictionary[tag].Enqueue(objectToSpawn);
            GameObject prefabToSpawn = prefabDictionary[tag];
            objectToSpawn = CreatePrewarmedObject(prefabToSpawn);
        }

        objectToSpawn.transform.SetPositionAndRotation(position, rotation);
        objectToSpawn.SetActive(true);
        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}