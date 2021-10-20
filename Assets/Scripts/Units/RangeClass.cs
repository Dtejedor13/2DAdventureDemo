using Assets.Scripts.Core.BaseClasses;
using Assets.Scripts.Core.eventArgs;
using Assets.Scripts.Core.Interfaces;
using System;

namespace Assets.Scripts.Core
{
    public class RangeClass : EnemyBase, IUnit
    {
        public event EventHandler<RangeAttackEventArgs> RangeAttackEvent;

        void FixedUpdate()
        {
            bool IsRunning = false;
            bool IsAttacking = false;

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

                if (CheckTargetInAttackRange() && !AttackAnimationIsRunning) // Unit is in range, unit start attacking
                {
                    IsAttacking = true;
                    TargetAggro = true;
                    AttackAnimationIsRunning = true;
                    AttackCanDealDamage = true;
                    RangeAttackEventArgs args = new RangeAttackEventArgs
                    {
                        AttackValue = Attack,
                        RotateProjectileSprite = IsFacingToLeft
                    };
                    RangeAttackEvent?.Invoke(this, args);
                }
                else if (!CheckTargetInAttackRange() && AttackAnimationIsRunning) // unit is attacking, but target is out of range => cancel attack
                {
                    AttackAnimationIsRunning = false;
                    AttackCanDealDamage = false;
                }

                if (((CheckTargetInAttackRange() || TargetAggro) && !AttackAnimationIsRunning)) // unit is out of range, unit move to target
                {
                    if (!TargetAggro) TargetAggro = true;
                    // start running
                    IsRunning = true;
                    Move();
                }
            }

            AnimationHandler(IsRunning, IsAttacking, false, null, null);
        }

        private bool CheckTargetInAttackRange()
        {
            if (GetDistance() <= (IsFacingToLeft ? AttackRange : AttackRange * -1))
                return true;
            else
                return false;
        }
    }
}
