using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]

public class PlayerController : MonoBehaviour
{
    private InputAction moveia;
    private Rigidbody2D rb;

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

    private void Awake()
    {
        moveia = InputSystem.actions.FindAction("Move");
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        PlayerMove();
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
}
