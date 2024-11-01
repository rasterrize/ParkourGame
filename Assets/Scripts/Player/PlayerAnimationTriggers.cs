using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{
    [Header("Animators")]
    public Animator leftArm;
    public Animator rightArm;

    [Header("msc")]
    [SerializeField] GameObject playerModel;

    private PlayerMovement playerMovement;
    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }
    private void Update()
    {
        if (playerMovement.currentMovementState == MovementType.Walking) { walkingAnimation(true); }
        else { walkingAnimation(false); }

        if (playerMovement.currentMovementState == MovementType.Running) { runningAnimation(true); }
        else { runningAnimation(false); }
        
        if (playerMovement.currentMovementState == MovementType.Jumping) { jumpingAnimation(); }

        if (playerMovement.currentMovementState == MovementType.Sliding) { slidingAnimation(); }
    }
    public void jumpingAnimation()
    {
        leftArm.SetTrigger("Jump");
        rightArm.SetTrigger("Jump");
    }
    public void runningAnimation(bool boolRun)
    {
        leftArm.SetBool("Running", boolRun);
        rightArm.SetBool("Running", boolRun);
    }
    public void slidingAnimation()
    {
        leftArm.SetTrigger("Slide");
        rightArm.SetTrigger("Slide");
    }
    public void walkingAnimation(bool boolRun)
    {
        leftArm.SetBool("Walking", boolRun);
        rightArm.SetBool("Walking", boolRun);
    }
}
