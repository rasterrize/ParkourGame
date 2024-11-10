using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity.UI;
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
        
      
    }
    public void rhythmBarActivated(float inputBarSpeed)
    {
        barObjectCheck = Instantiate(barObject, transform);
        barObjectCheck.speed = inputBarSpeed;
    }

}
