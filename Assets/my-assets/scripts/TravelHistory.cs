using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelHistory : MonoBehaviour
{

    [SerializeField] private GameObject ship; // for accessing transform data
    [SerializeField] private GameObject historyPrefab; // collider prefab for marking segments where debris has been spawned
    [SerializeField] private FlightControls flightData; // for accessing flight readout data 
    private GameObject historyPack; // for setting up next history marking
    private Queue<GameObject> historyArchive; // for deleting history


    // going to mark history every ten seconds
    void Start()
    {
        historyArchive = new Queue<GameObject>();
        InvokeRepeating("markHistory", 8f, 8f);
        InvokeRepeating("deleteHistory", 32f, 8f);
    }

    // a collider will be placed to represent where debris has been spawned, which will deactivate the debris spawner if collided with by the ship
    void markHistory()
    {
        if (flightData.getThrottleIn() > 20)
        {
            historyPack = Instantiate(historyPrefab);
            StartCoroutine(setHistoryAsActiveTrigger());
            //historyPack.GetComponent<Collider>().isTrigger = true;
            historyPack.transform.position = ship.transform.position - ship.transform.forward * 750f;

            historyArchive.Enqueue(historyPack);
        }
    }

    void deleteHistory()
    {
        Destroy(historyArchive.Dequeue());
    }

    IEnumerator setHistoryAsActiveTrigger()
    {
        Debug.Log("Set Active Trigger accessed.");
        yield return new WaitForSeconds(3f);
        historyPack.GetComponent<Collider>().isTrigger = true;
        Debug.Log(historyArchive.Peek().GetComponent<Collider>().isTrigger);
    }
}
