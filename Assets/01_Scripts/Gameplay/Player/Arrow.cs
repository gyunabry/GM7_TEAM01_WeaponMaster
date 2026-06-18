using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Arrow : MonoBehaviour
{
    public float arrowSpeed;
    private PlayerAttack playerAttack;
    private PlayerWeaponSO playerWeapon;
<<<<<<< HEAD
<<<<<<< HEAD
    private PlayerController playerController;


<<<<<<< Updated upstream
    void Update()
    {
        Vector2 dir = new Vector2(1f, 1f);
        transform.Translate(dir * arrowSpeed * Time.deltaTime);
=======
=======
>>>>>>> parent of c78d4e96 (feat: bow & stat)
    private void Awake()
    {
        playerWeapon = GetComponentInParent<PlayerWeaponSO>();
    }
    public void SetPool(ObjectPool<Arrow> pool)
    {
        this.pool = pool;
    }

    void Update()
    {
        transform.Translate(Vector2.up * arrowSpeed * Time.deltaTime);
        co = StartCoroutine(ReleaseTime());
>>>>>>> parent of c78d4e96 (feat: bow & stat)
    }
=======
    
>>>>>>> Stashed changes
    private void OnEnable()
    {
        playerController = FindAnyObjectByType<PlayerController>();
        playerAttack = FindAnyObjectByType<PlayerAttack>();
        playerWeapon = playerController.GetWeapon();
        Vector2 direction = playerAttack.colliderA.transform.position - transform.position;
        float rotz = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.position = transform.position;
        transform.Rotate(0f, 0f, rotz + 45f);
        // 활성화 4초 이후 풀로 반환
        Invoke(nameof(ReturnToPool), 4f);
    }
<<<<<<< HEAD
    private void ReturnToPool()
    {
        CancelInvoke(nameof(ReturnToPool));
        PoolManager.Instance.ReturnPool(this);
    }
<<<<<<< HEAD
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            IDamageable go = collision.gameObject.GetComponent<IDamageable>(); //IDamageable 로 안되면 바꾸기
            if (go != null)
            {
                go.TakeDamage(playerWeapon.weaponDamage);
            }
        }
    }
=======
>>>>>>> parent of c78d4e96 (feat: bow & stat)
=======
>>>>>>> parent of c78d4e96 (feat: bow & stat)
}
