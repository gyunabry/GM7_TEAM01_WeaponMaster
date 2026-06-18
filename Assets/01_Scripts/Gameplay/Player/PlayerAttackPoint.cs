using UnityEngine;

public class PlayerAttackPoint : MonoBehaviour
{
    private PlayerWeaponSO playerWeapon;
    private PlayerController playerController;
    
    private void Awake()
    {
        playerController = FindAnyObjectByType<PlayerController>();
        playerWeapon = playerController.GetWeapon();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerWeapon.weaponType.ToString() == "Bow") return;
        if (collision.CompareTag("Enemy"))
        {
            IDamageable go = collision.gameObject.GetComponent<IDamageable>(); //IDamageable 로 안되면 바꾸기
            if (go != null)
            {
                go.TakeDamage(playerWeapon.weaponDamage);
            }
        }
    }
}
