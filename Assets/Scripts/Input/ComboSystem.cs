using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboSystem : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private int comboIndex = 0;
    private bool canAttack = true;
    private bool attackInputBuffered = false;

    void Update()
    {
        if (InputManager.playerCombat)
        {
            if (canAttack)
            {
                PerformCombo();
            }
            else
            {
                attackInputBuffered = true;
            }
        }
    }

    private void PerformCombo()
    {
        if (!canAttack) return;

        canAttack = false;

        animator.SetInteger("Combo Index", comboIndex);

        switch (comboIndex)
        {
            case 0:
                comboIndex = 1; 
                break;
            case 1:
                comboIndex = 2;
                break;
            case 2:
                comboIndex = 1;
                break;
        }

        animator.SetInteger("Combo Index", comboIndex);

        StartCoroutine(WaitForAnimation());
    }

    private IEnumerator WaitForAnimation()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;
        
        float elapsedTime = 0f;

        while (elapsedTime < animationLength)
        {
            elapsedTime += Time.deltaTime;

            if (attackInputBuffered)
            {
                attackInputBuffered = false;
                PerformCombo();
                yield break;
            }
        }

        yield return null;

        canAttack = true;

        if (!attackInputBuffered)
        {
            ResetCombo();
        }
        else
        {
            attackInputBuffered = false;
            PerformCombo();
        }
    }

    private void ResetCombo()
    {
        comboIndex = 0;
        animator.SetInteger("Combo Index", comboIndex);
        canAttack = true;
        attackInputBuffered = false;
    }
}
