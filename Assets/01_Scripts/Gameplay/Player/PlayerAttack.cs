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
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private GameObject weaponSprite;
    [SerializeField] private GameObject arrowPrefab;
    private Coroutine co;
    private Coroutine attackco;

    public Collider2D colliderA = null;

    private Dictionary<string, float> playerStat;

    private bool isCo = false;
    private bool isAttackCo = false;
   

    private void Awake() //무기 생성 부분 UI완성시 바꿀것
    {
        playerController = GetComponentInParent<PlayerController>();
        playerWeapon = playerController.GetWeapon();
        Vector3 srPosition = transform.position;
        srPosition.x += 0.3f;
        
        GameObject go = Instantiate(weaponSprite, srPosition, Quaternion.Euler(0f, 0f, -45f), transform);
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
                colliderA = Physics2D.OverlapCircle(transform.position, playerWeapon.weaponRange + (playerStat["range"] / 100), enemyLayer);
                if (colliderA != null) {
                    transform.rotation = Quaternion.Euler(0, 0, LookEnemy(colliderA));
                    if (isAttackCo == false)
                    {
                        attackco = StartCoroutine(Attack(colliderA));
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
            Vector2 direction = other.transform.position - transform.position.normalized;
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
<<<<<<< HEAD
            Vector2 direction = (other.transform.position - transform.position).normalized;
            SpawnProjectile(direction);
            yield return new WaitForSecondsRealtime(playerWeapon.weaponAttackSpeed / ((playerStat["attackSpeed"]) / 100));
            isAttackCo = false;
            attackco = null;
=======

>>>>>>> parent of c78d4e96 (feat: bow & stat)
        }
    }
    private void SpawnProjectile(Vector2 direction)
    {
        Arrow arrow = PoolManager.Instance.GetPool(playerWeapon.arrow);
        // 투사체의 현재 위치를 몬스터의 위치로 설정
        arrow.transform.position = transform.position;
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * arrow.arrowSpeed;
        }
    }
}
