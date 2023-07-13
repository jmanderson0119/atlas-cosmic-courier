using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// obstacle gameplay controller: will pick a random point in front of the player outside their view and launch an object at them
public class DebrisLauncher : MonoBehaviour
{
    [Header("Target Data")]
    [SerializeField] private GameObject ship; // player position reference
    [SerializeField] private FlightControls flightData; // player flight data reference

    [Header("Objects to Lauch")]
    [SerializeField] private List<GameObject> debris; // launchable object references

    // the need to know's: the object to be launched, its rigidbody, its starting location, how its spinning, and how its moving
    private GameObject spawnedObject;
    private Rigidbody rigidBody;
    private Vector2 spawnRelativeToShipXAndY;
    private Vector3 spawnRelativeToShip;
    private Vector3 spawnLocation;
    private Vector3 spawnTorque;
    private Vector3 spawnForce;

    private int objectSpeed;
    private float timeToReceive;
    private Vector3 positionToReceive;

    // launch() to be called every so often once the player is situated in the level
    void Start()
    {
        InvokeRepeating("launch", 10f, Random.Range(3f, 8f));
    }

    void launch()
    {
        Debug.Log("Fire");

        // position: select a random point in a circle placed in front of the player
        spawnRelativeToShipXAndY = Random.insideUnitCircle * 500f;
        spawnRelativeToShip = new Vector3(ship.transform.position.x + spawnRelativeToShipXAndY.x, ship.transform.position.y + spawnRelativeToShipXAndY.y, ship.transform.position.z + 2000f);

        spawnLocation = ship.transform.right * spawnRelativeToShip.x + ship.transform.up * spawnRelativeToShip.y + ship.transform.forward * spawnRelativeToShip.z;

        Debug.Log("(" + spawnLocation.x + ", " + spawnLocation.y + ", " + spawnLocation.z + ")");
        
        // spawn & initialization
        spawnedObject = Instantiate(debris[Random.Range(0, debris.Count)], spawnLocation, Quaternion.identity);
        rigidBody = spawnedObject.GetComponent<Rigidbody>();

        // apply torque & force
        objectSpeed = Random.Range(250, 500);
        timeToReceive = Vector3.Distance(ship.transform.position, spawnedObject.transform.position) / (objectSpeed);
        positionToReceive = ship.transform.position + ship.transform.forward * flightData.getThrottleIn() * timeToReceive;

        rigidBody.AddTorque(Random.onUnitSphere, ForceMode.VelocityChange);
        rigidBody.AddForce((positionToReceive - spawnedObject.transform.position).normalized * objectSpeed, ForceMode.VelocityChange);
        
    }
}
