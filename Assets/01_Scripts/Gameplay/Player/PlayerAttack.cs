using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public PlayerWeaponSO[] playerWeapon;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private LayerMask enemyLayer;
    private Coroutine co1;
    private Coroutine co2;
    private Coroutine co3;
    private Coroutine co4;
    private Coroutine co5;
    private Coroutine co6;

    private Dictionary<string, float> playerStat;

    private bool isCo1 = false;
    private void Awake()
    {
    }
    void Update()
    {
        playerStat = playerController.PlayerStat();
        if (isCo1 == false)
        {
            isCo1 = true;
            co1 = StartCoroutine(Weapon1());
        }
    }
    IEnumerator Weapon1()
    {
        while (true)
        {
            Collider2D collider = Physics2D.OverlapCircle(transform.position, playerWeapon[0].weaponRange + (playerStat["range"] / 100), enemyLayer);
            yield return new WaitForSecondsRealtime(playerWeapon[0].weaponAttackSpeed / ((playerStat["attackSpeed"]) / 100));
        }
    }
    IEnumerator Weapon2()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(playerWeapon[1].weaponAttackSpeed / ((playerStat["attackSpeed"]) / 100));
        }
    }
    IEnumerator Weapon3()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(playerWeapon[2].weaponAttackSpeed / ((playerStat["attackSpeed"]) / 100));
        }
    }
    IEnumerator Weapon4()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(playerWeapon[3].weaponAttackSpeed / ((playerStat["attackSpeed"]) / 100));
        }
    }
    IEnumerator Weapon5()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(playerWeapon[4].weaponAttackSpeed / ((playerStat["attackSpeed"]) / 100));
        }
    }
    IEnumerator Weapon6()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(playerWeapon[5].weaponAttackSpeed / ((playerStat["attackSpeed"]) / 100));
        }
    }

}
