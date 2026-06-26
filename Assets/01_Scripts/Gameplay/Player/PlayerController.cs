using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]

public class PlayerController : MonoBehaviour, IDamageable
{
    private InputAction moveia;
    private InputAction jumpia;
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
    private float NowHp { get; set; } = 100f;
    private bool Invincible { get; set; } = false;

    private PlayerWeaponSO.WeaponType reWeaponType;
    private Coroutine coHpRegen;
    private Dictionary<PlayerWeaponSO.WeaponType, GameObject> saveArm = new Dictionary<PlayerWeaponSO.WeaponType, GameObject>();

    public event Action<float, float> OnHpChanged;
    [SerializeField] private VoidEventChannel onPlayerDead;
    private LevelUp weaponUnlock;
    private Animator Ani;
    SpriteRenderer sp;

    Vector2 move;

    private void Awake()
    {
        moveia = InputSystem.actions.FindAction("Move");
        jumpia = InputSystem.actions.FindAction("Jump");
        rb = GetComponent<Rigidbody2D>();
        weaponUnlock = FindAnyObjectByType<LevelUp>();
        Ani = GetComponent<Animator>();
    }
    private void Start()
    {
        coHpRegen = StartCoroutine(HpRegen());
        sp = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        move = moveia.ReadValue<Vector2>().normalized;
        if (jumpia.WasPressedThisFrame())
        {
            weaponUnlock.transform.gameObject.SetActive(true);
        }
    }

    private void FixedUpdate()
    {
        PlayerMove();
    }

    public void PlayerMove()
    {
        rb.linearVelocity = move * ((moveSpeed + 100f) / 100f) * baseSpeed;
        
        if (move.x > 0.5f)
        {
            sp.flipX = true;

        }
        else if(move.x < -0.5f)
        {
            sp.flipX = false;

        }
        if (Mathf.Abs(move.x) > 0.5f)
        {
            Ani.SetBool("Move", true);
        }
        else if (Mathf.Abs(move.y) > 0.5f)
        {
            Ani.SetBool("Move", true);
        }
        else
        {
            Ani.SetBool("Move", false);
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
    public void TakeDamage(float damage, bool isCrit = false)
    {
        if (Invincible == true) return;
        co = StartCoroutine(OnEnemyAttack(damage));
    }
    IEnumerator OnEnemyAttack(float damage)
    {
        Invincible = true;
        NowHp -= damage * (100 / 100 + armor);
        OnHpChanged?.Invoke(NowHp, maxHp);
        if (NowHp < 0)
        {
            NowHp = 0;
            onPlayerDead.RaiseEvent();
        }

        HitText hitText = PoolManager.Instance.GetPool<HitText>();
        hitText.ShowDamage(damage, transform.position, false, true);

        yield return new WaitForSecondsRealtime(invincibleTime);
        Invincible = false;
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
        return this.NowHp;
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
            OnHpChanged?.Invoke(NowHp, maxHp);
        }
    }

    public void HpAbs()
    {
        int i = UnityEngine.Random.Range(1, 101);
        if(hpAbs >= i)
        {
            NowHp += 1;
            OnHpChanged?.Invoke(NowHp, maxHp);
        }
    }
    
    public void OnWeaponArm(PlayerWeaponSO.WeaponType weaponType)
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
                    PlayerAttack paChild2 = child2.GetComponent<PlayerAttack>();
                    child2.transform.position = transform.position + new Vector3(x2, y2, 0) * radius;
                    break;
                }
                float angle = Mathf.PI * 1.83f + i * (Mathf.PI * 2f) / childNum;
                GameObject child = transform.GetChild(i+2).gameObject;
                float x = Mathf.Cos(angle);
                float y = Mathf.Sin(angle);
                PlayerAttack paChild = child.GetComponent<PlayerAttack>();
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
                PlayerAttack paChild = child.GetComponent<PlayerAttack>();
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
                PlayerAttack paChild = child.GetComponent<PlayerAttack>();
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
                PlayerAttack paChild = child.GetComponent<PlayerAttack>();
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
                PlayerAttack paChild = child.GetComponent<PlayerAttack>();
                child.transform.position = transform.position + new Vector3(x, y, 0) * radius;
            }
        }
    }
    public void SetWeaponArm()
    {

        float radius = 1f;
        int childNum = transform.childCount - 2;
        for(int i = 0; i < childNum; i++)
        {
            GameObject childS = transform.GetChild(i+2).gameObject;
            childS.transform.DOComplete();
        }

        if (childNum == 2)
        {
            childNum++;
            for (int i = 0; i < childNum; i++)
            {
                if (i == 1)
                {
                    i++;
                    float angle2 = Mathf.PI * 1.83f + i * (Mathf.PI * 2f) / childNum;
                    GameObject child2 = transform.GetChild(i + 1).gameObject;
                    float x2 = Mathf.Cos(angle2);
                    float y2 = Mathf.Sin(angle2);
                    PlayerAttack paChild2 = child2.GetComponent<PlayerAttack>();
                    child2.transform.position = transform.position + new Vector3(x2, y2, 0) * radius;
                    break;
                }
                float angle = Mathf.PI * 1.83f + i * (Mathf.PI * 2f) / childNum;
                GameObject child = transform.GetChild(i + 2).gameObject;
                float x = Mathf.Cos(angle);
                float y = Mathf.Sin(angle);
                PlayerAttack paChild = child.GetComponent<PlayerAttack>();
                child.transform.position = transform.position + new Vector3(x, y, 0) * radius;
            }
            childNum--;
        }
        else if (childNum == 4)
        {
            for (int i = 0; i < childNum; i++)
            {
                float angle = Mathf.PI * 1.75f + i * (Mathf.PI * 2f) / childNum;
                GameObject child = transform.GetChild(i + 2).gameObject;
                float x = Mathf.Cos(angle);
                float y = Mathf.Sin(angle);
                PlayerAttack paChild = child.GetComponent<PlayerAttack>();
                child.transform.position = transform.position + new Vector3(x, y, 0) * radius;
            }
        }
        else if (childNum == 5)
        {
            for (int i = 0; i < childNum; i++)
            {
                float angle = Mathf.PI * 1.7f + i * (Mathf.PI * 2f) / childNum;
                GameObject child = transform.GetChild(i + 2).gameObject;
                float x = Mathf.Cos(angle);
                float y = Mathf.Sin(angle);
                PlayerAttack paChild = child.GetComponent<PlayerAttack>();
                child.transform.position = transform.position + new Vector3(x, y, 0) * radius;
            }
        }
        else if (childNum == 6)
        {
            for (int i = 0; i < childNum; i++)
            {
                float angle = Mathf.PI * 1.67f + i * (Mathf.PI * 2f) / childNum;
                GameObject child = transform.GetChild(i + 2).gameObject;
                float x = Mathf.Cos(angle);
                float y = Mathf.Sin(angle);
                PlayerAttack paChild = child.GetComponent<PlayerAttack>();
                child.transform.position = transform.position + new Vector3(x, y, 0) * radius;
            }
        }
        else
        {
            for (int i = 0; i < childNum; i++)
            {
                float angle = Mathf.PI * 1.83f + i * (Mathf.PI * 2f) / childNum;
                GameObject child = transform.GetChild(i + 2).gameObject;
                float x = Mathf.Cos(angle);
                float y = Mathf.Sin(angle);
                PlayerAttack paChild = child.GetComponent<PlayerAttack>();
                child.transform.position = transform.position + new Vector3(x, y, 0) * radius;
            }
        }
    }
}
