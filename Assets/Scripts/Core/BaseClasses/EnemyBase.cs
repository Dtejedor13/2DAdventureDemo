using UnityEngine;

namespace Assets.Scripts.Core.BaseClasses
{
    public abstract class EnemyBase : MonoBehaviour
    {
        public Animator animator;
        public RectTransform Hpbar;
        public SpriteRenderer Srenderer;

        public int HP { get; set; }
        public int MaxHP;
        public int Attack;
        public int Defense;
        public int MoveSpeed;
        public int TargetRange;
        public int AttackRange;
        public bool IsFacingToLeft = false;

        private Transform target;
        private BoxCollider2D PlayerHitBox;
        private bool targetAggro;
        private bool attackAnimationIsRunning;
        private bool attackCanDealDamage;

        public void OnStart()
        {
            HP = MaxHP;
            Hpbar.GetComponent<FloatingHpBar>().HP = HP;
            Hpbar.GetComponent<FloatingHpBar>().MaxHP = MaxHP;
            SearchForTarget();
        }

        #region Events
        public void ColissionEnter2D(Collision2D collision)
        {
            if (collision.collider.tag == "Player")
                PlayerHitBox = collision.collider as BoxCollider2D;
        }

        public void ColissionExit2D(Collision2D collision)
        {
            if (collision.collider.tag == "Player")
                PlayerHitBox = null;
        }
        #endregion

        #region Animation Triggers
        public void OnAttackAnimationEnd()
        {
            attackAnimationIsRunning = false;
            attackCanDealDamage = false;
        }

        public void OnDeathAnimationEnd()
        {
            Destroy(gameObject);
        }

        public void OnDamageAnimationEnd()
        {
            animator.SetBool("IsGettingDamage", false);
        }
        #endregion

        public void TakeDamage(int attackValue)
        {
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsAttacking", false);
            animator.SetBool("IsGettingDamage", true);
            int damage = attackValue - Defense;
            if (damage <= 0) damage = 1;
            HP -= damage;
            if (HP <= 0) animator.SetBool("IsDeath", true);

            Hpbar.GetComponent<FloatingHpBar>().ApplyDamage(HP);
        }

        public void ApplyDamage()
        {
            attackCanDealDamage = false;
            PlayerHitBox.GetComponent<Player>().TakeDamage(Attack);
        }

        public void SearchForTarget()
        {
            target = GameObject.Find("Player")?.GetComponent<Transform>();
        }

        public void CheckPositionAndFlipUnit()
        {
            if ((IsFacingToLeft && !TargetIsOnTheLeftSide()) ||
                        (!IsFacingToLeft && TargetIsOnTheLeftSide()))
            {
                transform.Rotate(0, 180f, 0);

                IsFacingToLeft = !IsFacingToLeft;
            }
        }

        public void Move()
        {
            transform.position += new Vector3((IsFacingToLeft ? MoveSpeed * -1 : MoveSpeed), 0, 0) * (0.025f * 1);
        }

        public bool TargetIsOnTheLeftSide()
        {
            return transform.position.x > GetTarget.position.x;
        }

        #region Getters
        public Transform GetTarget { get => target; }
        public BoxCollider2D GetPlayerHitBox { get => PlayerHitBox; }
        public bool GetTargetAggro { get => targetAggro; set => targetAggro = value; }
        public bool GetAttackAnimationIsRunning { get => attackAnimationIsRunning; set => attackAnimationIsRunning = value; }
        public bool GetAttackCanDealDamage { get => attackCanDealDamage; set => attackCanDealDamage = value; }
        #endregion
    }
}
