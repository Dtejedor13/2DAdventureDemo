using UnityEngine;

public class FloatingHpBar : MonoBehaviour
{
    public int MaxHP = 1;
    public int HP = 1;
    public RectTransform Healthbar;

    public void ApplyDamage(int newvalue)
    {
        HP = newvalue;
        if (HP <= 0)
        {
            HP = 0;
        }
        else
        {
            float percenage = 200 / MaxHP;
            percenage = percenage * HP;

            Healthbar.sizeDelta = new Vector2(percenage, Healthbar.sizeDelta.y);
        }
    }
}
