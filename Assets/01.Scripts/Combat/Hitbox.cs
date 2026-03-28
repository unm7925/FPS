using UnityEngine;

public enum HitboxType
{
    Head,
    Body,
    Limb
}



public class Hitbox:MonoBehaviour
{
    [SerializeField] private HitboxType hitboxType;
    private float damageHeadMul = 4f;
    private float damageBodyMul = 1f;
    private float damageLimbMul = 0.75f;

    public void ApplyDamage(int damage,HP targetHP, GameObject attacker)
    {
        switch(hitboxType) 
        {
            case HitboxType.Head :
                targetHP.TakeDamage((int)(damage * damageHeadMul),attacker) ;
                Debug.Log(damage*damageHeadMul);
                break;
            case HitboxType.Body :
                targetHP.TakeDamage((int)(damage * damageBodyMul),attacker) ;
                Debug.Log(damage*damageBodyMul);
                break;
            case HitboxType.Limb :
                targetHP.TakeDamage((int)(damage * damageLimbMul),attacker) ;
                Debug.Log(damage*damageLimbMul);
                break;
        }
    }

    public int CalcDamage(int baseDamage)
    {
        switch(hitboxType) 
        {
            case HitboxType.Head :
                return (int)(baseDamage * damageHeadMul);
            case HitboxType.Body :
                return (int)(baseDamage * damageBodyMul);
            case HitboxType.Limb :
                return  (int)(baseDamage * damageLimbMul);
        }

        return baseDamage;
    }
}

