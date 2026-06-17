using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Hierarchy;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.InputSystem;

    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]

public class PlayerController : MonoBehaviour
{
    private InputAction moveia;
    private InputAction jumpia;
    private Rigidbody2D rb;
    private Coroutine co;

    [Header("prefabs")]
    [SerializeField] GameObject arm;

    [Header("WeaponManager")]
    [SerializeField] PlayerWeaponManager weaponManager;

    [Header("Tag")]
    [SerializeField] private string enemyTagName;
    [SerializeField] private string enemyAttackTagName;

    [Header("Player Stat")]
    [SerializeField] private float maxHp = 100f;
    [SerializeField] private float hpRegen = 0f;
    [SerializeField] private float hpAbs = 0f;
    [SerializeField] private float damage = 100f;
    [SerializeField] private float armorPiercing = 0f;
    [SerializeField] private float attackSpeed = 100f;
    [SerializeField] private float cri = 0f;
    [SerializeField] private float range = 0f;
    [SerializeField] private float armor = 0f;
    [SerializeField] private float evasion = 0f;
    [SerializeField] private float moveSpeed = 100f;
    [SerializeField] private float invincibleTime = 1.0f;

    [Header("Player default")]
    [SerializeField] private float baseSpeed = 500f;

    [Header("Player Weapon")]
    [SerializeField] private List<PlayerWeaponSO> playerWeapon;
    private float nowHp = 100f;
    private bool invincible = false;

    private void Awake()
    {
        moveia = InputSystem.actions.FindAction("Move");
        jumpia = InputSystem.actions.FindAction("Jump");
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        PlayerMove();
        if (jumpia.WasPressedThisFrame())
        {
            OnWeaponArm();
        }
    }
    public void PlayerMove()
    {
        Vector2 move = moveia.ReadValue<Vector2>().normalized;
        rb.linearVelocity = move * (moveSpeed / 100) * baseSpeed * Time.deltaTime;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (invincible == true) return;
        if (collision.collider.CompareTag(enemyTagName))
        {
            co = StartCoroutine(OnEnemyAttack(collision));
        }
        else if (collision.collider.CompareTag(enemyAttackTagName))
        {
            
        }
    }

    public Dictionary<string, float> PlayerStat()
    {
        Dictionary<string, float> playerStat = new Dictionary<string, float>();
        playerStat.Add("maxHp", maxHp);
        playerStat.Add("hpRegen", hpRegen);
        playerStat.Add("hpAbs", hpAbs);
        playerStat.Add("damage", damage);
        playerStat.Add("armorPiercing", armorPiercing);
        playerStat.Add("attackSpeed", attackSpeed);
        playerStat.Add("cri", cri);
        playerStat.Add("range", range);
        playerStat.Add("armor", armor);
        playerStat.Add("evasion", evasion);
        playerStat.Add("moveSpeed", moveSpeed);

        return playerStat;
         
    }
    public PlayerWeaponSO GetWeapon()
    {
        if (playerWeapon == null) return null;
        int a = playerWeapon.Count;
        return playerWeapon[a - 1];
    }
    IEnumerator OnEnemyAttack(Collision2D collision)
    {
        invincible = true;
        EnemyAttackData enemyAttackData = collision.collider.GetComponent<EnemyAttackData>();
        nowHp = enemyAttackData.attackDamage;
        yield return new WaitForSecondsRealtime(invincibleTime);
        invincible = false;
        co = null;
    }
    public void OnWeaponArm()
    {
        playerWeapon.Add(weaponManager.GetWeapon("Sword"));
        Instantiate(arm, transform.position, Quaternion.identity, transform);

        float radius = 1f;
        int childNum = transform.childCount;
        if (childNum == 2) 
        {
            childNum++;
            for (int i = 0; i < childNum; i++)
            {
                if (i == 1) {
                    i++;
                    float angle2 = Mathf.PI * 1.83f + i * (Mathf.PI * 2f) / childNum;
                    GameObject child2 = transform.GetChild(i-1).gameObject;
                    float x2 = Mathf.Cos(angle2);
                    float y2 = Mathf.Sin(angle2);
                    child2.transform.position = transform.position + new Vector3(x2, y2, 0) * radius;
                    break;
                }
                float angle = Mathf.PI * 1.83f + i * (Mathf.PI * 2f) / childNum;
                GameObject child = transform.GetChild(i).gameObject;
                float x = Mathf.Cos(angle);
                float y = Mathf.Sin(angle);
                child.transform.position = transform.position + new Vector3(x, y, 0) * radius;
            }
            childNum--;
        }
        else if(childNum == 4)
        {
            for (int i = 0; i < childNum; i++)
            {
                float angle = Mathf.PI * 1.75f + i * (Mathf.PI * 2f) / childNum;
                GameObject child = transform.GetChild(i).gameObject;
                float x = Mathf.Cos(angle);
                float y = Mathf.Sin(angle);
                child.transform.position = transform.position + new Vector3(x, y, 0) * radius;
            }
        }
        else if (childNum == 5)
        {
            for (int i = 0; i < childNum; i++)
            {
                float angle = Mathf.PI * 1.7f + i * (Mathf.PI * 2f) / childNum;
                GameObject child = transform.GetChild(i).gameObject;
                float x = Mathf.Cos(angle);
                float y = Mathf.Sin(angle);
                child.transform.position = transform.position + new Vector3(x, y, 0) * radius;
            }
        }
        else if (childNum == 6)
        {
            for (int i = 0; i < childNum; i++)
            {
                float angle = Mathf.PI * 1.67f + i * (Mathf.PI * 2f) / childNum;
                GameObject child = transform.GetChild(i).gameObject;
                float x = Mathf.Cos(angle);
                float y = Mathf.Sin(angle);
                child.transform.position = transform.position + new Vector3(x, y, 0) * radius;
            }
        }
        else
        {
            for (int i = 0; i < childNum; i++)
            {
                float angle = Mathf.PI * 1.83f + i * (Mathf.PI * 2f) / childNum;
                GameObject child = transform.GetChild(i).gameObject;
                float x = Mathf.Cos(angle);
                float y = Mathf.Sin(angle);
                child.transform.position = transform.position + new Vector3(x, y, 0) * radius;
            }
        }
            
    }
}
