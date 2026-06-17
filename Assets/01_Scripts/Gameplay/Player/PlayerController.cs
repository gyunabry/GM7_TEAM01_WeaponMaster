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
    [SerializeField] GameObject arm;
    [SerializeField] PlayerWeaponManager weaponManager;

    [Header("Tag")]
    [SerializeField] private string enemyTagName;
    [SerializeField] private string enemyAttackTagName;

    [Header("Player Stat")]
    [SerializeField] private float maxHp = 100;
    [SerializeField] private float hpRegen = 0;
    [SerializeField] private float hpAbs = 0;
    [SerializeField] private float damage = 100;
    [SerializeField] private float armorPiercing = 0;
    [SerializeField] private float attackSpeed = 100;
    [SerializeField] private float cri = 0;
    [SerializeField] private float range = 0;
    [SerializeField] private float armor = 0;
    [SerializeField] private float evasion = 0;
    [SerializeField] private float moveSpeed = 100;

    [Header("Player default")]
    [SerializeField] private float baseSpeed = 500;

    [Header("Player Weapon")]
    [SerializeField] private List<PlayerWeaponSO> playerWeapon;
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
            playerWeapon.Add(weaponManager.GetWeapon("Sword"));
            Instantiate(arm, transform.position, Quaternion.identity, transform);
        }
    }
    public void PlayerMove()
    {
        Vector2 move = moveia.ReadValue<Vector2>().normalized;
        rb.linearVelocity = move * (moveSpeed / 100) * baseSpeed * Time.deltaTime;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(enemyTagName))
        {

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
    public void OnWeaponArm()
    {
        playerWeapon.Add(weaponManager.GetWeapon("Sword"));
        Instantiate(arm, transform.position, Quaternion.identity, transform);

        float radius = 1.0f;
        int childNum = transform.childCount;
        for(int i = 0; i < childNum; i++)
        {
            float angle = i * (Mathf.PI * 2f) / childNum;
            GameObject child = transform.GetChild(i).gameObject;
            child.transform.position = transform.position + (new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0)) * radius;
        }
    }
}
