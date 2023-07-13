using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelHistory : MonoBehaviour
{
    [Header("Object to Track")]
    [SerializeField] private GameObject ship; // for accessing transform data

    [Header("Tracking Field")]
    [SerializeField] private GameObject historyPrefab; // collider prefab for marking segments where debris has been spawned 

    private GameObject historyPack; // for setting up next history marking
    private Queue<GameObject> historyArchive; // for deleting history

    // going to mark history every eight seconds and delete it every 8 seconds once three history packs have been queued
    void Start()
    {
        historyArchive = new Queue<GameObject>(); // initialize history queue
        
        InvokeRepeating("markHistory", 8f, 8f);
        InvokeRepeating("deleteHistory", 32f, 8f);
    }

    // a collider will be placed to represent where debris has been spawned, which will deactivate the debris spawner if collided with by the ship
    void markHistory()
    {
        historyPack = Instantiate(historyPrefab); // this is what's "marking" where debris field has been spawned
       
        StartCoroutine(setHistoryAsActiveTrigger()); // waits to activate the history field
        
        historyPack.transform.position = ship.transform.position - ship.transform.forward * 1500f; // pack is positioned here to give the ship enough time to escape the pack before its activated

        historyArchive.Enqueue(historyPack); // placed in the archive
    }

    // colliders that have "sufficiently aged" will be destroyed for performance
    void deleteHistory()
    {
        // just in case something goes wrong, dequeue/destroy won't be attempted if history is not present
        if (historyArchive.Count != 0)
            Destroy(historyArchive.Dequeue());
    }

    // waiting to activate the history makes sure the ship can escape the most recently instantiated pack; I've set up history tracking this way due to how the ship speed compares to the diameter of the pack prefab
    IEnumerator setHistoryAsActiveTrigger()
    {
        yield return new WaitForSeconds(7.8f);
        historyPack.GetComponent<Collider>().isTrigger = true;
    }
}
