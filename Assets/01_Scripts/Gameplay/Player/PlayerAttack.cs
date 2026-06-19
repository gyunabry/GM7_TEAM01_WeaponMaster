using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

public class PlayerAttack : MonoBehaviour
{
    private PlayerWeaponSO playerWeapon;
    private PlayerController playerController;
    private ArrowPooling arrowPooling;
    [SerializeField] private LayerMask enemyLayer;
    private GameObject weaponSprite;
    private BoxCollider2D childBox;
    private Transform parentTrans;
    private Coroutine co;
    private Coroutine attackco;
    private PlayerWeaponSO.WeaponType weaponType;
    private Dictionary<string, float> playerStat;

    private AssetBundle assetBundle;
    private GameObject instant;

    private bool isCo = false;
    private bool isAttackCo = false;
    private string weaponName;
    private Collider2D[] enemyTamgi = new Collider2D[50];
    private Collider2D enemyTrans;
    private float nowDamage;
    private float nowArmorPiercing;
    private float nowAttackSpeed;
    private float nowRange;
    private float nowCri;
    private int ijk = 0;


    private void Awake() //무기 생성 부분 UI완성시 바꿀것
    {
        arrowPooling = FindFirstObjectByType<ArrowPooling>();
        playerController = GetComponentInParent<PlayerController>();
        GetPlayerStat();
    }
    public float GetDamage()
    {
        return nowDamage;
    }
    public void OnPrefabLoaded(AsyncOperationHandle<GameObject> handle)
    {
        if(handle.Status == AsyncOperationStatus.Succeeded)
        {
            instant = handle.Result;
            instant.transform.Rotate(0f, 0f, -45f);
            SpriteRenderer sr = instant.GetComponent<SpriteRenderer>();
            sr.sprite = playerWeapon.weaponIcon;
            instant.transform.SetParent(this.transform);
            Vector3 srPosition = transform.position;
            srPosition.x += 0.3f;
            instant.transform.position = srPosition;
            childBox = GetComponentInChildren<BoxCollider2D>();
        }
        else
        {
            Debug.Log("무기 프리팹이 없거나 이름이 다름");
        }
    }
    private void Start()
    {
        playerWeapon = playerController.GetWeapon();
        weaponType = playerWeapon.weaponType;
        weaponName = weaponType.ToString();
        Addressables.InstantiateAsync(weaponType.ToString()).Completed += OnPrefabLoaded;
    }
    // GameObject go = Instantiate(weaponSprite, srPosition, Quaternion.Euler(0f, 0f, -45f), transform);
    
    void Update()
    {
        if (isCo == false)
        {
            isCo = true;
            co = StartCoroutine(Weapon());
        }
    }
    public PlayerWeaponSO.WeaponType GetParentType()
    {
        return weaponType;
    }
    public void GetPlayerStat()
    {
        playerStat = playerController.PlayerStat();
    }
    public void GetUpgrade(int j)
    {
        ijk = j;
        SetWeaponStat(ijk);
    }
    public void SetWeaponStat(int i)
    {
        if (playerWeapon.upgrades.Count == 0)
        {
            nowDamage = playerWeapon.weaponDamage;
            nowArmorPiercing = playerWeapon.weaponArmorPiercing;
            nowAttackSpeed = playerWeapon.weaponAttackSpeed;
            nowRange = playerWeapon.weaponRange;
            nowCri = playerWeapon.weaponCri;
        }
        else
        {
            nowDamage = playerWeapon.weaponDamage + playerWeapon.GetUpgradeDamage(i);
            nowArmorPiercing = playerWeapon.weaponArmorPiercing + playerWeapon.GetUpgradeArmorPiercing(i);
            nowAttackSpeed = playerWeapon.weaponAttackSpeed + playerWeapon.GetUpgradeAttackSpeed(i);
            nowRange = playerWeapon.weaponRange + playerWeapon.GetUpgradeRange(i);
            nowCri = playerWeapon.weaponCri + playerWeapon.GetUpgradeCri(i);
        }
    }
    IEnumerator Weapon()
    {
        GetUpgrade(0);
        yield return new WaitForSecondsRealtime(1.0f);

        while (true)
        {
            while (true)
            {
                enemyTrans = FindEnemy();
                if (enemyTrans != null) {
                    transform.rotation = Quaternion.Euler(0, 0, LookEnemy(enemyTrans));
                    if (isAttackCo == false)
                    {
                        attackco = StartCoroutine(Attack(enemyTrans));
                    }
                    break;
                }
                yield return null;
            }
            yield return null;
        }
    }
    public Collider2D FindEnemy()
    {
        enemyTamgi = Physics2D.OverlapCircleAll(transform.position, nowRange + (playerStat["range"] / 100), enemyLayer);
        Collider2D nearEnemy = null;
        float minDis = Mathf.Infinity;

        for (int i = 0; i < enemyTamgi.Length; i++)
        {
            float distance = Vector2.Distance(transform.position, enemyTamgi[i].transform.position);
            if (distance < minDis)
            {
                minDis = distance;
                nearEnemy = enemyTamgi[i];
            }
        }
        return nearEnemy;
    }
    public float LookEnemy(Collider2D collider)
    {
        Vector2 newrot = collider.transform.position - transform.position;
        float rotz = Mathf.Atan2(newrot.y, newrot.x) * Mathf.Rad2Deg;
        return rotz;
    }
    
    IEnumerator Attack(Collider2D other)
    {
        if(playerWeapon.weaponType.ToString() == "Sword" || playerWeapon.weaponType.ToString() == "Axe")
        {
            isAttackCo = true;
            childBox.enabled = true;
            Vector2 nowTrans = transform.localPosition;
            Vector2 direction = other.transform.position - transform.position;
            Vector2 targetPosition = (Vector2)other.transform.position - (direction * 0.05f); 
            transform.position = targetPosition;
            yield return new WaitForSecondsRealtime(0.1f);
            childBox.enabled = false;
            transform.localPosition = nowTrans;
            yield return new WaitForSecondsRealtime(nowAttackSpeed / ((playerStat["attackSpeed"]) / 100));
            isAttackCo = false;
            attackco = null;
        }
        else if(playerWeapon.weaponType.ToString() == "Bow")
        {
            isAttackCo = true;
            childBox.enabled = false;
            Vector2 direction = other.transform.position - transform.position;
            float rotz = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg -90f;
            Arrow arrow = arrowPooling.ArrowPool();
            arrow.transform.SetParent(transform);
            arrow.transform.position = transform.position;
            arrow.transform.Rotate(0f, 0f, rotz + 45f);
            yield return new WaitForSecondsRealtime(nowAttackSpeed / ((playerStat["attackSpeed"]) / 100));
            isAttackCo = false;
            attackco = null;
        }
    }
}
