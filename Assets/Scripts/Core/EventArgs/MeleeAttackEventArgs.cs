using System;
using UnityEngine;

namespace Assets.Scripts.Core.eventArgs
{
    public class MeleeAttackEventArgs : EventArgs
    {
        public int AttackValue { get; set; }
        public int AttackRange { get; set; }
        public BoxCollider2D AttackPoint { get; set; }
    }
}
