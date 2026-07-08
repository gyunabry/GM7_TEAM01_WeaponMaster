using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WeaponUnlock : MonoBehaviour
{
    public WeaponUnlockData wud = new WeaponUnlockData();

    public GameManager gameManager;
    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        Load();
    }

    public bool GetUnlock(PlayerWeaponSO.WeaponType pwsWt)
    {
        if(wud.weaponUnlock.TryGetValue(pwsWt, out var value))
        {
            return value;
        }
        else
        {
            return false;
        }
    }
    public void SaveUnlock(PlayerWeaponSO.WeaponType pwsWt, bool unlock)
    {
        wud.weaponUnlock.Add(pwsWt, unlock);
        wud.weaponUnlockType.Clear();
        wud.weaponUnlockBool.Clear();
        foreach(KeyValuePair<PlayerWeaponSO.WeaponType, bool> pair in wud.weaponUnlock)
        {
            wud.weaponUnlockType.Add(pair.Key);
            wud.weaponUnlockBool.Add(pair.Value);
        }
        string json = JsonUtility.ToJson(wud);
        File.WriteAllText(Application.persistentDataPath + "WeaponUnlockData.json", json);
    }
    public void Save()
    {
        string json = JsonUtility.ToJson(wud);
        File.WriteAllText(Application.persistentDataPath + "WeaponUnlockData.json", json);
    }
    public void Load()
    {
        string path = Application.persistentDataPath + "WeaponUnlockData.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            wud = JsonUtility.FromJson<WeaponUnlockData>(json);
            for (int i = 0; i < wud.weaponUnlockType.Count; i++) 
            {
                wud.weaponUnlock.Add(wud.weaponUnlockType[i], wud.weaponUnlockBool[i]);
            }
            
        }
    }
}
[System.Serializable]
public class WeaponUnlockData
{
    public int killCount;
    public Dictionary<PlayerWeaponSO.WeaponType, bool> weaponUnlock = new Dictionary<PlayerWeaponSO.WeaponType, bool>();
    public List<PlayerWeaponSO.WeaponType> weaponUnlockType = new List<PlayerWeaponSO.WeaponType>();
    public List<bool> weaponUnlockBool = new List<bool>();
}
