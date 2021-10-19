namespace Assets.Scripts.Core.Interfaces
{
    public interface IProjectile
    {
        int AttackValue { get; set; }
        float Speed { get; set; }
        void SetParams(int attack, float speed);
    }
}
