using Assets.Scripts.Core.BaseClasses;
using Assets.Scripts.Core.eventArgs;
using Assets.Scripts.Core.Interfaces;
using System;
using UnityEngine;

public class MeleeClass : EnemyBase, IUnit
{
    private void FixedUpdate()
    {
        bool IsRunning;
        bool IsAttacking;

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
                IsAttacking = true;
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
                    IsRunning = true;
                }
                // else
                //     animator.SetBool("IsRunning", false);
            }
        }

        Animationhandler(IsRunning, IsAttacking, false, false, false);

        if (attackAnimationIsRunning && playerHitBox != null && attackCanDealDamage)
            ApplyDamage();
    }
}
