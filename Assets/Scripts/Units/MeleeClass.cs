using Assets.Scripts.Core.BaseClasses;
using Assets.Scripts.Core.eventArgs;
using Assets.Scripts.Core.Interfaces;
using System;

public class MeleeClass : EnemyBase, IUnit
{
    public event EventHandler<RangeAttackEventArgs> RangeAttackEvent;

    private void FixedUpdate()
    {
        bool IsRunning = false;
        bool IsAttacking = false;

        if (animator.GetBool("IsGettingDamage")) return;

        if (animator.GetBool("IsDeath"))
        {
            Hpbar.GetComponent<FloatingHpBar>().ApplyDamage(0);
            return;
        }

        if (Target == null)
            SearchForTarget();
        else
        {
            CheckPositionAndFlipUnit();

            if (PlayerHitBox != null && !AttackAnimationIsRunning) // Unit is in attackrange
            {
                IsAttacking = true;
                AttackAnimationIsRunning = true;
                AttackCanDealDamage = true;
            }
            else
            {
                if (!AttackAnimationIsRunning && (GetDistance() <= AggroRange || TargetAggro))
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

        AnimationHandler(IsRunning, IsAttacking, false, null, false);

        if (AttackAnimationIsRunning && PlayerHitBox != null && AttackCanDealDamage)
            ApplyDamage();
    }
}
