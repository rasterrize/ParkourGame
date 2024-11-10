using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerRhythmController : MonoBehaviour
{
    [Header("Rhythm Bar")]
    [SerializeField] private BarMovementCheck barObject;
    private BarMovementCheck barObjectCheck;
    [SerializeField] private GameObject rhythmBar;
    [SerializeField] public int distanceDetection;

    [Header("Score")]
    private int playerScore;
    [SerializeField] private TextMeshProUGUI scoreCounter;
    [SerializeField] private TextMeshProUGUI scoreMultiper;

    [Header("Timer")]
    [SerializeField] GameObject timerObject;
    private float timerScale;
    private float playerTimer;

    public List<BarMovementCheck> rhythmBarList = new List<BarMovementCheck>();
    private void Start()
    {
        Mathf.Clamp(playerScore, 0f, 999999f);
        scoreMultiper.text = "x" + multiplerFunction().ToString();
    }
    private void Update()
    {
        updateTimer();
    }
    /// <summary>
    /// Detects the distance of the first set of rhythm bars.
    /// Adds points if the player successfully times the 'rhythm'.
    /// Takes points away for activating early.
    /// </summary>
    /// <param name="inputBarSpeed">Sets the speed of the bars</param>
    /// <param name="inputScoreReward">Sets the value of the points after a successful activation</param>
    /// <param name="inputTimeReward">Sets the value of the time after a successful activation</param>
    public void rhythmBarActivated(float inputBarSpeed, float inputScoreReward, float inputTimeReward)
    {
        if (rhythmBarList.Count != 0)
        {
            if (rhythmBarList[0].GetComponent<BarMovementCheck>().dist < distanceDetection)
            {
                // On successful activation gives the player a set amount of points
                Destroy(rhythmBarList[0].gameObject);
                rhythmBarList.Remove(rhythmBarList[0]);
                scoreUpdate(inputScoreReward * multiplerFunction(), inputTimeReward);
            }
            else
            {
                // Destorys the current object for a new one to spawn, punishing the player
                Destroy(rhythmBarList[0].gameObject);
                rhythmBarList.Remove(rhythmBarList[0]);
                scoreUpdate(-10f, 0f);
            }
        }
        // Creates a new set of rhythm bars
        barObjectCheck = Instantiate(barObject, rhythmBar.transform);
        barObjectCheck.speed = inputBarSpeed;
        rhythmBarList.Add(barObjectCheck);
    }

    public void scoreUpdate(float scoreAddition, float timeBonus)
    {
        // Adding to the player score & player multiplier
        playerScore += Mathf.RoundToInt(scoreAddition);
        scoreCounter.text = playerScore.ToString();

        scoreMultiper.text = "x" + multiplerFunction().ToString();

        // Adding Timer
        playerTimer += timeBonus * multiplerFunction();
        if (playerTimer < 0) { playerTimer = 0; }


    }

    void updateTimer()
    {
        timerScale = playerTimer / 10;
        if (playerTimer > 0.001f)
        {
            playerTimer -= Time.deltaTime * multiplerFunction();
        }
        timerObject.transform.localScale = new Vector3(Mathf.Clamp(timerScale, 0f, 1f), 1, 1);

    }
    private int multiplerFunction()
    {   //  Lower Parameter     Upper Parameter
        if (timerScale >= 1 && timerScale < 1.5) { return 2; }
        else if (timerScale >= 1.5 && timerScale < 2) { return 3; }
        else { return 1; }

    }
}
