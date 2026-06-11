using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapon System/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public float fireRate;     
    public float damage;      
    public float projectileSpeed; 
    public float heatPerShot;  
    public float coolDownRate; 
    public int bulletsPerShot; 
    public float spreadAngle;   
    public string bulletTag;    
}