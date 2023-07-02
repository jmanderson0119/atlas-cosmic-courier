using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// FlightControls allows the player the pitch, yaw, roll, and throttle in the level
public class FlightControls : MonoBehaviour
{
    [Header("Ship")] // refers to the ship the player is using
    [SerializeField] private GameObject shipBuild;
    private ShipStats shipStats;

    [Header("Input Mode")] // FPS/Flight Simulator selection
    [SerializeField] private InputMode inputMode;
    [SerializeField] private float invertedMouse;

    [Header("Input Deadzones")] // mouse "snap to center" sensitivity
    [SerializeField] private int mouseLook;

    [Header("Input Speeds")] // player movement sensitivities
    [SerializeField] private float throttleSpeed;
    [SerializeField] private float pitchSpeed;
    [SerializeField] private float yawSpeed;
    [SerializeField] private float rollSpeed;

    [Header("Input Accelerations")] // player movement accelerations (see Lerps)
    [SerializeField] private float throttleAcceleration;
    [SerializeField] private float rollAcceleration;

    [Header("Input Data Readout")] // player movement readouts
    [SerializeField] private float throttleIn;
    [SerializeField] private float rollIn;
    [SerializeField] private Vector2 mouseIn;
    private Vector2 lookIn; // mouse input relative to the screen center

    void Start()
    { 
        ShipStats shipStats = shipBuild.GetComponent<ShipStats>();

        inputMode = shipStats.getInputMode();
        invertedMouse = shipStats.getInvertedMouse();
        mouseLook = shipStats.getMouseLook();
        throttleSpeed = shipStats.getThrottleSpeed();
        pitchSpeed = shipStats.getPitchSpeed();
        yawSpeed = shipStats.getYawSpeed();
        rollSpeed = shipStats.getRollSpeed();
        throttleAcceleration = shipStats.getThrottleAcceleration();
        rollAcceleration = shipStats.getRollAcceleration();

        // cursor becomes invisible + confined to the game view
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    void Update()
    {
        // calculate throttle as a lerp between current throttle magnitude and the target throttle input, over the throttle acceleration by frame
        throttleIn = Mathf.Lerp(throttleIn, Input.GetAxisRaw("Throttle") * throttleSpeed, throttleAcceleration * Time.deltaTime);

        // calculate pitch and yaw accoring to player preferences
        if (inputMode == InputMode.FlightSim)
        {
            // how far in the x and y the mouse is from the center of the screen
            lookIn.x = (float) (Input.mousePosition.x - (0.5 * Screen.width));
            lookIn.y = (float) (Input.mousePosition.y - (0.5 * Screen.height));

            // set the direction the mouse is pointing in relative to the center; snap to center if it's close enough
            mouseIn.x = Mathf.Abs(lookIn.x) <= mouseLook ? 0f : (float) ((lookIn.x) / (0.5 * Screen.height));
            mouseIn.y = Mathf.Abs(lookIn.y) <= mouseLook ? 0f : (float) ((lookIn.y) / (0.5 * Screen.height));

            // calculate pitch and yaw as a normalized vector preserving the proportions between their magnitudes
            Vector3.Normalize(mouseIn);
        }
        else
        {
            // calculate pitch and yaw from mouse input tracking
            mouseIn.x = Input.GetAxis("Mouse X");
            mouseIn.y = Input.GetAxis("Mouse Y");
        }

        // calculate roll as a lerp between the current roll magnitude and the target roll input, over the roll acceleration by frame
        rollIn = Mathf.Lerp(rollIn, Input.GetAxisRaw("Roll") * rollSpeed, rollAcceleration * Time.deltaTime);

        // rotate and translate the ship at the corresponding speed by frame
        shipBuild.transform.Rotate(invertedMouse * mouseIn.y * pitchSpeed * Time.deltaTime, mouseIn.x * yawSpeed * Time.deltaTime, rollIn * Time.deltaTime, Space.Self);
        shipBuild.transform.position += shipBuild.transform.forward * throttleIn * Time.deltaTime;
    }
}