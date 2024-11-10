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

    [Header("Legs")]
    public GameObject legs;

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
        
        if (playerMovement.currentMovementState == MovementType.Sliding) { legs.SetActive(true); }
        else { legs.SetActive(false); }

        if (playerMovement.currentMovementState == MovementType.Sliding) { slidingAnimation(true); }
        

        //if (playerMovement.currentMovementState =- MovementType.WallRunning) { wallRunningAnimation(true); }
        //else { wallRunningAnimation(false); }
    }
    public void visibleArms(bool visible)
    {
        leftArm.enabled = visible;
      
    }
    public void runningAnimation(bool boolRun)
    {
        leftArm.SetBool("Running", boolRun);
        rightArm.SetBool("Running", boolRun);
    }
    public void slidingAnimation(bool boolSlide)
    {
        leftArm.SetTrigger("Slide");
        rightArm.SetTrigger("Slide");
        legs.SetActive(boolSlide);
        
    }
    public void walkingAnimation(bool boolRun)
    {
        leftArm.SetBool("Walking", boolRun);
        rightArm.SetBool("Walking", boolRun);
    }
    public void wallRunningAnimation(bool boolWallRun)
    {
        leftArm.SetBool("Wall Run", boolWallRun);
        rightArm.SetBool("Wall Run", boolWallRun);
    }
}
