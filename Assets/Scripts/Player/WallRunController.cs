using Player;
using UnityEngine;

public enum WallRunSide
{
    Left,
    Right,
}

public class WallRunController
{
    private PlayerMovement playerMovementHandle;
    private GameObject wallCheckLeft;
    private GameObject wallCheckRight;
    private LayerMask wallCheckLayer;

    private float wallRunSpeed;
    private Vector3 wallRunDirectionNormal;
    private float wallCheckDistance = 0.1f;
    public WallRunController(PlayerMovement playerMovement, GameObject wallCheckLeft, GameObject wallCheckRight, LayerMask wallCheckLayer, float wallRunSpeed)
    {
        playerMovementHandle = playerMovement;
        this.wallCheckLeft = wallCheckLeft;
        this.wallCheckRight = wallCheckRight;
        this.wallCheckLayer = wallCheckLayer;
        this.wallRunSpeed = wallRunSpeed;
    }

    public void RunWallChecks()
    {
        if (Physics.Raycast(wallCheckLeft.transform.position, -wallCheckLeft.transform.right, out var hit, wallCheckDistance, wallCheckLayer) && !playerMovementHandle.IsGrounded)
        {
            if (playerMovementHandle.GetMovementState() != MovementType.WallRunning) 
                BeginWallRun(WallRunSide.Left, hit.normal);
        }
        else if (Physics.Raycast(wallCheckRight.transform.position, wallCheckRight.transform.right, out var hit2, wallCheckDistance, wallCheckLayer) && !playerMovementHandle.IsGrounded)
        {
            if (playerMovementHandle.GetMovementState() != MovementType.WallRunning)
                BeginWallRun(WallRunSide.Right, hit2.normal);
        }
        else
        {
            if (playerMovementHandle.GetMovementState() == MovementType.WallRunning)
                EndWallRun();
        }
    }

    private void BeginWallRun(WallRunSide runSide, Vector3 wallNormal)
    {
        switch (runSide)
        {
            case WallRunSide.Left:
                wallRunDirectionNormal = Vector3.Cross(wallNormal, Vector3.up);
                break;
            case WallRunSide.Right:
                wallRunDirectionNormal = Vector3.Cross(wallNormal, Vector3.down);
                break;
        }
        playerMovementHandle.SetMovementState(MovementType.WallRunning);
    }

    public void Update(Vector2 movementInputs)
    {
        // Move player in the direction of the wall run
        if (movementInputs.y > 0.0f)
            playerMovementHandle.Controller.Move(wallRunDirectionNormal * (wallRunSpeed * Time.deltaTime));
        else
        {
            EndWallRun();
        }
    }

    public void EndWallRun()
    {
        playerMovementHandle.SetMovementState(MovementType.Idle);
    }
}
