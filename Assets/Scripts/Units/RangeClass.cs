using Assets.Scripts.Core.BaseClasses;
using Assets.Scripts.Core.eventArgs;
using Assets.Scripts.Core.Interfaces;
using System;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class RangeClass : EnemyBase, IUnit
    {
        public event EventHandler<RangeAttackEventArgs> RangeAttackEvent;

        void Update()
        {
            base.OnStart();
        }

        void FixedUpdate()
        {
            if (animator.GetBool("IsDeath"))
            {
                Hpbar.GetComponent<FloatingHpBar>().ApplyDamage(0);
                return;
            }

            if (GetTarget == null)
                SearchForTarget();
            else
            {
                CheckPositionAndFlipUnit();

                if (CheckTargetInAttackRange() && !GetAttackAnimationIsRunning) // Unit is in range, unit start attacking
                {
                    animator.SetBool("IsRunning", false);
                    animator.SetBool("IsAttacking", true);
                    GetTargetAggro = true;
                    GetAttackAnimationIsRunning = true;
                    GetAttackCanDealDamage = true;
                    RangeAttackEventArgs args = new RangeAttackEventArgs
                    {
                        AttackValue = Attack,
                        RotateProjectileSprite = IsFacingToLeft
                    };
                    RangeAttackEvent?.Invoke(this, args);
                }
                else if (!CheckTargetInAttackRange() && GetAttackAnimationIsRunning) // unit is attacking, but target is out of range => cancel attack
                {
                    GetAttackAnimationIsRunning = false;
                    GetAttackCanDealDamage = false;
                    animator.SetBool("IsAttacking", false);
                }

                if (((CheckTargetInAttackRange() || GetTargetAggro) && !GetAttackAnimationIsRunning)) // unit is out of range, unit move to target
                {
                    GetTargetAggro = true;
                    // start running
                    animator.SetBool("IsRunning", true);
                    Move();
                }
            }
        }

        private float GetRangeAttackEndPoint()
        {
            //Transform wepon = GetComponent<Wepon>().transform;
            //switch (IsFacingToLeft)
            //{
            //    default: return wepon.position.x - AttackRange;
            //    case true: return wepon.position.x + AttackRange;
            //}
            return 0f;
        }

        private float GetDistance()
        {
            switch (IsFacingToLeft)
            {
                case true: return transform.position.x - GetTarget.position.x;
                default: return GetTarget.position.x - transform.position.x;
            }
        }

        private bool CheckTargetInAttackRange()
        {
            if (GetDistance() <= (IsFacingToLeft ? AttackRange : AttackRange * -1))
                return true;
            else
                return false;
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
            base.ColissionEnter2D(collision);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            base.ColissionExit2D(collision);
        }
    }
}
