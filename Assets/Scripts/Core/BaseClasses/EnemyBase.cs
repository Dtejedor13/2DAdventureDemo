using UnityEngine;

namespace Assets.Scripts.Core.BaseClasses
{
    public abstract class EnemyBase : CharacterBase
    {
        // Editor
        public Animator animator;
        public RectTransform Hpbar;
        public SpriteRenderer Srenderer;
        public bool IsFacingToLeft;

        // Stats
        public int MaxHP;
        public int Attack;
        public int Defense;
        public float MoveSpeed;
        public int AggroRange;
        public int AttackRange;

        private int currentHP;
        private bool isGrounded;
        [System.NonSerialized] public Transform Target;
        [System.NonSerialized] public BoxCollider2D PlayerHitBox;
        [System.NonSerialized] public bool TargetAggro;
        [System.NonSerialized] public bool AttackAnimationIsRunning;
        [System.NonSerialized] public bool AttackCanDealDamage;

        void Update()
        {
            currentHP = MaxHP;
            Hpbar.GetComponent<FloatingHpBar>().HP = currentHP;
            Hpbar.GetComponent<FloatingHpBar>().MaxHP = MaxHP;
            SearchForTarget();
        }

        #region Events
        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.tag == "Player")
                PlayerHitBox = collision.collider as BoxCollider2D;
        }

        void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.collider.tag == "Player")
                PlayerHitBox = null;
        }
        #endregion

        #region Animation Triggers
        public void OnAttackAnimationEnd()
        {
            AttackAnimationIsRunning = false;
            AttackCanDealDamage = false;
        }

        public void OnDeathAnimationEnd()
        {
            Destroy(gameObject);
        }

        public void OnDamageAnimationEnd()
        {
            AnimationHandler(false, false, false, false, false);
        }
        #endregion

        public void TakeDamage(int attackValue)
        {
            bool IsDeath;
            int damage = attackValue - Defense;
            print($"Damage done {damage}, Hp bevor damage {currentHP}");

            if (damage <= 0) damage = 1;
            currentHP -= damage;

            IsDeath = currentHP <= 0;

            print($"Hp after damage calc {currentHP}");

            AnimationHandler(false, false, IsDeath, false, true);
            Hpbar.GetComponent<FloatingHpBar>().ApplyDamage(currentHP);
        }

        public void ApplyDamage()
        {
            AttackCanDealDamage = false;
            PlayerHitBox.GetComponent<Player>().TakeDamage(Attack);
        }

        public void ApplyHealing(int healingValue)
        {
            currentHP += healingValue;
            if (currentHP > MaxHP * 2) currentHP = MaxHP * 2;
            Hpbar.GetComponent<FloatingHpBar>().ApplyDamage(currentHP);
        }

        public void ApplyAttackBoost(int attackValue)
        {
            Attack += attackValue;
        }

        public void ApplyDefenseBoost(int defenseValue)
        {
            Defense += defenseValue;
        }

        public void SearchForTarget()
        {
            Target = GameObject.Find("Player")?.GetComponent<Transform>();
        }

        public void CheckPositionAndFlipUnit()
        {
            if ((IsFacingToLeft && !TargetIsOnTheLeftSide()) || (!IsFacingToLeft && TargetIsOnTheLeftSide()))
            {
                transform.Rotate(0, 180f, 0);
                IsFacingToLeft = !IsFacingToLeft;
            }
        }

        public void Move()
        {
            // transform.position += new Vector3((IsFacingToLeft ? MoveSpeed * -1 : MoveSpeed), 0, 0) * (0.025f * 1);
            // transform.position += BaseFunctions.MoveCharacter(IsFacingToLeft ? MoveSpeed * -1 : MoveSpeed, 0, 1);
            transform.position += MoveCharacter((IsFacingToLeft ? MoveSpeed * -1 : MoveSpeed), 0);
        }

        public bool TargetIsOnTheLeftSide()
        {
            return transform.position.x > Target.position.x;
        }

        public float GetDistance()
        {
            switch (IsFacingToLeft)
            {
                case true: return transform.position.x - Target.position.x;
                default: return Target.position.x - transform.position.x;
            }
        }

        public void AnimationHandler(bool _IsRunning,
        bool _IsAttacking, bool _IsDeath, bool? _isJumping, bool? _IsGettingDamage)
        {
            animator.SetBool("IsRunning", _IsRunning);
            animator.SetBool("IsAttacking", _IsAttacking);
            animator.SetBool("IsDeath", _IsDeath);
            if(_isJumping != null)
                animator.SetBool("IsJumping", (bool)_isJumping);
            if(_IsGettingDamage != null)
                animator.SetBool("IsGettingDamage", (bool)_IsGettingDamage);
        }
    }
}
