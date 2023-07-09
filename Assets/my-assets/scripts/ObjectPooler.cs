/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField] private GameObject ship;
    private Dictionary<string, Queue<GameObject>> poolDictionary;
    [SerializeField] private List<Pool> pools;

    [System.Serializable]
    public class Pool
    {
        public string identifier;
        public GameObject prefab;
        public int size;
        public bool isVisible;
    }

    public static ObjectPooler Instance;

    public List<Pool> getPools() => pools;

    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject poolPrefab = Instantiate(pool.prefab);
                poolPrefab.SetActive(false);
                objectPool.Enqueue(poolPrefab);
            }

            poolDictionary.Add(pool.identifier, objectPool);
        }
    }

    public void spawn(string[] excludedIdentifiers)
    {
        List<Pool> viablePools = new List<Pool>();

        foreach (Pool pool in viablePools)
        {
            if (!pool.isVisible && !IsExcludedIdentifier(pool.identifier, excludedIdentifiers))
            {
                viablePools.Add(pool);
            }
        }

        if (viablePools.Count > 0)
        {
            Pool randomPool = viablePools[Random.Range(0, viablePools.Count)];
            GameObject spawnedObject = poolDictionary[randomPool.identifier].Dequeue();

            spawnedObject.SetActive(true);

            spawnedObject.transform.position = Random.insideUnitSphere.normalized * 1500f + ship.transform.position;
            spawnedObject.transform.Rotate(Random.Range(0, 179), Random.Range(0, 179), Random.Range(0, 179));

            poolDictionary[randomPool.identifier].Enqueue(spawnedObject);
        }
    }

    public bool IsExcludedIdentifier(string identifier, string[] excludedIdentifiers)
    {
        foreach (string excludedIdentifier in excludedIdentifiers)
        {
            if (identifier == excludedIdentifier)
            {
                return true;
            }
        }

        return false;
    }
}
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField] private GameObject ship;
    private Dictionary<string, Queue<GameObject>> poolDictionary;
    [SerializeField] private List<Pool> pools;

    public static ObjectPooler Instance;

    public List<Pool> GetPools() => pools;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject poolPrefab = Instantiate(pool.prefab);
                poolPrefab.SetActive(false);
                objectPool.Enqueue(poolPrefab);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public void Spawn(string[] excludedTags)
    {
        List<Pool> availablePools = new List<Pool>();

        foreach (Pool pool in pools)
        {
            if (!pool.isVisible && !IsExcludedTag(pool.tag, excludedTags))
            {
                availablePools.Add(pool);
            }
        }

        if (availablePools.Count > 0)
        {
            Pool randomPool = availablePools[Random.Range(0, availablePools.Count)];
            GameObject spawnedObject = poolDictionary[randomPool.tag].Dequeue();

            spawnedObject.SetActive(true);

            spawnedObject.transform.position = Random.insideUnitSphere.normalized * 1500f + ship.transform.position;
            spawnedObject.transform.rotation = Random.rotation;

            poolDictionary[randomPool.tag].Enqueue(spawnedObject);
        }
    }

    private bool IsExcludedTag(string tag, string[] excludedTags)
    {
        foreach (string excludedTag in excludedTags)
        {
            if (tag == excludedTag)
            {
                return true;
            }
        }

        return false;
    }

    public Pool GetParentPoolComponent(GameObject gameObject)
    {
        Transform parent = gameObject.transform.parent;
        Pool pool = null;

        while (parent != null)
        {
            pool = parent.GetComponent<Pool>();
            if (pool != null)
                break;

            parent = parent.parent;
        }

        return pool;
    }

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
        public bool isVisible;
    }
}

