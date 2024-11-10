using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BarMovement : BarMovementCheck
{
    [SerializeField] [Range(-1f,1f)] int direction;
    

    private void Start()
    {
        
    }
    float movementSpeed() { return GetComponent<BarMovementCheck>().speed; }
    void Update()
    {
        speed = movementSpeed();
        transform.Translate(new Vector3(direction, 0, 0) * Time.deltaTime * speed);
        
    }
   

}