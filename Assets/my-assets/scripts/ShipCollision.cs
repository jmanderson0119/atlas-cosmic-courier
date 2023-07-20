using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// handles collisions that involve the player (damage dealt, physics behavior, ui changes, etc.)
public class ShipCollision : MonoBehaviour
{
    [SerializeField] private GameObject playerCamera; // referencing for the CameraShake script for visual cue
    [SerializeField] private HealthManager healthManager; // player health data
    private AimAssist aimAssistManager; // player aim assist reference 
    private CollisionCamShake shaker; // referencing for the shake() function for visual cue
    private FlightControls flightData; // reference to throttle data

    // initialization
    void Start()
    {
        shaker = playerCamera.GetComponent<CollisionCamShake>();
        flightData = GameObject.Find("scene-manager").GetComponent<FlightControls>();
        healthManager = GetComponent<HealthManager>();
        aimAssistManager = AimAssist.aimAssist;
        
    }

    void OnTriggerEnter(Collider collision)
    {
        // collision handler for ship-debris action: adds a force + torque to debris, shakes camera, breaks shield/blows up ship
        if (collision.gameObject.CompareTag("DebrisCollision") && Vector3.Distance(transform.position, collision.gameObject.transform.position) < 10f)
        {
            // collision handling
            collision.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * flightData.getThrottleIn() * 100f , ForceMode.Force);
            collision.gameObject.GetComponent<Rigidbody>().AddTorque(transform.forward * flightData.getThrottleIn() * 100f, ForceMode.Force);
            
            // visual cues
            StartCoroutine(shaker.collisionShake());
            healthManager.debrisCollision();
        }
        else if (collision.gameObject.CompareTag("DebrisCollision") && Vector3.Distance(transform.position, collision.gameObject.transform.position) >= 10f)
        {
            aimAssistManager.AimAssistEngage();
        }
    }

    void OnTriggerExit(Collider collision)
    {
        // the aim assist will only be disengaged if the debris is exiting the aim assist field
        if (Vector3.Distance(transform.position, collision.gameObject.transform.position) > 10f)
        {
            aimAssistManager.AimAssistDisengage();
        }
    }
}
