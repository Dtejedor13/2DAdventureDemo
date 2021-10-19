using System;

namespace Assets.Scripts.Core.eventArgs
{
    public class RangeAttackEventArgs : EventArgs
    {
        public int AttackValue { get; set; }
        public bool RotateProjectileSprite { get; set; }
    }
}
