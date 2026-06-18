using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    public PlayerWeaponSO[] playerWeapon;

    public PlayerWeaponSO GetWeapon(string type)
    {
        foreach(PlayerWeaponSO weapon in playerWeapon)
        {
            if(weapon.weaponType.ToString() == type)
            {
                return weapon;
            }
        }
        return null;
    }
}