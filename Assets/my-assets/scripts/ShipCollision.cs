using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCollision : MonoBehaviour
{
    [SerializeField] private GameObject playerCamera;
    private CameraShake shaker;

    // Start is called before the first frame update
    void Start()
    {
        shaker = playerCamera.GetComponent<CameraShake>();       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collision)
    {
        StartCoroutine(shaker.shake());
    }
}
