using UnityEngine;
using UnityEngine.UI;

public class BarMovementCheck : MonoBehaviour
{
    public float speed = 100;
    [SerializeField] private GameObject left;
    [SerializeField] private GameObject right;
    [HideInInspector] public float dist;
    [SerializeField] private PlayerRhythmController rhythmController;

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
        if (dist <= 25)
        {
            rhythmController.ScoreUpdate(-100, -2f);
            Destroy(rhythmController.rhythmBarList[0].gameObject);
            rhythmController.rhythmBarList.Remove(rhythmController.rhythmBarList[0]);
        }

        // Translating the rhythm bars
        left.transform.Translate(Vector3.right * (Time.deltaTime * speed));
        right.transform.Translate(Vector3.left * (Time.deltaTime * speed));
    }
}