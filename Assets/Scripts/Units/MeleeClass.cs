using Assets.Scripts.Core.BaseClasses;
using Assets.Scripts.Core.eventArgs;
using Assets.Scripts.Core.Interfaces;
using System;
using UnityEngine;

public class MeleeClass : EnemyBase, IUnit
{
    private void FixedUpdate()
    {
        if (animator.GetBool("IsGettingDamage")) return;

        if (animator.GetBool("IsDeath"))
        {
            Hpbar.GetComponent<FloatingHpBar>().ApplyDamage(0);
            return;
        }

        if (target == null)
            SearchForTarget();
        else
        {
            CheckPositionAndFlipUnit();

            if (playerHitBox != null && !attackAnimationIsRunning) // Unit is in attackrange
            {
                animator.SetBool("IsRunning", false);
                animator.SetBool("IsAttacking", true);
                attackAnimationIsRunning = true;
                attackCanDealDamage = true;
            }
            else
            {
                if (!attackAnimationIsRunning && (getDistance() <= AggroRange || TargetAggro))
                {
                    if (!TargetAggro) TargetAggro = true;
                    Move();
                    // Move animation
                    animator.SetBool("IsRunning", true);
                }
                // else
                //     animator.SetBool("IsRunning", false);
            }
        }

        if (attackAnimationIsRunning && playerHitBox != null && attackCanDealDamage)
            ApplyDamage();
    }
}
