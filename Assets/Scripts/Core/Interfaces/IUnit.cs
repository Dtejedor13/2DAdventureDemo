using Assets.Scripts.Core.eventArgs;
using System;

namespace Assets.Scripts.Core.Interfaces
{
    interface IUnit
    {
        int HP { get; set; }
        void TakeDamage(int attackValue);
        event EventHandler<RangeAttackEventArgs> RangeAttackEvent;
        void ApplyDamage();
    }
}
