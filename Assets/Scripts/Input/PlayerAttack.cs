using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMovement playerMovement;

    private const string RegularAttackTrigger = "Regular Attack";
    private const string CrouchAttackTrigger = "Crouch Attack";
    private const string AirAttackTrigger = "Air Attack";

    private bool canAttack = true;

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
            PerformCrouchingAttack();
        }
        else if (!playerMovement.CheckIfGrounded())
        {
            PerformAirAttack();
        }
        else
        {
            PerformRegularAttack();
        }
    }

    private void PerformRegularAttack()
    {
        canAttack = false;

        playerMovement.rigidbody.velocity = Vector2.zero;
        animator.SetFloat("Run Speed", 0f);

        animator.ResetTrigger(RegularAttackTrigger);
        animator.SetTrigger(RegularAttackTrigger);

        StartCoroutine(WaitForAnimation());
    }

    private void PerformCrouchingAttack()
    {
        canAttack = false;

        animator.ResetTrigger(CrouchAttackTrigger);
        animator.ResetTrigger(RegularAttackTrigger);
        animator.SetTrigger(CrouchAttackTrigger);

        StartCoroutine(WaitForAnimation());
    }

    private void PerformAirAttack()
    {
        canAttack = false;

        animator.ResetTrigger(AirAttackTrigger);
        animator.SetTrigger(AirAttackTrigger);

        StartCoroutine(WaitForAnimation());
    }

    private IEnumerator WaitForAnimation()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        canAttack = true;
    }
}
