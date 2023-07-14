using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * obstacle gameplay controller: will pick a random point in front of the player outside their view and launch an object at them
 * uses pooling to recycle due to larger scale, similar to ObjectPooler.cs
 */
public class DebrisLauncher : MonoBehaviour
{
    [Header("Target Data")]
    [SerializeField] private GameObject ship; // player position reference
    [SerializeField] private FlightControls flightData; // player flight data reference

    [Header("Objects to Lauch")]
    [SerializeField] private List<Pool> debris; // list of pools to draw debris types from
    private Dictionary<string, Queue<GameObject>> debrisDictionary = new Dictionary<string, Queue<GameObject>>();

    // logistics of launch: object, its accuracy, its RB, position relative to ship, spawn location in world space, launch direction
    private GameObject spawnedObject;
    private int objectAccuracy;
    private Rigidbody rigidBody;
    private Vector3 spawnRelativeToShip;
    private Vector3 spawnLocation;
    private Vector3 positionToReceive;

    // name of pool, what's "in the pool", number of objects in the pool
    [System.Serializable]
    public class Pool
    {
        public string poolTag;
        public GameObject debrisPrefab;
        public int poolSize;
    }

    // launch() to be called every so often once the player is situated in the level
    void Start()
    {
        InvokeRepeating("launch", 1f, 0.25f);

        // intializing debris pools
        foreach (Pool pool in debris)
        {
            Queue<GameObject> debrisPool = new Queue<GameObject>();

            for (int i = 0; i < pool.poolSize; i++)
            {
                GameObject debrisPrefab = Instantiate(pool.debrisPrefab);
                debrisPrefab.SetActive(false);
                debrisPool.Enqueue(debrisPrefab);
            }

            debrisDictionary.Add(pool.poolTag, debrisPool);
        }
    }

    // guesses the position of the targeted object and launches a piece of debris in that direction
    void launch()
    {
        // position: select a random point in a circle placed in front of the player
        spawnRelativeToShip = Random.insideUnitCircle * 1420f;
        spawnLocation = ship.transform.position + ship.transform.right * spawnRelativeToShip.x + ship.transform.up * spawnRelativeToShip.y + ship.transform.forward * 3000f;

        // spawn & initialization
        Pool randomPool = debris[Random.Range(0, debris.Count)];

        spawnedObject = debrisDictionary[randomPool.poolTag].Dequeue();
        spawnedObject.SetActive(true);

        spawnedObject.transform.position = spawnLocation;
        rigidBody = spawnedObject.GetComponent<Rigidbody>();
        objectAccuracy = Random.Range(1, 21); // assign an accuracy rating

        // 80% of the time - accurate, 10% - fairly accurate, 5% - somewhat accurate, 5% - somewhat inaccurate
        // following numbers were carefully calculated values, if you add or subtract to one value, you must adjust the other three by the same amount
        if (objectAccuracy <= 16)
        {
            positionToReceive = ship.transform.position + ship.transform.forward * 1332.8125f + Random.insideUnitSphere * 67.1875f;
        }
        else if (objectAccuracy <= 18)
        {
            positionToReceive = ship.transform.position + ship.transform.forward * 1255.625f + Random.insideUnitSphere * 134.375f;
        }
        else if (objectAccuracy == 19)
        {
            positionToReceive = ship.transform.position + ship.transform.forward * 1131.25f + Random.insideUnitSphere * 268.75f;
        }
        else
        {
            positionToReceive = ship.transform.position + ship.transform.forward * 862.5f + Random.insideUnitSphere * 537.5f;
        }

        // calculate appropriate force & torque
        rigidBody.AddTorque(Random.onUnitSphere, ForceMode.VelocityChange);
        rigidBody.AddForce((positionToReceive - spawnedObject.transform.position).normalized * 350, ForceMode.VelocityChange);

        // spawned object is added back to pool for future spawn
        debrisDictionary[randomPool.poolTag].Enqueue(spawnedObject);
    }
}
