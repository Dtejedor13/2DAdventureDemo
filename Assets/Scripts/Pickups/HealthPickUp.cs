using Assets.Scripts.Core.Interfaces;
using UnityEngine;

public class HealthPickUp : MonoBehaviour, IPickUp
{
    public int PickupValue { get; set; }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.tag == "Player")
        {
            collider.GetComponent<IUnit>().ApplyHealing(PickupValue);
            Destroy(gameObject);
        }
    }
}
