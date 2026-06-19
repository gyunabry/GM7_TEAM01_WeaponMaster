using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName ="scriptableObject/WeaponData")]
public class PlayerWeaponSO : ScriptableObject
{
    public enum WeaponType {Null = -1, Sword, Bow, CrossBow, Shield, Axe, Whip, Spear, Halberd, Hammer}

    [Header("info")]
    public WeaponType weaponType;
    public string weaponName;
    public int weaponId;
    [TextArea]
    public string weaponDes;
    public Sprite weaponIcon;

    [Header("weaponStat")]
    public float weaponDamage;
    public float weaponArmorPiercing;
    public float weaponAttackSpeed;
    public float weaponRange;
    public float weaponCri;

    [Header("UpgradeList")]
    public List<WeaponStat> upgrades;

    [Header("weaponSkill")]
    public bool skill1;
    public bool skill2;
    public bool skill3;
    public bool skill4;

    [Header("weaponRoute")]
    public bool route1;
    public bool route2;
    public bool route3;
    public bool route4;

    public float GetUpgradeDamage(int i)
    {
        return upgrades[i].upgradeDamage;
    }
    public float GetUpgradeArmorPiercing(int i)
    {
        return upgrades[i].upgradeArmorPiercing;
    }
    public float GetUpgradeAttackSpeed(int i)
    {
        return upgrades[i].upgradeAttackSpeed;
    }
    public float GetUpgradeRange(int i)
    {
        return upgrades[i].upgradeRange;
    }
    public float GetUpgradeCri(int i)
    {
        return upgrades[i].upgradeCri;
    }
}
[Serializable]
public class WeaponStat
{
    public string upgradeName;
    public float upgradeDamage;
    public float upgradeArmorPiercing;
    public float upgradeAttackSpeed;
    public float upgradeRange;
    public float upgradeCri;

    public WeaponStat(string uN, float uD, float uAP, float uAS, float uR, float uC)
    {
        upgradeDamage = uD;
        upgradeArmorPiercing = uAP;
        upgradeAttackSpeed = uAS;
        upgradeRange = uR;
        upgradeCri = uC;
    }

}