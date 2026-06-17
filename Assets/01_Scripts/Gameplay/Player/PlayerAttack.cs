using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAttack : MonoBehaviour
{
    private PlayerWeaponSO playerWeapon;
    private PlayerController playerController;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private GameObject weaponSprite;
    private Coroutine co;
    private Coroutine attackco;
    

    private Dictionary<string, float> playerStat;

    private bool isCo = false;
    private bool isAttackCo = false;
   

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        playerWeapon = playerController.GetWeapon();
        Vector3 srPosition = transform.position;
        srPosition.x += 0.3f;
        srPosition.y += 0.3f;
        GameObject go = Instantiate(weaponSprite, srPosition, Quaternion.identity, transform);
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        sr.sprite = playerWeapon.weaponIcon;
       

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
                    if (isAttackCo == false)
                    {
                    attackco = StartCoroutine(Attack(collider));
                    }
                    break;
                }
                yield return null;
            }
            yield return new WaitForSecondsRealtime(playerWeapon.weaponAttackSpeed / ((playerStat["attackSpeed"]) / 100));
        }
    }
    
    IEnumerator Attack(Collider2D other)
    {
        if(playerWeapon.weaponType.ToString() == "Sword")
        {

            isAttackCo = true;
            Vector2 nowTrans = transform.localPosition;
            Vector2 attackTrans = other.transform.position;
            transform.position = attackTrans;
            yield return new WaitForSecondsRealtime(0.2f);
            transform.localPosition = nowTrans;
            yield return new WaitForSecondsRealtime(playerWeapon.weaponAttackSpeed / ((playerStat["attackSpeed"]) / 100));
            isAttackCo = false;
            attackco = null;
        }
        
    }
}
