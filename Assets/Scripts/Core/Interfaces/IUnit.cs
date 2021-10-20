using Assets.Scripts.Core.eventArgs;
using System;

namespace Assets.Scripts.Core.Interfaces
{
    interface IUnit
    {
        event EventHandler<RangeAttackEventArgs> RangeAttackEvent;
        void TakeDamage(int attackValue);
        void ApplyDamage();
    }
}
