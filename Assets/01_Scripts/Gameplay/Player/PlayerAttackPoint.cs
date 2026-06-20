using System.Collections;
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

    private float damage;
    private void Awake()
    {
        ue = new UnityEvent();
    }
    private void Start()
    {
        playerAttack = GetComponentInParent<PlayerAttack>();
        playerController = FindAnyObjectByType<PlayerController>();
        SetWeaponType();
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
    
    
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            nowDamage = playerAttack.GetDamage();
            Debug.Log(nowDamage);
            EnemyController enemy;
            collision.TryGetComponent<EnemyController>(out enemy);
            if(enemy != null)
            {
                enemy.TakeDamage(nowDamage);
            }
        }
    }
}