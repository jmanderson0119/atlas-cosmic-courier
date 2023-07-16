using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCamShake : MonoBehaviour
{
    [Header("Shake Behavior")] // how long and how much
    [SerializeField] private float duration;
    [SerializeField] private AnimationCurve magnitude;
    
    private float magnitudeEval; // magnitude at a given time
    private Vector3 originalPosition; // for resetting cam
    private float timeElapsed; // for tracking for long camera shake has been active
    private float deltaX; // shake lattitudinally
    private float deltaY; // shake longitudinally

    // initialization
    void Start()
    {
        originalPosition = transform.localPosition;
    }

    // offsets the local position of the camera each active frame, then resets the local position back to center
    public IEnumerator collisionShake()
    {
        timeElapsed = 0f;

        while (timeElapsed < duration)
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
