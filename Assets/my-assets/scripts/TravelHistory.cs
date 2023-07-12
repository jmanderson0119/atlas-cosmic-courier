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
            historyPack.transform.position = ship.transform.position - transform.forward * 1500f;

            historyArchive.Enqueue(historyPack);
        }
    }

    void deleteHistory()
    {
        if (historyArchive.Count != 0)
            Destroy(historyArchive.Dequeue());
    }

    IEnumerator setHistoryAsActiveTrigger()
    {
        yield return new WaitForSeconds(7.8f);
        historyPack.GetComponent<Collider>().isTrigger = true;
    }
}
