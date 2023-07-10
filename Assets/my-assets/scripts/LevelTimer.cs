using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

// configurable timer for in-level performance tracking 
public class LevelTimer : MonoBehaviour
{
    [Header("Timer")] // HUD level timer
    [SerializeField] private TextMeshProUGUI timerGUI;

    [Header("Time Readout")] // total time in level
    [SerializeField] private float currentTime;
    private int minutes;
    private float seconds;

    [Header("Limit Settings")] // timer configuration
    [SerializeField] private bool countDown;
    [SerializeField] private bool hasLimit;
    [SerializeField] private float timeLimit;

    [Header("Format Settings")] // format configuration
    [SerializeField] bool hasFormat;
    [SerializeField] private TimeFormats format;
    private Dictionary<TimeFormats, string> timeFormats = new Dictionary<TimeFormats, string>();


    // Start is called before the first frame update
    void Start()
    {
        // configuration time formats with timeFormats dictionary
        timeFormats.Add(TimeFormats.Whole, "{0:00}:{1:00}");
        timeFormats.Add(TimeFormats.Tenth, "{0:00}:{1:00.0}");
        timeFormats.Add(TimeFormats.Hundredth, "{0:00}:{1:00.00}");
        
        // centers timer based on format
        switch (format)
        {
            case TimeFormats.Whole:
                timerGUI.transform.position += new Vector3(10f, 0f, 0f);
                break;
            case TimeFormats.Tenth:
                timerGUI.transform.position += new Vector3(5f, 0f, 0f);
                break;
        }
        
    }
        
    // Update is called once per frame
    void Update()
    {
        // count up for stopwatch and down for countdown timer
        currentTime = countDown ? currentTime -= Time.deltaTime : currentTime += Time.deltaTime;
        
        // stops counting if lower/upper bound is reached
        if (hasLimit && ((countDown && currentTime <= timeLimit) || (!countDown && currentTime >= timeLimit)))
        {
            currentTime = timeLimit;
            SetTimerText();
            enabled = false;
        }

        SetTimerText();
    }

    // sets display time with respect to selected time format
    private void SetTimerText()
    {
        minutes = (int) (currentTime / 60);
        seconds = (currentTime % 60);

        timerGUI.text = hasFormat ? string.Format(timeFormats[format], minutes, seconds) : currentTime.ToString();
    }

    // formatting options: seconds, deciseconds, milliseconds trailing digit 
    private enum TimeFormats
    {
        Whole,
        Tenth,
        Hundredth
    }
}
