using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostCameraShake : MonoBehaviour
{
    [Header("Shake Behavior")]
    [SerializeField] private float duration;
    [SerializeField] private AnimationCurve magnitude;

    private FlightControls flightData;
    private float magnitudeEval;
    private Vector3 originalPosition;
    private float timeElapsed;
    private float deltaX;
    private float deltaY;

    void Start()
    {
        originalPosition = transform.localPosition;
        flightData = GameObject.Find("scene-manager").GetComponent<FlightControls>();
    }

    void Update()
    {
    }

    public IEnumerator boostShake()
    {
        timeElapsed = 0f;

        while (timeElapsed < duration && flightData.getThrottleIn() >= 20f)
        {
            magnitudeEval = magnitude.Evaluate(timeElapsed);

            deltaX = Random.Range(-1f, 1f) * magnitudeEval;
            deltaY = Random.Range(-1f, 1f) * magnitudeEval;

            transform.localPosition = new Vector3(deltaX, originalPosition.y + deltaY, originalPosition.z);

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}
