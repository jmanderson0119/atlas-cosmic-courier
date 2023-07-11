using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// will deactivate debris spawn when the player is in a region where they have already generated a field
public class SpawnController : MonoBehaviour
{
    [SerializeField] private GameObject manager;
    private ObjectPooler objectPooler;
    private bool inHistory;

    void Start()
    {
        objectPooler = manager.GetComponent<ObjectPooler>();
    }

    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Entered History");
        objectPooler.setInHistory(true);
    }

    void OnTriggerExit(Collider collision)
    {
        Debug.Log("Exited History");
        objectPooler.setInHistory(false);
    }
}
