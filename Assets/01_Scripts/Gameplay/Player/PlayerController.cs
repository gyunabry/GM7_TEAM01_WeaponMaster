using System.Collections;
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
    [SerializeField] private float baseSpeed = 500f;

    [Header("Player Weapon")]
    private Dictionary<PlayerWeaponSO.WeaponType, PlayerWeaponSO> playerWeapon = new Dictionary<PlayerWeaponSO.WeaponType, PlayerWeaponSO>();
    private float nowHp { get; set; } = 100f;
    private bool invincible { get; set; } = false;
    private PlayerWeaponSO.WeaponType reWeaponType;
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
    //끝
    IEnumerator OnEnemyAttack(float damage)
    {
        invincible = true;
        nowHp -= damage;
        if (nowHp < 0) 
        {
            nowHp = 0;
        }
        //죽었을때 사용할 명령어
        Debug.Log("죽었다!"); //임시 명령어
        yield return new WaitForSecondsRealtime(invincibleTime);
        invincible = false;
        co = null;
    }
    
    public void OnWeaponArm() //이곳 무기 획득 UI완성시 최우선으로 바꿀것
    {
        if(playerWeapon.Count == 0)
        {
            reWeaponType = OnWeaponTypeName(PlayerWeaponSO.WeaponType.Bow);
        }
        else if (playerWeapon.Count == 1)
        {
            reWeaponType = OnWeaponTypeName(PlayerWeaponSO.WeaponType.Sword);
        }
        else if(playerWeapon.Count == 2)
        {
            reWeaponType = OnWeaponTypeName(PlayerWeaponSO.WeaponType.Axe);
        }


        else
        {
            return;
        }
        PlayerWeaponSO pws;
        if(playerWeapon.TryGetValue(reWeaponType, out pws))
        {
            return;
        }
        PlayerWeaponSO.WeaponType imWeaponType = weaponManager.GetWeaponType(reWeaponType);
        PlayerWeaponSO imWeapon = weaponManager.GetWeapon(reWeaponType);
        playerWeapon.Add(imWeaponType, imWeapon);
        
        Instantiate(arm, transform.position, Quaternion.identity, transform);
        float radius = 1f;
        int childNum = transform.childCount - 1;
        if (childNum == 2) 
        {
            childNum++;
            for (int i = 0; i < childNum; i++)
            {
                if (i == 1) {
                    i++;
                    float angle2 = Mathf.PI * 1.83f + i * (Mathf.PI * 2f) / childNum;
                    GameObject child2 = transform.GetChild(i).gameObject;
                    float x2 = Mathf.Cos(angle2);
                    float y2 = Mathf.Sin(angle2);
                    child2.transform.position = transform.position + new Vector3(x2, y2, 0) * radius;
                    break;
                }
                float angle = Mathf.PI * 1.83f + i * (Mathf.PI * 2f) / childNum;
                GameObject child = transform.GetChild(i+1).gameObject;
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
                GameObject child = transform.GetChild(i+1).gameObject;
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
                GameObject child = transform.GetChild(i+1).gameObject;
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
                GameObject child = transform.GetChild(i+1).gameObject;
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
                GameObject child = transform.GetChild(i+1).gameObject;
                float x = Mathf.Cos(angle);
                float y = Mathf.Sin(angle);
                child.transform.position = transform.position + new Vector3(x, y, 0) * radius;
            }
        }
            
    }
}
