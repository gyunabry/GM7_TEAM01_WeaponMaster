using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerWeaponSO playerWeapon;
    private PlayerController playerController;
    [SerializeField] private LayerMask enemyLayer;
    private Coroutine co;
    

    private Dictionary<string, float> playerStat;

    private bool isCo = false;
    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        playerWeapon = playerController.GetWeapon();
    }
    void Update()
    {
        playerStat = playerController.PlayerStat();
        if (isCo == false)
        {
            isCo = true;
            co = StartCoroutine(Weapon());
        }
    }
    IEnumerator Weapon()
    {
        while (true)
        {
            while (true)
            {
                Collider2D collider = Physics2D.OverlapCircle(transform.position, playerWeapon.weaponRange + (playerStat["range"] / 100), enemyLayer);
                if (collider != null) { 

                    break;
                }
                yield return null;
            }
            yield return new WaitForSecondsRealtime(playerWeapon.weaponAttackSpeed / ((playerStat["attackSpeed"]) / 100));
        }
        
    }
    
}
