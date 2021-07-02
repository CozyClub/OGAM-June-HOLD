using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAnimatorControl : MonoBehaviour
{
    const float NORMAL_SPEED = 1/1.9f;
    const float CROUCH_SPEED = 1/1.7f;
    public Animator animator;
    public void Jump()
    {
        animator.   SetTrigger("Jump");
    }
    public void UpdateStates(Vector3 velocity, bool crouch, playerState state, float playerSpeed)
    {
        animator.SetFloat("Velocity", velocity.sqrMagnitude);
        animator.SetBool("Sneak", crouch);
        switch(state)
        {    
            case(playerState.Sneak): 
                animator.speed = playerSpeed*CROUCH_SPEED;       
            break;
            case(playerState.Idle):
                animator.speed = 1;
            break;
            case(playerState.Walk):
                animator.speed = playerSpeed*NORMAL_SPEED;
            break;
        }
    }
}
