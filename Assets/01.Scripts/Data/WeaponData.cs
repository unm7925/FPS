
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Datas/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("프리팹")]
    public GameObject prefab;

    [Header("장착 위치")]
    public Vector3 equipPosition = Vector3.zero;
    public Quaternion equipRotation = Quaternion.identity;
    
    public enum WeaponType { Primary, Secondary, Melee }
    public WeaponType weaponType;
    
    
    public RuntimeAnimatorController animatorController;
    
    [Header("총기 스펙")]
    public float reloadTime;
    public int magazineSize;
    public int maxReserve;
    
    public int damage;
    public float range;
    public float fireRate;

    [Header("상태에 따른 탄퍼짐")] 
    public float spreadIdle;
    public float spreadCrouch;
    public float spreadJump;
    public float spreadMove;
    public float maxSpread;

    [Header("연사에 의한 탄퍼짐")]
    public float recoilIncrement;
    public float recoverDelay;
    public float recoverRate;
}
