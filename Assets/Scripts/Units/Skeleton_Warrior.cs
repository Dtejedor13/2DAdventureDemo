using Assets.Scripts.Core.BaseClasses;
using Assets.Scripts.Core.eventArgs;
using Assets.Scripts.Core.Interfaces;
using System;
using UnityEngine;

public class Skeleton_Warrior : EnemyBase, IUnit
{
    public event EventHandler<RangeAttackEventArgs> RangeAttackEvent;

    // Start is called before the first frame update
    void Start()
    {
        base.OnStart();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        base.ColissionEnter2D(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        base.ColissionExit2D(collision);
    }

    private void FixedUpdate()
    {
        if (animator.GetBool("IsGettingDamage")) return;
        if (animator.GetBool("IsDeath"))
        {
            Hpbar.GetComponent<FloatingHpBar>().ApplyDamage(0);
            return;
        }

        if (GetTarget == null)
            SearchForTarget();
        else
        {
            if (GetPlayerHitBox != null && !GetAttackAnimationIsRunning)
            {
                animator.SetBool("IsRunning", false);
                animator.SetBool("IsAttacking", true);
                GetAttackAnimationIsRunning = true;
                GetAttackCanDealDamage = true;
            }
            else
            {
                if (GetTarget.position.x > transform.position.x) Srenderer.flipX = false; else Srenderer.flipX = true;

                if (!GetAttackAnimationIsRunning && ((Srenderer.flipX && (transform.position.x - GetTarget.position.x <= TargetRange)) 
                    || (!Srenderer.flipX && (GetTarget.position.x - transform.position.x <= TargetRange)) || GetTargetAggro))
                {
                    GetTargetAggro = true;
                    transform.position += new Vector3((Srenderer.flipX ? MoveSpeed * -1 : MoveSpeed), 0, 0) * (0.025f * 1);
                    // Move animation
                    animator.SetBool("IsRunning", true);
                }
                else
                    animator.SetBool("IsRunning", false);
            }
        }

        if (GetAttackAnimationIsRunning && GetPlayerHitBox != null && GetAttackCanDealDamage)
            ApplyDamage();
    }
}
