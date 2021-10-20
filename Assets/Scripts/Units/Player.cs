using Assets.Scripts.Core.eventArgs;
using Assets.Scripts.Core.BaseClasses;
using Assets.Scripts.Core.Interfaces;
using System;
using UnityEngine;
using Assets.Scripts.Core;

public class Player : CharacterBase, IUnit
{
    public event EventHandler<RangeAttackEventArgs> RangeAttackEvent;
    public event EventHandler<MeleeAttackEventArgs> MeleeAttackEvent;

    public Animator animator;
    public RectTransform Hpbar;

    public int MaxHP;
    public float MovmentSpeed = 1.5f;
    public float JumpPower = 3f;
    public int Attack;
    public int Defense;

    private int currentHP;
    private int attackPower;
    private BoxCollider2D enemyHitColider;
    private bool attackAnimationIsRunning;
    private bool attackCanDealDamage;
    private bool isFlying = true;
    private bool isFacingToLeft = false;

    private void Start()
    {
        currentHP = MaxHP;
        Hpbar.GetComponent<FloatingHpBar>().HP = currentHP;
        Hpbar.GetComponent<FloatingHpBar>().MaxHP = currentHP;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Enemy")
            enemyHitColider = collision.collider as BoxCollider2D;

        if (collision.collider.tag == "Envoriment" && isFlying)
        {
            // character is landing
            animator.SetBool("IsInAir", false);
            animator.SetBool("IsLanding", true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Enemy")
            enemyHitColider = null;
    }

    private bool GroundCheck()
    {
        return transform.Find("GroundCheck").GetComponent<GroundCheck>().IsGrounded;
    }

    private void FixedUpdate()
    {
        bool isRunning = false;
        bool isJumping = false;
        bool isCrouching = false;
        bool isFalling = false;
        bool isLanding = false;
        int isAttacking = 0;

        bool isGrounded = GroundCheck();

        Debug.Log(isGrounded);

        if (isGrounded && !attackAnimationIsRunning)
        {
            if (Input.GetKey(KeyCode.E))
            {
                attackAnimationIsRunning = true;
                attackCanDealDamage = true;
                attackPower = Attack * 3;
                isAttacking = 1;
            }
            else if (Input.GetKey(KeyCode.Q))
            {
                attackAnimationIsRunning = true;
                attackCanDealDamage = true;
                attackPower = Attack * 6;
                isAttacking = 2;
            }
        }

        if (isAttacking == 0)
        {
            if (Input.GetKey(KeyCode.D) && !attackAnimationIsRunning)
            {
                Move(MovmentSpeed, 0, animator.GetBool("IsCrouching"));
                isRunning = true;
                RotateUnit(true);
            }
            if (Input.GetKey(KeyCode.A) && !attackAnimationIsRunning)
            {
                Move(MovmentSpeed * -1, 0, animator.GetBool("IsCrouching"));
                isRunning = true;
                RotateUnit(false);
            }
        }

        if (isAttacking == 0 && isGrounded)
        {
            if (Input.GetKey(KeyCode.Space) && isGrounded && !attackAnimationIsRunning)
            {
                isJumping = true;
                isFalling = true;
                isLanding = false;
                Jump();
            }
            if (Input.GetKey(KeyCode.S) && isGrounded && !attackAnimationIsRunning)
            {
                isCrouching = true;
            }
        }

        AnimationHandler(isRunning, isCrouching, isJumping, isFalling, isLanding, false, false, isAttacking);

        if (attackAnimationIsRunning && attackCanDealDamage && enemyHitColider != null)
        {
            System.Random random = new System.Random();
            if (random.Next(1, 101) + 20 >= 100)
                attackPower = (int)Math.Round(attackPower * 1.5, 0);
            ApplyDamage();
        }
    }

    private void Move(float x, float y, bool slide)
    {
        transform.position += new Vector3(x, y, 0) * (0.025f * (slide ? 1 : 2));
    }

    private void Jump()
    {
        transform.Translate(Vector3.up * JumpPower * 1.2f, Space.World);
    }

    public void TakeDamage(int attackValue)
    {
        int damage = attackValue - Defense;
        if (damage <= 0) damage = 1;
        currentHP -= damage;
        if (currentHP <= 0) currentHP = 0;
        // Set Value to Hp bar
        Hpbar.GetComponent<FloatingHpBar>().ApplyDamage(currentHP);
        AnimationHandler(false, false, false, false, false, true, currentHP == 0);
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

    void RotateUnit(bool toRight)
    {
        if (isFacingToLeft == toRight)
        {
            GetComponent<SpriteRenderer>().flipX = !isFacingToLeft;
            isFacingToLeft = !isFacingToLeft;
        }
    }

    private void AnimationHandler(bool isRunning, bool isCrouching, bool isJumping,
        bool isFalling, bool isLanding, bool isGettingDamage, bool isDeath, int isAttacking = 0)
    {
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsInAir", isFalling);
        animator.SetBool("IsLanding", isLanding);
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsCrouching", isCrouching);
        animator.SetBool("IsAttacking", isAttacking == 1);
        animator.SetBool("IsAttacking2", isAttacking == 2);
        animator.SetBool("IsTakingDamage", isGettingDamage);
        animator.SetBool("IsDeath", isDeath);
    }
}