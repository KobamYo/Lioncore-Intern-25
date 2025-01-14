using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private float attackDelay = 0.3f;
    private bool canAttack = true;

    [SerializeField] private Animator animator;
    private string currentAnimation;

    // Animation States
    private const string PLAYER_IDLE = "Player_Idle";
    private const string PLAYER_ATTACK_STANDING = "Player_AttackStanding";
    private const string PLAYER_ATTACK_CROUCH = "Player_AttackCrouch";
    private const string PLAYER_ATTACK_AIR = "Player_AttackAir";

    void Update()
    {
        if (InputManager.playerCombat && canAttack)
        {
            HandleAttack();
        }
    }

    private void HandleAttack()
    {
        if (InputManager.playerCrouch && playerMovement.CheckIfGrounded())
        {
            PerformAttack(PLAYER_ATTACK_CROUCH, false);
        }
        else if (!playerMovement.CheckIfGrounded())
        {
            PerformAttack(PLAYER_ATTACK_AIR, false);
        }
        else
        {
            PerformAttack(PLAYER_ATTACK_STANDING, true);
        }
    }

    private void PerformAttack(string animationState, bool enableMovementAfter)
    {
        canAttack = false;

        if (enableMovementAfter)
        {
            playerMovement.enabled = false;
            playerMovement.rigidbody.velocity = Vector2.zero;
            animator.SetFloat("Run Speed", 0f);
        }

        PlayAnimation(animationState);
        StartCoroutine(WaitForAnimation(enableMovementAfter));
    }

    private void PlayAnimation(string newAnimation)
    {
        if (currentAnimation == newAnimation) return;

        animator.Play(newAnimation);
        currentAnimation = newAnimation;
    }

    private IEnumerator WaitForAnimation(bool enableMovement)
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        if (enableMovement)
        {
            playerMovement.enabled = true;
        }
        canAttack = true;

        // Transition back to Idle animation after attack finishes
        PlayAnimation(PLAYER_IDLE);
    }
}
