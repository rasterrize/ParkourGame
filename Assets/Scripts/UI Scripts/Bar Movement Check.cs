using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarMovementCheck : MonoBehaviour
{
    public float speed = 100;
    [SerializeField] GameObject left;
    [SerializeField] GameObject right;
    [HideInInspector] public float dist;
    [SerializeField] PlayerRhythmController rhythmController;
    private void Start()
    {
        dist = Vector3.Distance(left.transform.position, right.transform.position);
        rhythmController = GameObject.Find("Player").GetComponent<PlayerRhythmController>();
       
    }
    private void Update()
    {
        
        if (dist <= rhythmController.distanceDetection) 
        {
            left.GetComponent<Image>().color = Color.red;
            right.GetComponent<Image>().color = Color.red;
        }
       

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
