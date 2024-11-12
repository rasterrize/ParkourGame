using UnityEngine;

public class BarMovement : BarMovementCheck
{
    [SerializeField] [Range(-1f, 1f)] private int direction;

    private float GetMovementSpeed() => GetComponent<BarMovementCheck>().speed;

    private void Update()
    {
        speed = GetMovementSpeed();
        transform.Translate(new Vector3(direction, 0, 0) * (Time.deltaTime * speed));
    }
}