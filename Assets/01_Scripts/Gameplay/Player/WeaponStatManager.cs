using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStatManager : MonoBehaviour
{
    [SerializeField] private List<WeaponStat> weaponStat = new List<WeaponStat>();
    private void Awake()
    {
        weaponStat.Add(new WeaponStat("´Ü°Ë",-1f, 0f, -0.3f, -1f, 5f));
    }
    
}

//public class WeaponStat
//{
//    public string upgradeName;
//    public float upgradeDamage;
//    public float upgradeArmorPiercing;
//    public float upgradeAttackSpeed;
//    public float upgradeRange;
//    public float upgradeCri;

//    public WeaponStat(string uN, float uD, float uAP, float uAS, float uR, float uC)
//    {
//        upgradeDamage = uD;
//        upgradeArmorPiercing = uAP;
//        upgradeAttackSpeed = uAS;
//        upgradeRange = uR;
//        upgradeCri = uC;
//    }
//}

