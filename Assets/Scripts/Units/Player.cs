using Assets.Scripts.Core.eventArgs;
using Assets.Scripts.Core.BaseClasses;
using Assets.Scripts.Core.Interfaces;
using System;
using UnityEngine;

public class Player : CharacterBase, IUnit
{
    public event EventHandler<RangeAttackEventArgs> RangeAttackEvent;

    public Animator animator;
    public RectTransform Hpbar;
    public SpriteRenderer renderer;

    public int MaxHP;
    public float MovmentSpeed = .5f;
    public float JumpPower = 3f;
    public int Attack;
    public int defense;

    private int currentHP;
    private int attackPower;
    private BoxCollider2D enemyHitColider;
    private bool isGrounded;
    private bool attackAnimationIsRunning;
    private bool attackCanDealDamage;

    private void Start()
    {
        currentHP = MaxHP;
        Hpbar.GetComponent<FloatingHpBar>().HP = currentHP;
        Hpbar.GetComponent<FloatingHpBar>().MaxHP = currentHP;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Enviroment")
            isGrounded = true;
        else if (collision.collider.tag == "Enemy")
            enemyHitColider = collision.collider as BoxCollider2D;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Enviroment")
            isGrounded = false;
        else if (collision.collider.tag == "Enemy")
            enemyHitColider = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetBool("IsTakingDamage")) return;

        bool IsJumping;
        bool IsLanding;
        bool IsRunning;
        bool IsCrouching;
        int IsAttacking = 0;

        bool wasFlyng = animator.GetBool("IsInAir");
        if (wasFlyng && isGrounded)
            IsLanding = true;

        if (Input.GetKey(KeyCode.D) && !attackAnimationIsRunning)
        {
            Move(MovmentSpeed, 0, animator.GetBool("IsCrouching"));
            renderer.flipX = false;
            IsRunning = true;
        }
        if (Input.GetKey(KeyCode.A) && !attackAnimationIsRunning)
        {
            Move(MovmentSpeed * -1, 0, animator.GetBool("IsCrouching"));
            IsRunning = true;
            renderer.flipX = true;
        }
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !attackAnimationIsRunning)
            IsJumping = true;

        if (Input.GetKey(KeyCode.S) && isGrounded && !attackAnimationIsRunning)
            IsCrouching = true;

        if ((Input.GetKeyDown(KeyCode.E) || Input.GetKey(KeyCode.Q)) && isGrounded)
        {
            bool attack2 = Input.GetKey(KeyCode.Q);
            if (!attackAnimationIsRunning)
            {
                IsAttacking = (attack2 ? 2 : 1);
                attackAnimationIsRunning = true;
                attackCanDealDamage = true;
                attackPower = Attack * (attack2 ? 6 : 3);
            }
        }

        AnimationHandler(IsRunning, false, false, IsCrouching, IsLanding, IsAttacking);
    }

    private void FixedUpdate()
    {
        if (attackAnimationIsRunning && attackCanDealDamage && enemyHitColider != null)
        {
            System.Random random = new System.Random();
            if (random.Next(1, 101) + 20 >= 100)
            {
                attackPower = (int)Math.Round(attackPower * 1.5, 0);
                //Debug.Log("Crittical Hit!");
            }
            ApplyDamage(attackPower);
        }
    }

    private void Move(float x, float y, bool slide)
    {
        // transform.position += new Vector3(x, y, 0) * (0.025f * (slide ? 1 : 2));
        // transform.position = BaseFunctions.MoveCharacter(x, y, 0, JumpPower, MovmentSpeed, (slide ? 2 : 1));
        transform.position += MoveCharacter(x, y, JumpPower, MovmentSpeed, (slide ? 2 : 1));
    }

    private void Jump()
    {
        transform.Translate(Vector3.up * JumpPower * 1.2f, Space.World);
    }

    public void TakeDamage(int attackValue)
    {
        animator.SetBool("IsTakingDamage", true);
        int damage = attackValue - defense;
        if (damage <= 0) damage = 1;
        currentHP -= damage;
        if (currentHP <= 0) currentHP = 0;
        // Set Value to Hp bar
        Hpbar.GetComponent<FloatingHpBar>().ApplyDamage(currentHP);
        if (currentHP == 0)
            animator.SetBool("IsDeath", true);
    }

    public void AttackAnimationEnds()
    {
        attackAnimationIsRunning = false;
        attackCanDealDamage = false;
        animator.SetBool("IsAttacking", false);
        animator.SetBool("IsAttacking2", false);
    }

    public void OnDeathAnimationEnd()
    {
        Destroy(gameObject);
    }

    public void OnHurtAnimationEnd()
    {
        animator.SetBool("IsTakingDamage", false);
    }

    public void ApplyDamage()
    {
        enemyHitColider.GetComponent<IUnit>().TakeDamage(attackPower);
        attackCanDealDamage = false;
    }

    private void AnimationHandler(bool IsRunning, bool IsTakingDamage,
    bool IsDeath, bool IsCrouching, bool IsLanding, int IsAttacking = 0)
    {
        animator.SetBool("IsAttacking", IsAttacking == 1);
        animator.SetBool("IsAttacking2", IsAttacking == 2);
        animator.SetBool("IsTakingDamage", IsTakingDamage);
        animator.SetBool("IsDeath", IsDeath);
        animator.SetBool("IsCrouching", IsCrouching);
        animator.SetBool("IsLanding", IsLanding);
        animator.SetBool("IsInAir", !isGrounded);
    }
}