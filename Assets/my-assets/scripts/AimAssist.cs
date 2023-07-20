using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// upon debris/targetables entering the aim assist field attached to the front of the player-display, 
// the pitch/yaw speed will be altered to decrease look sensitivity, increasing player accuracy
public class AimAssist : MonoBehaviour
{
    [Header("Flight Data")]
    [SerializeField] private FlightControls flightData;

    [Header("Aim Assist Strength")]
    [SerializeField] private float adjustedPitchSpeed;
    [SerializeField] private float adjustedYawSpeed;
    
    // saves the pitch and yaw for on exit() behavior
    private float defaultPitchSpeed;
    private float defaultYawSpeed;

    // saves how many targets are in the AA field currently
    private int objectsToTarget;

    #region AimAssist
    public static AimAssist aimAssist; // singleton for accessing AA methods
    #endregion

    // initialization
    void Start()
    {
        flightData = GameObject.Find("scene-manager").GetComponent<FlightControls>();
        
        defaultPitchSpeed = flightData.getPitchSpeed();
        defaultYawSpeed = flightData.getYawSpeed();

        aimAssist = this;
    }

    // set new pitch and yaw sensitivity upon target entering AA field
    public void AimAssistEngage()
    {
        Debug.Log("AA engaged");

        objectsToTarget++;

        if (objectsToTarget > 0)
        {
            flightData.setPitchSpeed(adjustedPitchSpeed);
            flightData.setYawSpeed(adjustedYawSpeed);
        }
    }

    // reset pitch and yaw sensitivity upon target leaving AA field
    public void AimAssistDisengage()
    {
        Debug.Log("AA disengaged");

        objectsToTarget--;

        if (objectsToTarget == 0)
        {
            flightData.setPitchSpeed(defaultPitchSpeed);
            flightData.setYawSpeed(defaultYawSpeed);
        }
    }
}
