using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    public PlayerWeaponSO[] playerWeapon;
    private void Awake()
    {
        foreach (PlayerWeaponSO pW in playerWeapon)
        {
            pW.ResetStatUpgrade();
        }
    }

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
    public PlayerWeaponSO[] GetWeaponList()
    {
        return playerWeapon;
    }
}