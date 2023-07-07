using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controller for the camera FOV on change in speed
public class FOVController : MonoBehaviour
{
    [Header("Ship Components")] //ship camera GameObject
    [SerializeField] private GameObject shipView; 
    private ShipStats shipStats; // obtains acceleration for changing FOV based on how much "throttle" is being applied
    private Camera shipCamera;

    [Header("FOV Bounds")] // FOV configuration settings
    [SerializeField] private float fullSpeedFOV;
    [SerializeField] private float restingFOV;



    void Start()
    {
        // obtain camera and ship data
        shipCamera = shipView.GetComponent<Camera>();
        shipStats = GetComponent<ShipStats>();
    }

    void Update()
    {
        // FOV lerps toward upper bound on 'W' and toward lower bound otherwise by the throttle acceleration, scaled to achieve a nice effect
        shipCamera.fieldOfView = Input.GetKey(KeyCode.W) ? Mathf.Lerp(shipCamera.fieldOfView, fullSpeedFOV, 3f * shipStats.getThrottleAcceleration() * Time.deltaTime) : Mathf.Lerp(shipCamera.fieldOfView, restingFOV, 5f * shipStats.getThrottleAcceleration() * Time.deltaTime);
        Debug.Log("FOV: " + shipCamera.fieldOfView);
    }
}
