using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerRhythmController : MonoBehaviour
{
    [Header("Rhythm Bar")] [SerializeField]
    private BarMovementCheck barObject;

    private BarMovementCheck barObjectCheck;
    [SerializeField] private GameObject rhythmBar;
    [SerializeField] public int distanceDetection;

    [Header("Score")] private int playerScore;
    [SerializeField] private TextMeshProUGUI scoreCounter;
    [SerializeField] private TextMeshProUGUI scoreMultiper;

    [Header("Timer")] [SerializeField] private GameObject timerObject;
    private float timerScale;
    private float playerTimer;

    public List<BarMovementCheck> rhythmBarList = new();

    private void Start()
    {
        playerScore = 0;
        scoreMultiper.text = "x" + MultiplerFunction();
    }

    private void Update()
    {
        UpdateTimer();
    }

    /// <summary>
    /// Detects the distance of the first set of rhythm bars.
    /// Adds points if the player successfully times the 'rhythm'.
    /// Takes points away for activating early.
    /// </summary>
    /// <param name="inputBarSpeed">Sets the speed of the bars</param>
    /// <param name="inputScoreReward">Sets the value of the points after a successful activation</param>
    /// <param name="inputTimeReward">Sets the value of the time after a successful activation</param>
    public void RhythmBarActivated(float inputBarSpeed, float inputScoreReward, float inputTimeReward)
    {
        if (rhythmBarList.Count != 0)
        {
            if (rhythmBarList[0].GetComponent<BarMovementCheck>().dist < distanceDetection)
            {
                // On successful activation gives the player a set amount of points
                Destroy(rhythmBarList[0].gameObject);
                rhythmBarList.Remove(rhythmBarList[0]);
                ScoreUpdate(inputScoreReward * MultiplerFunction(), inputTimeReward);
            }
            else
            {
                // Destorys the current object for a new one to spawn, punishing the player
                Destroy(rhythmBarList[0].gameObject);
                rhythmBarList.Remove(rhythmBarList[0]);
                ScoreUpdate(-10f, 0f);
            }
        }

        // Creates a new set of rhythm bars
        barObjectCheck = Instantiate(barObject, rhythmBar.transform);
        barObjectCheck.speed = inputBarSpeed;
        rhythmBarList.Add(barObjectCheck);
    }

    public void ScoreUpdate(float scoreAddition, float timeBonus)
    {
        // Adding to the player score & player multiplier
        playerScore += Mathf.RoundToInt(scoreAddition);
        scoreCounter.text = playerScore.ToString();

        // Adding Timer
        playerTimer += timeBonus;
        if (playerTimer < 0)
            playerTimer = 0;
    }

    private void UpdateTimer()
    {
        scoreMultiper.text = "x" + MultiplerFunction();
        timerScale = playerTimer / 10;
        if (playerTimer > 0.001f)
            playerTimer -= Time.deltaTime;

        timerObject.transform.localScale = new Vector3(Mathf.Clamp(timerScale, 0f, 1f), 1, 1);
    }

    private int MultiplerFunction()
    {
        //  Lower Parameter     Upper Parameter
        if (playerTimer < 5) { return 1; }

        if (playerTimer >= 7 && playerTimer < 15) { return 2; }

        if (playerTimer >= 15 && playerTimer < 20) { return 3; }

        if (playerTimer >= 20 && playerTimer < 25) { return 4; }

        if (playerTimer >= 25 && playerTimer < 28) { return 6; }

        if (playerTimer >= 31 && playerTimer < 35) { return 8; }

        return 10;
    }
}