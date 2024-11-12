using UnityEngine;

public class CustomGroundCheck : MonoBehaviour
{
    private const float DEFAULT_GROUND_CHECK_RADIUS = 0.4f;

    [SerializeField] private GameObject groundCheckObject;
    [SerializeField] private float groundCheckRadius = DEFAULT_GROUND_CHECK_RADIUS;
    [SerializeField] private LayerMask groundLayer;

    public bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheckObject.transform.position, groundCheckRadius, groundLayer);
    }
}