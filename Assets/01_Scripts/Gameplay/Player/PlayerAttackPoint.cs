using UnityEngine;

public class PlayerAttackPoint : MonoBehaviour
{
    private PlayerWeaponSO playerWeapon;
    private PlayerController playerController;
    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        playerWeapon = playerController.GetWeapon();
    }

    
    public void OnTriggerEnter2D(Collider2D collision)
    {
<<<<<<< HEAD
        if (playerWeapon.weaponType.ToString() == "Bow") return;    
=======
>>>>>>> parent of c78d4e96 (feat: bow & stat)
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("적 타격");
            IDamageable go = collision.gameObject.GetComponent<IDamageable>(); //IDamageable 로 안되면 바꾸기
            if (go != null)
            {
                go.TakeDamage(playerWeapon.weaponDamage);
            }
        }
    }
}
