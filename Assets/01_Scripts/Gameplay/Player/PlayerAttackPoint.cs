using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAttackPoint : MonoBehaviour
{
    
    private UnityEvent ue;
    private PlayerAttack playerAttack;
    private PlayerController playerController;
    private PlayerWeaponSO.WeaponType weaponType;
    private PlayerWeaponSO weaponStat;

    private float nowDamage;
    private float nowArmorPiercing;
    private float nowAttackSpeed;
    private float nowRange;
    private float nowCri;
    private float nowSize;
    private Sprite nowSprite;

    private List<GameObject> enemyGO = new List<GameObject>();
    private BoxCollider2D box;
    private Arrow arrow;
    private bool criHit = false;
    private bool bulletDestroy = false;
    

    private float damage;
    private void Awake()
    {
        ue = new UnityEvent();
        box = GetComponent<BoxCollider2D>();
        arrow = GetComponent<Arrow>();
        
    }
    private void Start()
    {
        playerAttack = GetComponentInParent<PlayerAttack>();
        playerController = FindAnyObjectByType<PlayerController>();
        SetWeaponType();
    }
    private void Update()
    {
        if(box.enabled == false && enemyGO.Count > 0)
        {
            enemyGO.Clear();
        }
    }
    public void SetWeaponType()
    {
        if (transform.parent != null)
        {
            weaponType = playerAttack.GetParentType();
        }
        weaponStat = playerController.GetWeaponStat(weaponType);
        nowDamage = playerAttack.GetUpgradeDamage();
        nowArmorPiercing = playerAttack.GetUpgradeArmorPiercing();
        nowAttackSpeed = playerAttack.GetUpgradeAttackSpeed();
        nowRange = playerAttack.GetUpgradeRange();
        nowCri = playerAttack.GetUpgradeCri();
        nowSize = playerAttack.GetUpgradeSize();
    }
    public void SetCri()
    {
        nowCri = playerAttack.GetUpgradeCri();
    }
    
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (enemyGO.Contains(collision.gameObject) && arrow == null)
            {
                return;
            }
            else
            {
                enemyGO.Add(collision.gameObject);
                int critical = Random.Range(1, 101);
                SetCri();
                if(nowCri >= critical)
                {
                    criHit = true;
                }
                if(criHit == true)
                {
                    nowDamage = playerAttack.GetDamage() * 2;
                }
                else
                {
                    nowDamage = playerAttack.GetDamage();
                }
             // Debug.Log(nowDamage);
                EnemyController enemy;
                collision.TryGetComponent<EnemyController>(out enemy);
                if (enemy != null)
                {
                    if (criHit == true)
                    {
                        enemy.TakeDamage(nowDamage, true);
                        playerController.HpAbs();
                    }
                    else
                    {
                        enemy.TakeDamage(nowDamage);
                        playerController.HpAbs();
                    }
                }
                criHit = false;
            }
        }
        if (collision.CompareTag("Boss"))
        {
            if (enemyGO.Contains(collision.gameObject) && arrow == null)
            {
                return;
            }
            else
            {
                enemyGO.Add(collision.gameObject);
                int critical = Random.Range(1, 101);
                SetCri();
                if (nowCri >= critical)
                {
                    criHit = true;
                }
                if (criHit == true)
                {
                    nowDamage = playerAttack.GetDamage() * 2;
                }
                else
                {
                    nowDamage = playerAttack.GetDamage();
                }
                // Debug.Log(nowDamage);
                BossController enemy;
                collision.TryGetComponent<BossController>(out enemy);
                if (enemy != null)
                {
                    if(criHit == true)
                    {
                        enemy.TakeDamage(nowDamage, true);
                        playerController.HpAbs();
                    }
                    else
                    {
                        enemy.TakeDamage(nowDamage);
                        playerController.HpAbs();
                    }
                }
                criHit = false;
            }
        }
    }
}