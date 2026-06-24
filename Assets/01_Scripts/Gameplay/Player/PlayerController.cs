using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Hierarchy;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.InputSystem;

    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]

public class PlayerController : MonoBehaviour, IDamageable
{
    private InputAction moveia;
    private Rigidbody2D rb;
    private Coroutine co;

    [Header("prefabs")]
    [SerializeField] GameObject arm;

    [Header("WeaponManager")]
    [SerializeField] PlayerWeaponManager weaponManager;

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
    [SerializeField] private float nowExp = 0;
    [SerializeField] private float reqExp = 50;
    [SerializeField] private float gold = 0;

    [Header("Player default")]
    [SerializeField] private float baseSpeed = 5f;

    [Header("Player Weapon")]
    private Dictionary<PlayerWeaponSO.WeaponType, PlayerWeaponSO> playerWeapon = new Dictionary<PlayerWeaponSO.WeaponType, PlayerWeaponSO>();
    private float nowHp { get; set; } = 100f;
    private bool invincible { get; set; } = false;

    private PlayerWeaponSO.WeaponType reWeaponType;
    private Coroutine coHpRegen;
    private Dictionary<PlayerWeaponSO.WeaponType, GameObject> saveArm = new Dictionary<PlayerWeaponSO.WeaponType, GameObject>();

    public event Action<float, float> OnHpChanged;
    [SerializeField] private VoidEventChannel onPlayerDead;

    Vector2 move;

    private void Awake()
    {
        moveia = InputSystem.actions.FindAction("Move");
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        coHpRegen = StartCoroutine(HpRegen());
    }

    void Update()
    {
        move = moveia.ReadValue<Vector2>().normalized;
    }

    private void FixedUpdate()
    {
        PlayerMove();
    }

    public void PlayerMove()
    {
        rb.linearVelocity = move * ((moveSpeed + 100f) / 100f) * baseSpeed;
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
    public Dictionary<PlayerWeaponSO.WeaponType, PlayerWeaponSO> GetWeaponList()
    {
        return playerWeapon;
    }
    public PlayerWeaponSO GetWeapon()
    {
        PlayerWeaponSO pws;
        if(playerWeapon.TryGetValue(reWeaponType, out pws))
        {
            return pws;
        }
        else
        {
            return null;
        }
    }
    public PlayerWeaponSO GetWeaponStat(PlayerWeaponSO.WeaponType type)
    {
        PlayerWeaponSO pws;
        if(playerWeapon.TryGetValue(type, out pws)){
            return pws;
        }
        else
        {
            return null;
        }
    }
    public PlayerWeaponSO.WeaponType OnWeaponTypeName(PlayerWeaponSO.WeaponType typeName)
    {
        return typeName;
    }
    public void TakeDamage(float damage)
    {
        if (invincible == true) return;
        co = StartCoroutine(OnEnemyAttack(damage));
    }
    IEnumerator OnEnemyAttack(float damage)
    {
        invincible = true;
        nowHp -= damage * (100 / 100 + armor);
        OnHpChanged?.Invoke(nowHp, maxHp);
        if (nowHp < 0)
        {
            nowHp = 0;
            onPlayerDead.RaiseEvent();
        }

        HitText hitText = PoolManager.Instance.GetPool<HitText>();
        hitText.ShowDamage(damage, transform.position, false, true);

        yield return new WaitForSecondsRealtime(invincibleTime);
        invincible = false;
        co = null;
    }
    //경험치와 골드 수치 추가하는 메서드
    public void SetExp(float value)
    {
        nowExp += value;
    }
    public void SetGold(float value)
    {
        gold += value;
    }
    //플레이어 스탯 수치 가져오기
    public float GetMaxHp()
    {
        return this.maxHp;
    }
    public float GetNowHp()
    {
        return this.nowHp;
    }
    public float GetNowExp()
    {
        return this.nowExp;
    }
    public float GetReqExp()
    {
        return this.reqExp;
    }
    public float GetNowGold()
    {
        return this.gold;
    }
    public Dictionary<PlayerWeaponSO.WeaponType, GameObject> GetArm()
    {
        return saveArm;
    }
    //끝
    IEnumerator HpRegen()
    {
        while (true)
        {
            if(hpRegen != 0)
            {
                yield return new WaitForSeconds(300 / (hpRegen + 100));
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
            OnHpChanged?.Invoke(nowHp, maxHp);
        }
    }

    public void HpAbs()
    {
        int i = UnityEngine.Random.Range(1, 101);
        if(hpAbs >= i)
        {
            nowHp += 1;
            OnHpChanged?.Invoke(nowHp, maxHp);
        }
    }
    
    public void OnWeaponArm(PlayerWeaponSO.WeaponType weaponType) //이곳 무기 획득 UI완성시 최우선으로 바꿀것
    {
        reWeaponType = OnWeaponTypeName(weaponType);

        PlayerWeaponSO pws;
        if(playerWeapon.TryGetValue(reWeaponType, out pws))
        {
            return;
        }
        if (transform.childCount > 7) return;
        PlayerWeaponSO.WeaponType imWeaponType = weaponManager.GetWeaponType(reWeaponType);
        PlayerWeaponSO imWeapon = weaponManager.GetWeapon(reWeaponType);
        playerWeapon.TryAdd(imWeaponType, imWeapon);

        saveArm.Add(imWeaponType, Instantiate(arm, transform.position, Quaternion.identity, transform));
        float radius = 1f;
        int childNum = transform.childCount - 2;
        if (childNum == 2) 
        {
            childNum++;
            for (int i = 0; i < childNum; i++)
            {
                if (i == 1) {
                    i++;
                    float angle2 = Mathf.PI * 1.83f + i * (Mathf.PI * 2f) / childNum;
                    GameObject child2 = transform.GetChild(i+1).gameObject;
                    float x2 = Mathf.Cos(angle2);
                    float y2 = Mathf.Sin(angle2);
                    child2.transform.position = transform.position + new Vector3(x2, y2, 0) * radius;
                    break;
                }
                float angle = Mathf.PI * 1.83f + i * (Mathf.PI * 2f) / childNum;
                GameObject child = transform.GetChild(i+2).gameObject;
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
                GameObject child = transform.GetChild(i+2).gameObject;
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
                GameObject child = transform.GetChild(i+2).gameObject;
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
                GameObject child = transform.GetChild(i+2).gameObject;
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
                GameObject child = transform.GetChild(i+2).gameObject;
                float x = Mathf.Cos(angle);
                float y = Mathf.Sin(angle);
                child.transform.position = transform.position + new Vector3(x, y, 0) * radius;
            }
        }
            
    }
}
