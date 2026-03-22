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

    public void ApplyDamage(int damage,HP targetHP)
    {
        
        switch(hitboxType) 
        {
            case HitboxType.Head :
                targetHP.TakeDamage((int)(damage * damageHeadMul),gameObject) ;
                Debug.Log(damage*damageHeadMul);
                break;
            case HitboxType.Body :
                targetHP.TakeDamage((int)(damage * damageBodyMul),gameObject) ;
                Debug.Log(damage*damageBodyMul);
                break;
            case HitboxType.Limb :
                targetHP.TakeDamage((int)(damage * damageLimbMul),gameObject) ;
                Debug.Log(damage*damageLimbMul);
                break;
        }
    }
}

