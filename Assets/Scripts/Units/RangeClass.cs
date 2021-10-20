﻿using Assets.Scripts.Core.BaseClasses;
using Assets.Scripts.Core.eventArgs;
using Assets.Scripts.Core.Interfaces;
using System;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class RangeClass : EnemyBase, IUnit
    {
        void FixedUpdate()
        {
            bool IsRunning;
            bool IsAttacking;

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

                if (CheckTargetInAttackRange() && !attackAnimationIsRunning) // Unit is in range, unit start attacking
                {
                    IsAttacking = true;
                    targetAggro = true;
                    attackAnimationIsRunning = true;
                    attackCanDealDamage = true;
                    RangeAttackEventArgs args = new RangeAttackEventArgs
                    {
                        AttackValue = Attack,
                        RotateProjectileSprite = IsFacingToLeft
                    };
                    RangeAttackEvent?.Invoke(this, args);
                }
                else if (!CheckTargetInAttackRange() && attackAnimationIsRunning) // unit is attacking, but target is out of range => cancel attack
                {
                    attackAnimationIsRunning = false;
                    attackCanDealDamage = false;
                }

                if (((CheckTargetInAttackRange() || targetAggro) && !attackAnimationIsRunning)) // unit is out of range, unit move to target
                {
                    if (!TargetAggro) TargetAggro = true;
                    // start running
                    IsRunning = true;
                    Move();
                }
            }

            AnimationHandler(IsRunning, IsAttacking, false, false, false);
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
