using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity.UI;
using static HelperScript;
using System.Timers;
using UnityEngine.UIElements;
using UnityEngine.Rendering;
public class RhythmBarScript : MonoBehaviour
{
    [Header("Bar Set Up")]
    [SerializeField] BarMovementCheck barObject;
    BarMovementCheck barObjectCheck;

    [Header("Timer")]
    [SerializeField] GameObject timerObject;

    private void Update()
    {
        
        updateTimer();
    }
    public void rhythmBarActivated(float inputBarSpeed)
    {
        
        barObjectCheck = Instantiate(barObject, transform);
        barObjectCheck.speed = inputBarSpeed;

    }
    float timerScale;

    void updateTimer()
    {

        timerScale = playerTimer / 10;
        
        if (playerTimer > 0.001f)
        {
            playerTimer -= Time.deltaTime;
        }
        timerObject.transform.localScale = new Vector3(timerScale, 1, 1);
    }
}
