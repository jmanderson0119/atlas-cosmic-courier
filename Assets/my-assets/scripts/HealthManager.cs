using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    // visual health cues
    [Header("GUI Elements")]
    [SerializeField] private GameObject healthbar;
    [SerializeField] private GameObject shieldBar;

    // health data
    [Header("Health Readout")]
    [SerializeField] private float playerHealthCurrent;

    public const float playerHealthMaximum = 200f; // default health

    // getter/setter pair for current health
    public float getPlayerHealthCurrent() => playerHealthCurrent;
    public void setPlayerHealthCurrent(float health) { playerHealthCurrent = health; }

    // reset player health to maximum on level-startup
    void Start()
    {
        playerHealthCurrent = playerHealthMaximum;
    }

    // will check for lose condition within the level
    void Update()
    {
        if (playerHealthCurrent <= 0)
        {
            //StartCoroutine(gameOver());
        }
    }

    public void debrisCollision()
    {
        if (getPlayerHealthCurrent() > 100)
        {
            shieldBar.SetActive(false); // visual cue
            setPlayerHealthCurrent(100f); // shield break

            Debug.Log("Player Health: " + getPlayerHealthCurrent());
        }
        else
        {
            healthbar.SetActive(false); // visual cue
            setPlayerHealthCurrent(0f); // ship destroyed

            //StartCoroutine(GameOver());
            return;
        }
    }
}
