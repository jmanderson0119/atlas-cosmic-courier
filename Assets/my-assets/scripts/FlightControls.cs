using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// FlightControls allows the player the pitch, yaw, and roll in the level
public class FlightControls : MonoBehaviour
{

    [SerializeField] private Rigidbody playerBody; // player's rb

    [SerializeField] private float pitchSpeed; // vertical sensitivity
    [SerializeField] private float yawSpeed; // horizontal sensitivity
    [SerializeField] private float rollSpeed; // roll sensitivity

    [SerializeField] private float acceleration; // Acceleration rate
    [SerializeField] private float deceleration; // Deceleration rate
    [SerializeField] private float maxSpeed; // Maximum speed

    private float currentSpeed = 0f; // Current speed
    private float pitchIn; // player pitch input
    private float yawIn; // player yaw input
    private float rollIn; // player roll input
    private float throttleIn; // player throttle input
    private Quaternion rotation; // player rotation update quaternion


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // hide the cursor and center it upon entering the level
    }


    void Update()
    {
        /*
     	* This is the pitch/yaw/roll mechanic. It reads, by frame, the mouse movement, which is then used
     	* to calculate a quaternion. This quaternion describes a rotation about the ship's axes based on the mouse sensitivity,
     	* the amount of mouse movement, and time between frames. This is then multiplied by the player's current orientation
     	* to concatenate them and yield the updated orientation:
     	*/

        pitchIn = Input.GetAxis("Mouse Y"); // step 1.1: get vertical mouse input
        yawIn = Input.GetAxis("Mouse X"); // step 1.2: get horizontal mouse input

        // step 1.3: get the 'A' and 'D' key input
        if (Input.GetKey(KeyCode.A))
        {
            rollIn = 1f; // rotate CCW
        }

        if (Input.GetKey(KeyCode.D))
        {
            rollIn = -1f; // rotate CW
        }

        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            rollIn = 0f;
        }

        rotation = Quaternion.Euler(-pitchIn * pitchSpeed * Time.deltaTime, yawIn * yawSpeed * Time.deltaTime, rollIn * rollSpeed * Time.deltaTime); // step 2: calculate the rotation update quaternion

        transform.rotation *= rotation; // step 3: rotate the ship

        /*
     	* This is the throttle mechanic. It reads the keybind activity of 'W' and 'S' and applies a constant acceleration with consideration
     	* to max speed of the ship, the acceleration and deceleration set on the ship, and the time between frames.
     	*/

        throttleIn = Input.GetAxis("Vertical"); // Get throttle input

        if (throttleIn > 0f)
        {
            // Accelerate the ship
            currentSpeed += acceleration * Time.deltaTime;

            // Limit the speed to the maximum speed
            currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);
        }
        else if (throttleIn < 0f)
        {
            // Decelerate the ship
            currentSpeed -= deceleration * Time.deltaTime;

            // Limit the speed to zero
            currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);
        }

        // Move the ship forward based on the current speed
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }
}
