using UnityEngine;

public class RhythmBarScript : MonoBehaviour
{
    [Header("Bar Set Up")] [SerializeField]
    private BarMovementCheck barObject;

    [Header("Timer")] [SerializeField] private GameObject timerObject;

    private BarMovementCheck barObjectCheck;

    public void RhythmBarActivated(float inputBarSpeed)
    {
        barObjectCheck = Instantiate(barObject, transform);
        barObjectCheck.speed = inputBarSpeed;
    }
}