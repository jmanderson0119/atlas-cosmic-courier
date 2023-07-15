using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Behavior")]
    [SerializeField] private float duration;
    [SerializeField] private AnimationCurve magnitude;
    
    private float magnitudeEval;
    private Vector3 originalPosition;
    private float timeElapsed;
    private float deltaX;
    private float deltaY;

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartCoroutine(shake());
        }
    }

    public IEnumerator shake()
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
