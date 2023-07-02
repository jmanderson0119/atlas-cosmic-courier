using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores ship configuration settings for the default ship option
public class ShipStats : MonoBehaviour
{
    /*
     * FlightSim
     * 75
     * 150
     * 40
     * 30
     * 110
     * 0.8
     * 3.5
     */
    [Header("Input Mode")] // mouse "snap to center" sensitivity
    [SerializeField] private InputMode inputModeConfig;
    [SerializeField] private float invertedMouseConfig;

    [Header("Input Deadzones")] // player movement sensitivities
    [SerializeField] private int mouseLookConfig;

    [Header("Input Speeds")] // player movement sensitivities
    [SerializeField] private float throttleSpeedConfig;
    [SerializeField] private float pitchSpeedConfig;
    [SerializeField] private float yawSpeedConfig;
    [SerializeField] private float rollSpeedConfig;

    [Header("Input Accelerations")] // player movement accelerations (see Lerps)
    [SerializeField] private float throttleAccelerationConfig;
    [SerializeField] private float rollAccelerationConfig;

    // getter methods for ship config settings
    public InputMode getInputMode() => inputModeConfig;
    public float getInvertedMouse() => invertedMouseConfig;
    public int getMouseLook() => mouseLookConfig;
    public float getThrottleSpeed() => throttleSpeedConfig;
    public float getPitchSpeed() => pitchSpeedConfig;
    public float getYawSpeed() => yawSpeedConfig;
    public float getRollSpeed() => rollSpeedConfig;
    public float getThrottleAcceleration() => throttleAccelerationConfig;
    public float getRollAcceleration() => rollAccelerationConfig;
}

// selection between different types of mouse look calculations
public enum InputMode
{
    FPS,
    FlightSim
}
