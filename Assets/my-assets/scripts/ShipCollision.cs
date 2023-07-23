using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// handles collisions that involve the player (damage dealt, physics behavior, ui changes, etc.)
public class ShipCollision : MonoBehaviour
{
    [Header("Ship Components")]
    [SerializeField] private GameObject playerCamera; // referencing for the CameraShake script for visual cue
    [SerializeField] private GameObject shipFront; // relevant ref for player location relative to debris
    [SerializeField] private GameObject aimAssistField; // for handling aim assist behavior

    [Header("Health Data")]
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
        // accounts for both whether collision occurs from aim assist field or the ship body itself
        if (collision.gameObject.CompareTag("DebrisCollision") && Vector3.Distance(collision.ClosestPointOnBounds(aimAssistField.transform.position), shipFront.transform.position) <= 2.2)
        {
            // collision handling
            collision.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * flightData.getThrottleIn() * 100f , ForceMode.Force);
            collision.gameObject.GetComponent<Rigidbody>().AddTorque(transform.forward * flightData.getThrottleIn() * 100f, ForceMode.Force);
            
            // visual cues
            StartCoroutine(shaker.collisionShake());
            healthManager.debrisCollision();
        }
        // case where debris collides with aim assist field
        else if(collision.gameObject.CompareTag("DebrisCollision") && Vector3.Distance(collision.ClosestPointOnBounds(aimAssistField.transform.position), shipFront.transform.position) > 2.2)
        {
            aimAssistManager.AimAssistEngage();
        }
    }
    // disengage AA upon object leaving AA field
    void OnTriggerExit(Collider collision)
    {
        aimAssistManager.AimAssistDisengage();
    }
}
