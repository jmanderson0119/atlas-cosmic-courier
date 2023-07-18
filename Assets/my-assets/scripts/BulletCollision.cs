using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// handles collision of bullets with various environmental obstacles in the level
public class BulletCollision : MonoBehaviour
{
    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Hit at: " + collision.gameObject.transform.position);
        gameObject.SetActive(false);
    }
}
