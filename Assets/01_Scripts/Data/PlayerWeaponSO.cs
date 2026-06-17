using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName ="scriptableObject/WeaponData")]
public class PlayerWeaponSO : ScriptableObject
{
    public enum WeaponType {Sword, Bow, CrossBow, Shield, Axe, Whip, Spear, Halberd, Hammer}

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

}
