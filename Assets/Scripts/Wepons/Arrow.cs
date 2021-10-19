using Assets.Scripts.Core.Interfaces;
using UnityEngine;

public class Arrow : MonoBehaviour
{

    public float Speed = 20f;
    public Rigidbody2D rb;

    private int attackValue;
    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.right * Speed;    
    }

    public void SetParams(int attackValue)
    {
        this.attackValue = attackValue;
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        hitInfo.GetComponent<IUnit>().TakeDamage(attackValue);
        Destroy(gameObject);
    }
}
