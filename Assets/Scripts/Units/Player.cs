using Assets.Scripts.Core.eventArgs;
using Assets.Scripts.Core.Interfaces;
using System;
using UnityEngine;

public class Player : MonoBehaviour, IUnit
{
    public Animator animator;
    public SpriteRenderer renderer;
    public int HP { get; set; }

    private int baseAttack = 4;
    private int attack;
    private int defense = 2;

    private BoxCollider2D enemyHitColider;
    public RectTransform Hpbar;
    bool isGrounded;
    float movmentSpeed = .5f;
    float jumpPower = 3f;
    bool attackAnimationIsRunning;
    bool attackCanDealDamage;

    public event EventHandler<RangeAttackEventArgs> RangeAttackEvent;

    private void Start()
    {
        HP = 100;
        Hpbar.GetComponent<FloatingHpBar>().HP = HP;
        Hpbar.GetComponent<FloatingHpBar>().MaxHP = HP;
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
        bool wasFlyng = animator.GetBool("IsInAir");
        animator.SetBool("IsInAir", !isGrounded);

        if (wasFlyng && isGrounded)
        {
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsLanding", true);
        }

        if (Input.GetKey(KeyCode.D) && !attackAnimationIsRunning)
        {
            Move(movmentSpeed, 0, animator.GetBool("IsCrouching"));
            renderer.flipX = false;
            animator.SetBool("IsRunning", true);
        }
        if (Input.GetKey(KeyCode.A) && !attackAnimationIsRunning)
        {
            Move(movmentSpeed * -1, 0, animator.GetBool("IsCrouching"));
            animator.SetBool("IsRunning", true);
            renderer.flipX = true;
        }
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !attackAnimationIsRunning)
        {
            Jump();
            animator.SetBool("IsJumping", true);
            animator.SetBool("IsLanding", false);
        }
        if (Input.GetKey(KeyCode.S) && isGrounded && !attackAnimationIsRunning)
        {
            animator.SetBool("IsCrouching", true);
        }
        if ((Input.GetKeyDown(KeyCode.E) || Input.GetKey(KeyCode.Q)) && isGrounded)
        {
            bool attack2 = Input.GetKey(KeyCode.Q);
            if (!attackAnimationIsRunning)
            {
                animator.SetBool((attack2? "IsAttacking2" : "IsAttacking"), true);
                attackAnimationIsRunning = true;
                attackCanDealDamage = true;
                attack = baseAttack * (attack2 ? 6 : 3);
            }
        }

        // cancel Animations
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            animator.SetBool("IsRunning", false);
        if (!Input.GetKey(KeyCode.S))
            animator.SetBool("IsCrouching", false);

    }

    private void FixedUpdate()
    {
        if (attackAnimationIsRunning && attackCanDealDamage && enemyHitColider != null)
        {
            System.Random random = new System.Random();
            if (random.Next(1, 101) + 20 >= 100)
            {
                attack = (int)Math.Round(attack * 1.5, 0);
                //Debug.Log("Crittical Hit!");
            }
            enemyHitColider.GetComponent<IUnit>().TakeDamage(attack);
            attackCanDealDamage = false;
        }
    }

    private void Move(float x, float y, bool slide)
    {
        transform.position += new Vector3(x, y, 0) * (0.025f * (slide ? 1 : 2));
    }

    private void Jump()
    {
        transform.Translate(Vector3.up * jumpPower * 1.2f, Space.World);
    }

    public void TakeDamage(int attackValue)
    {
        animator.SetBool("IsTakingDamage", true);
        int damage = attackValue - defense;
        if (damage <= 0) damage = 1;
        HP -= damage;
        if (HP <= 0) HP = 0;
        // Set Value to Hp bar
        Hpbar.GetComponent<FloatingHpBar>().ApplyDamage(HP);
        if (HP == 0) 
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
        throw new NotImplementedException();
    }
}
