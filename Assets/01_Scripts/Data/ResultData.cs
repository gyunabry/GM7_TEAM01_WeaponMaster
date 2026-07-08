using System.Collections.Generic;
using UnityEngine;

public class ResultWeaponData
{
    public string weaponName;
    public Sprite icon;
    public int level;
    public int damage;
}

public class ResultData
{
    public string stageName;
    public Difficulty difficulty;
    public float clearTime;
    public int playerLevel;
    public int killCount;
    public List<ResultWeaponData> weapons;
}
