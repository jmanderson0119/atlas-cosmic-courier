using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// controls audio and visual cues for player attacks + calls the shoot() function to deal damage (see ObjectPooler.cs)
public class ShipFire : MonoBehaviour
{
    // bullet sound effects
    [SerializeField] private AudioSource bulletSource;
    [SerializeField] private AudioClip bulletNoise;

    private ObjectPooler objectPooler; // Instance reference

    // Initialization
    void Start()
    {
        objectPooler = ObjectPooler.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        // if the player fires, the shot is taken and hte audio cue is activated
        if (Input.GetMouseButtonDown(0))
        {
            bulletSource.PlayOneShot(bulletNoise, 0.5f);
            StartCoroutine(objectPooler.shoot());
        }
    }
}
