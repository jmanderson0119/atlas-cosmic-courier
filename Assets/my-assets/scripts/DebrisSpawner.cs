/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisSpawner : MonoBehaviour
{
    [SerializeField] private GameObject ship;
    [SerializeField] private FlightControls flightData;
    private ObjectPooler debrisPools;


    // Start is called before the first frame update
    void Start()
    {
        debrisPools = ObjectPooler.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (flightData.getThrottleIn() >= 20f)
        {
            string[] excludedIdentifiers = GetVisibleIdentifiers();
            debrisPools.spawn(excludedIdentifiers);
        }
    }

    private string[] GetVisibleIdentifiers()
    {
        List<string> visibleIdentifiers = new List<string>();

        RaycastHit[] hits = Physics.SphereCastAll(ship.transform.position, 1000f, ship.transform.forward);

        foreach (RaycastHit hit in hits)
        {
            GameObject hitObject = hit.collider.gameObject;
            ObjectPooler.Pool pool = hitObject.GetComponentInParent<ObjectPooler.Pool>();

            if (pool != null &&  !visibleIdentifiers.Contains(pool.identifier))
            {
                visibleIdentifiers.Add(pool.identifier);
                pool.isVisible = true;
            }
        }

        return visibleIdentifiers.ToArray();
    }
}
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisSpawner : MonoBehaviour
{
    [SerializeField] private GameObject ship;
    [SerializeField] private FlightControls flightData;
    private ObjectPooler debrisPools;

    void Start()
    {
        debrisPools = ObjectPooler.Instance;
    }

    void Update()
    {
        if (flightData.getThrottleIn() >= 20f)
        {
            string[] excludedTags = GetVisibleTags();
            debrisPools.Spawn(excludedTags);
        }
    }

    private string[] GetVisibleTags()
    {
        List<string> visibleTags = new List<string>();

        RaycastHit[] hits = Physics.SphereCastAll(ship.transform.position, 1000f, ship.transform.forward);

        foreach (RaycastHit hit in hits)
        {
            GameObject hitObject = hit.collider.gameObject;
            ObjectPooler.Pool pool = debrisPools.GetParentPoolComponent(hitObject);

            if (pool != null && !visibleTags.Contains(pool.tag))
            {
                visibleTags.Add(pool.tag);
                pool.isVisible = true;
            }
        }

        return visibleTags.ToArray();
    }
}



