using Assets.Scripts.Core.Interfaces;
using UnityEngine;

public class Wepon : MonoBehaviour
{
    public Transform Firepoint;
    public GameObject ProjectilePrefab;

    void Start()
    {
        GetComponent<IUnit>().RangeAttackEvent += Wepon_RangeAttackEvent;
    }

    private void Wepon_RangeAttackEvent(object sender, Assets.Scripts.Core.eventArgs.RangeAttackEventArgs e)
    {
        Quaternion rotation = e.RotateProjectileSprite ? Quaternion.Euler(0f, 180f, 0f) : new Quaternion();
        Instantiate(ProjectilePrefab, Firepoint.position, rotation);
    }

    void OnDestroy()
    {
        GetComponent<IUnit>().RangeAttackEvent -= Wepon_RangeAttackEvent;
    }
}
