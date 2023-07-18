using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// handles collisions that involve the player (damage dealt, physics behavior, ui changes, etc.)
public class ShipCollision : MonoBehaviour
{
    [SerializeField] private GameObject playerCamera; // referencing for the CameraShake script for visual cue
    [SerializeField] private HealthManager healthManager; // player health data
    private CollisionCamShake shaker; // referencing for the shake() function for visual cue
    private FlightControls flightData; // reference to throttle data

    // initialization
    void Start()
    {
        shaker = playerCamera.GetComponent<CollisionCamShake>();
        flightData = GameObject.Find("scene-manager").GetComponent<FlightControls>();
        healthManager = GetComponent<HealthManager>();
    }

    void OnTriggerEnter(Collider collision)
    {
        // collision handler for ship-debris action: adds a force + torque to debris, shakes camera, breaks shield/blows up ship
        if (collision.gameObject.CompareTag("DebrisCollision"))
        {
            // collision handling
            collision.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * flightData.getThrottleIn() * 100f , ForceMode.Force);
            collision.gameObject.GetComponent<Rigidbody>().AddTorque(transform.forward * flightData.getThrottleIn() * 100f, ForceMode.Force);
            
            // visual cues
            StartCoroutine(shaker.collisionShake());
            healthManager.debrisCollision();
        }
    }
}
