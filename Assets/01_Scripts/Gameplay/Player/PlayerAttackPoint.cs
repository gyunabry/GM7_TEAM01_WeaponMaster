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
    }
    //public void SetWeaponStat()
    //{
    //    weaponStat = playerController.GetWeaponStat(weaponType);
    //}
    
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            damage = playerAttack.GetDamage();
            EnemyController enemy;
            collision.TryGetComponent<EnemyController>(out enemy);
            if(enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}