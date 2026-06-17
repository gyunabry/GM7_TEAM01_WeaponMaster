using UnityEngine;

public class PlayerAttackPoint : MonoBehaviour
{
    private PlayerWeaponSO playerWeapon;
    private void Awake()
    {
        playerWeapon = GetComponentInParent<PlayerWeaponSO>();
    }

    
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            IDamageable go = collision.gameObject.GetComponent<IDamageable>();
            if (go != null)
            {
                go.TakeDamage(playerWeapon.weaponDamage);
            }
        }
    }
}
