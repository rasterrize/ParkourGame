using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarMovementCheck : MonoBehaviour
{
    public float speed = 100;
    Transform left;
    Transform right;
    [HideInInspector] public float dist;
    [SerializeField] PlayerRhythmController rhythmController;
    private void Start()
    {
        left = transform.Find("leftBar");
        
        right = transform.Find("rightBar");

        rhythmController = GameObject.Find("Player").GetComponent<PlayerRhythmController>();

    }
    private void Update()
    {
        
        if (dist <= 100) {  Debug.DrawLine(left.transform.position, right.transform.position, Color.red); }
        else { Debug.DrawLine(left.transform.position, right.transform.position, Color.blue); }

        dist = Vector3.Distance(left.transform.position, right.transform.position);
        if (dist <=  25) 
        {
            
            rhythmController.scoreUpdate(-100, -2f);
            Destroy(rhythmController.rhythmBarList[0].gameObject);
            rhythmController.rhythmBarList.Remove(rhythmController.rhythmBarList[0]);
            
        }
        //Translating the rhythm bars
        left.transform.Translate(Vector3.right * Time.deltaTime * speed);
        right.transform.Translate(Vector3.left * Time.deltaTime * speed);
        //Debug.Log(dist);
    }
}
