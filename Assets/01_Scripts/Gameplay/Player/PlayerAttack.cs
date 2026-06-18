using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAttack : MonoBehaviour
{
    private PlayerWeaponSO playerWeapon;
    private PlayerController playerController;
    private ArrowPooling arrowPooling;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private GameObject weaponSprite;
    private Coroutine co;
    private Coroutine attackco;
    

    private Dictionary<string, float> playerStat;

    private bool isCo = false;
    private bool isAttackCo = false;
   

    private void Awake() //무기 생성 부분 UI완성시 바꿀것
    {
        arrowPooling = FindFirstObjectByType<ArrowPooling>();
        playerController = GetComponentInParent<PlayerController>();
        playerWeapon = playerController.GetWeapon();
        Vector3 srPosition = transform.position;
        srPosition.x += 0.3f;
        GetPlayerStat();
        GameObject go = Instantiate(weaponSprite, srPosition, Quaternion.Euler(0f, 0f, -45f), transform);
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        sr.sprite = playerWeapon.weaponIcon;
       

    }
    void Update()
    {
        if (isCo == false)
        {
            isCo = true;
            co = StartCoroutine(Weapon());
        }
    }
    public void GetPlayerStat()
    {
        playerStat = playerController.PlayerStat();
    }
    IEnumerator Weapon()
    {
        while (true)
        {
            while (true)
            {
                Collider2D collider = Physics2D.OverlapCircle(transform.position, playerWeapon.weaponRange + (playerStat["range"] / 100), enemyLayer);
                if (collider != null) {
                    transform.rotation = Quaternion.Euler(0, 0, LookEnemy(collider));
                    if (isAttackCo == false)
                    {
                        attackco = StartCoroutine(Attack(collider));
                    }
                    break;
                }
                yield return null;
            }
            yield return null;
        }
    }

    public float LookEnemy(Collider2D collider)
    {
        Vector2 newrot = collider.transform.position - transform.position;
        float rotz = Mathf.Atan2(newrot.y, newrot.x) * Mathf.Rad2Deg;
        return rotz;
    }

    IEnumerator Attack(Collider2D other)
    {
        if(playerWeapon.weaponType.ToString() == "Sword")
        {
            isAttackCo = true;
            Vector2 nowTrans = transform.localPosition;
            Vector2 direction = other.transform.position - transform.position;
            Vector2 targetPosition = (Vector2)other.transform.position - (direction *0.3f);
            transform.position = targetPosition;
            yield return new WaitForSecondsRealtime(0.2f);
            transform.localPosition = nowTrans;
            yield return new WaitForSecondsRealtime(playerWeapon.weaponAttackSpeed / ((playerStat["attackSpeed"]) / 100));
            isAttackCo = false;
            attackco = null;
        }
        else if(playerWeapon.weaponType.ToString() == "Bow")
        {
            isAttackCo = true;
            Vector2 direction = other.transform.position - transform.position;
            float rotz = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg -90f;
            Arrow arrow = arrowPooling.ArrowPool();
            arrow.transform.position = transform.position;
            arrow.transform.Rotate(0f, 0f, rotz + 45f);
            yield return new WaitForSecondsRealtime(playerWeapon.weaponAttackSpeed / ((playerStat["attackSpeed"]) / 100));
            isAttackCo = false;
            attackco = null;
        }
    }
}
