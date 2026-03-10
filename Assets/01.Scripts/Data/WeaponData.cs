
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Datas/WeaponData")]
public class WeaponData : ScriptableObject
{
    
    public RuntimeAnimatorController animatorController;
    
    public float reloadTime;
    
    public int magazineSize;
    public int maxReserve;
    
    public float spread;
    public float damage;
    public float range;
    public float fireRate;
    
}
