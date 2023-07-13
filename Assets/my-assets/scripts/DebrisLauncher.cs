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

    // logistics of launch: object, its accuracy, its RB, position relative to ship, spawn location in world space
    private GameObject spawnedObject;
    private int objectAccuracy;
    private Rigidbody rigidBody;
    private Vector3 spawnRelativeToShip;
    private Vector3 spawnLocation;

    // connection logistics: speed of debris, time until reception, position at reception
    private Vector3 positionToReceive;

    // launch() to be called every so often once the player is situated in the level
    void Start()
    {
        InvokeRepeating("launch", 2f, Random.Range(1f, 6f));
    }

    // guesses the position of the targeted object and launches a piece of debris in that direction
    void launch()
    {
        Debug.Log("Fire");

        // position: select a random point in a circle placed in front of the player
        spawnRelativeToShip = Random.insideUnitCircle * 500f;
        spawnLocation = ship.transform.position + ship.transform.right * spawnRelativeToShip.x + ship.transform.up * spawnRelativeToShip.y + ship.transform.forward * 2000f;

        // spawn & initialization
        spawnedObject = Instantiate(debris[Random.Range(0, debris.Count)], spawnLocation, Quaternion.identity);
        rigidBody = spawnedObject.GetComponent<Rigidbody>();

        objectAccuracy = Random.Range(1, 21); // assign an accuracy rating


        // 80% of the time - accurate, 10% - fairly accurate, 5% - somewhat accurate, 5% - somewhat inaccurate
        if (objectAccuracy <= 16)
        {
            positionToReceive = ship.transform.position + ship.transform.forward * 2082.8125f + Random.insideUnitSphere * 67.1875f;
        }
        else if (objectAccuracy <= 18)
        {
            positionToReceive = ship.transform.position + ship.transform.forward * 2015.625f + Random.insideUnitSphere * 134.375f;
        }
        else if (objectAccuracy == 19)
        {
            positionToReceive = ship.transform.position + ship.transform.forward * 1881.25f + Random.insideUnitSphere * 268.75f;
        }
        else
        {
            positionToReceive = ship.transform.position + ship.transform.forward * 1612.5f + Random.insideUnitSphere * 537.5f;
        }

        // calculate appropriate force & torque
        rigidBody.AddTorque(Random.onUnitSphere, ForceMode.VelocityChange);
        rigidBody.AddForce((positionToReceive - spawnedObject.transform.position).normalized * 350, ForceMode.VelocityChange);
    }
}
