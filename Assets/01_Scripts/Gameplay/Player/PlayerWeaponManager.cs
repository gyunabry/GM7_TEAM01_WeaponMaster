using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System.IO;
using UnityEngine.UIElements;

public class PlayerWeaponManager : MonoBehaviour
{
    [SerializedDictionary("WeaponType", "Weapon")]
    public SerializedDictionary<PlayerWeaponSO.WeaponType, PlayerWeaponSO> playerWeapon;
    [SerializedDictionary("unWeaponType", "unWeapon")]
    public SerializedDictionary<PlayerWeaponSO.WeaponType, PlayerWeaponSO> unlockWeapon;
    [SerializeField]
    private List<int> unlockCount = new List<int>();
    [SerializeField]
    private List<PlayerWeaponSO.WeaponType> unlockType = new List<PlayerWeaponSO.WeaponType>();
    private int unlockList = 0;
    GameManager gameManager;
    WeaponUnlock wu;
    private void Awake()
    {
        unlockList = 0;
        gameManager = FindAnyObjectByType<GameManager>();
        wu = FindAnyObjectByType<WeaponUnlock>();
        foreach (KeyValuePair<PlayerWeaponSO.WeaponType, PlayerWeaponSO> pW in playerWeapon)
        {
            pW.Value.ResetStatUpgrade();
        }
        foreach (KeyValuePair<PlayerWeaponSO.WeaponType, PlayerWeaponSO> pW in unlockWeapon)
        {
            pW.Value.ResetStatUpgrade();
        }
    }
    private void Update()
    {
        if (unlockCount.Count <= unlockList)
        {
            return;
        }
        else
        {
            Debug.Log(wu.GetUnlock(unlockType[unlockList]));
            if (wu.GetUnlock(unlockType[unlockList]))
            {
                if (unlockWeapon.TryGetValue(unlockType[unlockList], out var nowUnlockWeapon))
                {
                    playerWeapon.TryAdd(unlockType[unlockList], nowUnlockWeapon);
                    unlockList++;
                }
            }
            else if (gameManager.KillCount >= unlockCount[unlockList])
            {
                if (unlockWeapon.TryGetValue(unlockType[unlockList], out var nowUnlockWeapon))
                {
                    playerWeapon.TryAdd(unlockType[unlockList], nowUnlockWeapon);
                    wu.SaveUnlock(unlockType[unlockList], true);
                    unlockList++;
                }
            }
        }
    }
    public PlayerWeaponSO GetWeapon(PlayerWeaponSO.WeaponType type)
    {
        foreach (KeyValuePair<PlayerWeaponSO.WeaponType, PlayerWeaponSO> weapon in playerWeapon)
        {
            if (weapon.Value.weaponType == type)
            {
                return weapon.Value;
            }
        }
        return null;
    }
    public void SetWeapon(PlayerWeaponSO.WeaponType type, PlayerWeaponSO value)
    {
        playerWeapon.TryAdd(type, value);
    }
    public PlayerWeaponSO.WeaponType GetWeaponType(PlayerWeaponSO.WeaponType type)
    {
        foreach (KeyValuePair<PlayerWeaponSO.WeaponType, PlayerWeaponSO> weapon in playerWeapon)
        {
            if (weapon.Value.weaponType == type)
            {
                return weapon.Value.weaponType;
            }
        }
        return PlayerWeaponSO.WeaponType.Null;
    }
    public PlayerWeaponSO[] GetWeaponList()
    {
        return playerWeapon.Values.ToArray();
    }
}