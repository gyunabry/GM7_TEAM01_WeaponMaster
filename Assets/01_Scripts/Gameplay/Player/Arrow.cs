using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float arrowSpeed;
    private ObjectPool<Arrow> pool;
    private Coroutine co;
    private PlayerWeaponSO playerWeapon;
    private PlayerController playerController;

    private void Awake()
    {
        playerController = FindAnyObjectByType<PlayerController>();
        playerWeapon = playerController.GetWeapon();
    }
    
    public void SetPool(ObjectPool<Arrow> pool)
    {
        this.pool = pool;
    }

    void Update()
    {
        Vector2 dir = new Vector2(1f, 1f);
        transform.Translate(dir * arrowSpeed * Time.deltaTime);
        co = StartCoroutine(ReleaseTime());
    }
    IEnumerator ReleaseTime()
    {
        yield return new WaitForSecondsRealtime(5.0f);
        pool.Release(this);
        co = null;
    }
   
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            IDamageable go = collision.gameObject.GetComponent<IDamageable>(); //IDamageable 로 안되면 바꾸기
            if (go != null)
            {
                go.TakeDamage(playerWeapon.weaponDamage);
                pool.Release(this);
            }
        }
    }
}
