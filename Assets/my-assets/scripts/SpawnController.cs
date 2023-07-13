using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// will deactivate debris spawn when the player is in a region where they have already generated a field
public class SpawnController : MonoBehaviour
{
    [Header("Spawn Source")]
    [SerializeField] private GameObject manager; // manager for accessing the boolean in charge of allowing debris spawn
    
    private ObjectPooler objectPooler; // object for accessing the boolean in charge of allowing debris spawn

    // initialize objectPooler
    void Start()
    {
        objectPooler = manager.GetComponent<ObjectPooler>();
    }

    // let the spawning manager know the ship is in a history field
    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Entered History");
        objectPooler.setInHistory(true);
    }

    // let the spawning manager know the ship is not in a history field
    void OnTriggerExit(Collider collision)
    {
        Debug.Log("Exited History");
        objectPooler.setInHistory(false);
    }
}
