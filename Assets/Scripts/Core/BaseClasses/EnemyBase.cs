using UnityEngine;

namespace Assets.Scripts.Core.BaseClasses
{
    public abstract class EnemyBase : CharacterBase
    {
        public event EventHandler<RangeAttackEventArgs> RangeAttackEvent;

        // Editor
        public Animator animator;
        public RectTransform Hpbar;
        public SpriteRenderer Srenderer;
        public bool IsFacingToLeft;

        // Stats
        public int MaxHP;
        public int Attack;
        public int Defense;
        public int MoveSpeed;
        public int AggroRange;
        public int AttackRange;

        private int currentHP;
        private Transform target;
        private BoxCollider2D playerHitBox;
        private bool IsGrounded;
        [System.NonSerialized] public bool TargetAggro;
        private bool attackAnimationIsRunning;
        private bool attackCanDealDamage;

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
                playerHitBox = collision.collider as BoxCollider2D;
        }

        void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.collider.tag == "Player")
                playerHitBox = null;
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
            print($"Damage done {damage}, Hp bevor damage {currentHP}");
            if (damage <= 0) damage = 1;
            currentHP -= damage;
            if (currentHP <= 0) animator.SetBool("IsDeath", true);
            print($"Hp after damage calc {currentHP}");

            Hpbar.GetComponent<FloatingHpBar>().ApplyDamage(currentHP);
        }

        public void ApplyDamage()
        {
            attackCanDealDamage = false;
            playerHitBox.GetComponent<Player>().TakeDamage(Attack);
        }

        public void SearchForTarget()
        {
            target = GameObject.Find("Player")?.GetComponent<Transform>();
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
            transform.position += MoveCharacter(IsFacingToLeft ? MoveSpeed + -1 : MoveSpeed, 0, 1);
        }

        public bool TargetIsOnTheLeftSide()
        {
            return transform.position.x > target.position.x;
        }

        private float GetDistance()
        {
            switch (IsFacingToLeft)
            {
                case true: return transform.position.x - target.position.x;
                default: return target.position.x - transform.position.x;
            }
        }

        private void AnimationHandler(bool _IsRunning,
        bool _IsAttacking, bool _IsDeath, bool _isJumping, bool _IsGettingDamage)
        {
            animator.SetBool("IsRunning", _IsRunning);
            animator.SetBool("IsJumping", _isJumping);
            animator.SetBool("IsAttacking", _IsAttacking);
            animator.SetBool("IsDeath", _IsDeath);
            animator.SetBool("IsGettingDamage", _IsGettingDamage);
        }
    }
}
