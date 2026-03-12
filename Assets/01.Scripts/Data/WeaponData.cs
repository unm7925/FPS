
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Datas/WeaponData")]
public class WeaponData : ScriptableObject
{
    public GameObject prefab;

    public Vector3 equipPosition = Vector3.zero;
    public Quaternion equipRotation = Quaternion.identity;
    
    public enum WeaponType { Primary, Secondary, Melee }
    public WeaponType weaponType;
    
    public RuntimeAnimatorController animatorController;
    
    public float reloadTime;
    
    public int magazineSize;
    public int maxReserve;
    
    public float spread;
    public int damage;
    public float range;
    public float fireRate;
    
}
