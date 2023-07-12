using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// accesses spawn(); see ObjectPooler. Debris will be spawned in pairs so long as the thorttle achieves a certain threshold
public class DebrisSpawner : MonoBehaviour
{
    [SerializeField] private GameObject ship; // for determining spawn behavior
    [SerializeField] private FlightControls flightData; // for accessing flight readout data
    private ObjectPooler debrisPools; // for accessing spawn() instance function through singleton

    // Start is called before the first frame update
    void Start()
    {
        debrisPools = ObjectPooler.Instance; // initialize with singleton 
    }

    // Update is called once per frame
    void Update()
    {
        // quick note: going to change the spawning requisite soon to improve behavior
        if (flightData.getThrottleIn() >= 20)
        {
            debrisPools.spawn(2);
        }
    }
}