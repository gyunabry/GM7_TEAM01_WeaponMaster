using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    public PlayerWeaponSO[] playerWeapon;

    public PlayerWeaponSO GetWeapon(PlayerWeaponSO.WeaponType type)
    {
        foreach(PlayerWeaponSO weapon in playerWeapon)
        {
            if(weapon.weaponType == type)
            {
                return weapon;
            }
        }
        return null;
    }
    public PlayerWeaponSO.WeaponType GetWeaponType(PlayerWeaponSO.WeaponType type)
    {
        foreach(PlayerWeaponSO weapon in playerWeapon)
        {
            if(weapon.weaponType == type)
            {
                return weapon.weaponType;
            }
        }
        return PlayerWeaponSO.WeaponType.Null;
    }
}