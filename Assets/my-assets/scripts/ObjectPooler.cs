using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// object pool manager for all debris types + player bullets
public class ObjectPooler : MonoBehaviour
{
    [Header("Ship + Debris")]
    [SerializeField] private GameObject ship; // needed for accessing positional data
    [SerializeField] private GameObject playerCamera; // referencing for shoot() behavior
    [SerializeField] private List<Pool> pools; // collection of pools for config within Unity inspector

    private Dictionary<string, Queue<GameObject>> poolDictionary; // referencing for all pools by their ID
    private bool inHistory = false; // for activating/deactivating spawn based on location
    public static ObjectPooler Instance; // singleton, used for accessing spawn()

    // configuration data for all pools: name, debris object, and number of this debris that may be spawned at once
    [System.Serializable]
    public class Pool
    {
        public string identifier;
        public GameObject prefab;
        public int size;
    }

    public void setInHistory(bool inHistory) => this.inHistory = inHistory; // setter for the inHistory bool

    // initialize singleton
    private void Awake()
    {
        Instance = this;
    }

    // set up a Queue representation for every object pool using the config data from each Pool type
    void Start()
    {
        // initialize dictionary for pool reference later
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // intialize queue for each pool, load it with the specified prefab objects until pool.size is achieved
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            // prefabs are instantiated, set inactive, then queued
            for (int i = 0; i < pool.size; i++)
            {
                GameObject poolPrefab = Instantiate(pool.prefab);
                poolPrefab.SetActive(false);
                objectPool.Enqueue(poolPrefab);
            }

            // pool added to dictionary for later reference
            poolDictionary.Add(pool.identifier, objectPool);
        }
    }

    // will choose a random pool, dequeue an object, set active, then randomly position and rotate within the scene; enqueued for future use.
    public void spawn(int spawnNum)
    {
        if (!inHistory)
        {
            // spawns as many as desire per frame; I chose three because I liked the frequency of generation achieved
            for (int i = 0; i < spawnNum; i++)
            {
                Pool randomPool = pools[Random.Range(0, pools.Count - 1)]; // select random pool
                GameObject spawnedObject = poolDictionary[randomPool.identifier].Dequeue(); // dequeue object

                spawnedObject.SetActive(true); // "instantiate"

                spawnedObject.transform.position = Random.onUnitSphere.normalized * 2800 + ship.transform.position; // random position
                spawnedObject.transform.Rotate(Random.Range(0, 179), Random.Range(0, 179), Random.Range(0, 179)); // random rotation

                poolDictionary[randomPool.identifier].Enqueue(spawnedObject); // place back it queue for future "instantiation"
            }
        }
    }

    public IEnumerator shoot()
    {
        // get reference to bullet object and its RB
        if (poolDictionary["bulletPrefab"].Count != 0)
        {
            GameObject bullet = poolDictionary["bulletPrefab"].Dequeue();
            Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();

            // set position and rotation relative to player display
            bullet.transform.position = playerCamera.transform.position + playerCamera.transform.forward * 50f;
            bullet.transform.rotation = playerCamera.transform.rotation * Quaternion.Euler(90f, 0f, 0f);

            // visual cue
            bullet.SetActive(true);
            
            // fire
            bulletRB.AddForce(playerCamera.transform.forward * 3000f, ForceMode.VelocityChange);

            // wait until the bullet is out of the player's view
            yield return new WaitForSeconds(4f);

            // "unspawn" bullet
            bullet.SetActive(false);

            // remove force/inertial effects
            bulletRB.velocity = Vector3.zero;
            bulletRB.angularVelocity = Vector3.zero;

            // requeue for future use
            poolDictionary["bulletPrefab"].Enqueue(bullet);
        }
    }
}