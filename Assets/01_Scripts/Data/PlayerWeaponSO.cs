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
    public float weaponSize;

    [Header("UpgradeList")]
    public List<WeaponStat> upgrades;
    public WeaponStatUpValue statUpValue = new WeaponStatUpValue();
    public WeaponStatUpgrade statUpgrades = new WeaponStatUpgrade();
    public int upgradeNum;
    public int upgradeCount = 0;

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
    public float GetUpgradeSize(int i)
    {
        return upgrades[i].upgradeSize;
    }
    public Sprite GetUpgradeSprite(int i)
    {
        return upgrades[i].upgradeSprite;
    }
    //단순 스탯 업그레이드
    public void AddStatUpgradeDamage(float i)
    {
        statUpgrades.nowUpgradeDamage += i;
    }
    public void AddStatUpgradeArmorPiercing(float i)
    {
        statUpgrades.nowUpgradeArmorPiercing += i;
    }
    public void AddStatUpgradeAttackSpeed(float i)
    {
        statUpgrades.nowUpgradeAttackSpeed += i;
    }
    public void AddStatUpgradeRange(float i)
    {
        statUpgrades.nowUpgradeRange += i;
    }
    public void AddStatUpgradeCri(float i)
    {
        statUpgrades.nowUpgradeCri += i;
    }
    public void AddStatUpgradeSize(float i)
    {
        statUpgrades.nowUpgradeSize += i;
    }
    //증가한 단순 스탯 가져오기
    public float GetStatUpgradeDamage()
    {
        return statUpgrades.nowUpgradeDamage;
    }
    public float GetStatUpgradeArmorPiercing()
    {
        return statUpgrades.nowUpgradeArmorPiercing;
    }
    public float GetStatUpgradeAttackSpeed()
    {
        return statUpgrades.nowUpgradeAttackSpeed;
    }
    public float GetStatUpgradeRange()
    {
        return statUpgrades.nowUpgradeRange;
    }
    public float GetStatUpgradeCri()
    {
        return statUpgrades.nowUpgradeCri;
    }
    public float GetStatUpgradeSize()
    {
        return statUpgrades.nowUpgradeSize;
    }
    public void ResetStatUpgrade() //게임 재시작 할때 무조건 실행
    {
        statUpgrades.nowUpgradeDamage = 0;
        statUpgrades.nowUpgradeArmorPiercing = 0;
        statUpgrades.nowUpgradeAttackSpeed = 0;
        statUpgrades.nowUpgradeRange = 0;
        statUpgrades.nowUpgradeCri = 0;
        statUpgrades.nowUpgradeSize = 0;
        upgradeCount = 0;
    }
    //업그레이드 수치 가져오기
    public float GetUpValueDamage()
    {
        return statUpValue.upgradeStatDamage;
    }
    public float GetUpValueArmorPiercing()
    {
        return statUpValue.upgradeStatArmorPiercing;
    }      
    public float GetUpValueAttackSpeed()
    {
        return statUpValue.upgradeStatAttackSpeed;
    }      
    public float GetUpValueRange()
    {
        return statUpValue.upgradeStatRange;
    }      
    public float GetUpValueCri()
    {
        return statUpValue.upgradeStatCri;
    }      
    public float GetUpValueSize()
    {
        return statUpValue.upgradeStatSize;
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
    public float upgradeSize;
    [TextArea]
    public string upgradeDes;
    public Sprite upgradeSprite;
}
public class WeaponStatUpgrade
{
    public float nowUpgradeDamage;
    public float nowUpgradeArmorPiercing;
    public float nowUpgradeAttackSpeed;
    public float nowUpgradeRange;
    public float nowUpgradeCri;
    public float nowUpgradeSize;
}
[Serializable]
public class WeaponStatUpValue
{
    public float upgradeStatDamage;
    public float upgradeStatArmorPiercing;
    public float upgradeStatAttackSpeed;
    public float upgradeStatRange;
    public float upgradeStatCri;
    public float upgradeStatSize;
}