using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private GameObject shipCamera;
    //[SerializeField] private AnimationCurve shakeBehavior;
    [SerializeField] private float shakeDuration;

    private float currentTime;
    private float shake;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Entered");
        if (collision.gameObject.CompareTag("DebrisCollision"))
            StartCoroutine(cameraShake());
    }

    IEnumerator cameraShake()
    {
        Debug.Log("Collision at: " + transform.position);

        currentTime = 0f;

        while (currentTime < shakeDuration)
        {
            currentTime += Time.deltaTime;
            shake = Random.Range(-1f, 1f);
            shipCamera.transform.position += new Vector3(shake, shake, 0f);// * shakeBehavior.Evaluate(currentTime);
            yield return null;
        }

        Debug.Log("Reset Camera");
        shipCamera.transform.position = transform.position + transform.up + transform.forward * -2.4f;
    }
}
