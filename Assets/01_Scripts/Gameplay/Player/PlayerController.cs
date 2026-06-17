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
    [SerializeField] private float maxHp;
    [SerializeField] private float hpRegen;
    [SerializeField] private float hpAbs;
    [SerializeField] private float damage;
    [SerializeField] private float armorPiercing;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float cri;
    [SerializeField] private float range;
    [SerializeField] private float armor;
    [SerializeField] private float evasion;
    [SerializeField] private float moveSpeed;

    [Header("Player default")]
    [SerializeField] private float baseSpeed;

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
}
