using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerAttack : MonoBehaviour
{
    private PlayerWeaponSO playerWeapon;
    private PlayerController playerController;
    private ArrowPooling arrowPooling;
    [SerializeField] private LayerMask enemyLayer;
    private GameObject weaponSprite;
    private BoxCollider2D childBox;
    private CircleCollider2D childCircle;
    private Transform parentTrans;
    private Coroutine co;
    private Coroutine attackco;
    private PlayerWeaponSO.WeaponType weaponType;
    private Dictionary<string, float> playerStat;

    private AssetBundle assetBundle;
    private GameObject instant;
    SpriteRenderer sr;

    private bool isCo = false;
    private bool isAttackCo = false;
    private bool nowAttack = false;
    private string weaponName;
    private Collider2D[] enemyTamgi = new Collider2D[50];
    private Collider2D enemyTrans;
    private float nowDamage;
    private float nowArmorPiercing;
    private float nowAttackSpeed;
    private float nowRange;
    private float nowCri;
    private float nowSize;
    private Sprite nowSprite;
    private bool upgrade = false;

    private void Awake()
    {
        arrowPooling = FindFirstObjectByType<ArrowPooling>();
        playerController = GetComponentInParent<PlayerController>();
        GetPlayerStat();
    }
    public void Flip()
    {
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }
    public void FlipReset()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }
    
    public float GetDamage()
    {
        return nowDamage;
    }
    public void OnPrefabLoaded(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            instant = handle.Result;
            if (instant.name == "Shield(Clone)")
            {
                sr = instant.GetComponent<SpriteRenderer>();
                sr.sprite = playerWeapon.weaponIcon;
                instant.transform.SetParent(this.transform);
                instant.transform.localScale = new Vector3(nowSize, nowSize, nowSize);
                Vector3 srPosition = transform.position;
                instant.transform.position = srPosition;
                childBox = GetComponentInChildren<BoxCollider2D>();
                childBox.enabled = false;
            }
            else if (instant.name == "Hammer(Clone)")
            {
                instant.transform.Rotate(0f, 0f, -45f);
                sr = instant.GetComponent<SpriteRenderer>();
                sr.sprite = playerWeapon.weaponIcon;
                instant.transform.SetParent(this.transform);
                instant.transform.localScale = new Vector3(nowSize, nowSize, nowSize);
                Vector3 srPosition = transform.position;
                srPosition.x += 0.3f;
                instant.transform.position = srPosition;
                childCircle = GetComponentInChildren<CircleCollider2D>();
            }
            else
            {
                instant.transform.Rotate(0f, 0f, -45f);
                sr = instant.GetComponent<SpriteRenderer>();
                sr.sprite = playerWeapon.weaponIcon;
                instant.transform.SetParent(this.transform);
                instant.transform.localScale = new Vector3(nowSize, nowSize, nowSize);
                Vector3 srPosition = transform.position;
                srPosition.x += 0.3f;
                instant.transform.position = srPosition;
                childBox = GetComponentInChildren<BoxCollider2D>();
                childBox.enabled = false;
            }
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
        parentTrans = playerController.transform;
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
        upgrade = true;
        playerWeapon.upgradeNum = j;
        SetWeaponStat(playerWeapon.upgradeNum);
        sr.sprite = nowSprite;
    }
    public void SetWeaponStat(int i)
    {
        if (upgrade == false)
        {
            nowDamage = playerWeapon.weaponDamage + playerWeapon.GetStatUpgradeDamage();
            nowArmorPiercing = playerWeapon.weaponArmorPiercing + playerWeapon.GetStatUpgradeArmorPiercing();
            nowAttackSpeed = playerWeapon.weaponAttackSpeed + playerWeapon.GetStatUpgradeAttackSpeed();
            nowRange = playerWeapon.weaponRange + playerWeapon.GetStatUpgradeRange();
            nowCri = playerWeapon.weaponCri + playerWeapon.GetStatUpgradeCri();
            nowSize = (playerWeapon.weaponSize + playerWeapon.GetStatUpgradeSize());
            nowSprite = playerWeapon.weaponIcon;
        }
        else
        {
            nowDamage = playerWeapon.weaponDamage + playerWeapon.GetUpgradeDamage(playerWeapon.upgradeNum) + playerWeapon.GetStatUpgradeDamage();
            nowArmorPiercing = playerWeapon.weaponArmorPiercing + playerWeapon.GetUpgradeArmorPiercing(playerWeapon.upgradeNum) + playerWeapon.GetStatUpgradeArmorPiercing();
            nowAttackSpeed = playerWeapon.weaponAttackSpeed + playerWeapon.GetUpgradeAttackSpeed(playerWeapon.upgradeNum) + playerWeapon.GetStatUpgradeAttackSpeed();
            nowRange = playerWeapon.weaponRange + playerWeapon.GetUpgradeRange(playerWeapon.upgradeNum) + playerWeapon.GetStatUpgradeRange();
            nowCri = playerWeapon.weaponCri + playerWeapon.GetUpgradeCri(playerWeapon.upgradeNum) + playerWeapon.GetStatUpgradeCri();
            nowSize = (playerWeapon.GetUpgradeSize(playerWeapon.upgradeNum) + playerWeapon.GetStatUpgradeSize());
            nowSprite = playerWeapon.GetUpgradeSprite(playerWeapon.upgradeNum);
        }
        nowDamage = nowDamage * ((playerStat["damage"] + 100f) / 100f);
        nowArmorPiercing = nowArmorPiercing + playerStat["armorPiercing"];
        nowAttackSpeed = nowAttackSpeed / ((playerStat["attackSpeed"] + 100f) / 100f);
        if(nowAttackSpeed < 0.1f)
        {
            nowAttackSpeed = 0.1f;
        }
        nowRange = nowRange + (playerStat["range"] / 100);
        nowCri = nowCri + playerStat["cri"];
        if(instant != null)
        {
            instant.transform.localScale = new Vector3(nowSize, nowSize, nowSize);
        }
    }
    public float GetUpgradeDamage()
    {
        return nowDamage;
    }
    public float GetUpgradeArmorPiercing()
    {
        return nowArmorPiercing;
    }
    public float GetUpgradeAttackSpeed()
    {
        return nowAttackSpeed;
    }
    public float GetUpgradeRange()
    {
        return nowRange;
    }
    public float GetUpgradeCri()
    {
        return nowCri;
    }
    public float GetUpgradeSize()
    {
        return nowSize;
    }
    IEnumerator Weapon()
    {
        SetWeaponStat(0);
        yield return new WaitForSecondsRealtime(1.0f);
        sr.sprite = nowSprite;

        while (true)
        {
            while (true)
            {
                enemyTrans = FindEnemy();
                if (enemyTrans != null) {
                    if(nowAttack == false)
                    {
                    transform.rotation = Quaternion.Euler(0, 0, LookEnemy(enemyTrans));
                    }
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
    public void AttackSwingMotion(Transform targetPosi)
    {
        DG.Tweening.Sequence motion = DOTween.Sequence();
        Vector2 nowTrans = transform.localPosition;
        Vector2 direction = (Vector2)targetPosi.position - (Vector2)parentTrans.transform.position;
        Vector2 basePosi = direction.normalized;
        Vector2 rightDir = new Vector2(-basePosi.y, basePosi.x);

        float Posi = playerWeapon.GetStatUpgradeSize();
        Posi = Posi / 10;
        Vector3 pullPosi = targetPosi.position - (Vector3)(direction * 0.6f);
        Vector3 pullMainPosi = targetPosi.position - (Vector3)(direction * 0.2f);
        

        Vector3 leftPosi = pullPosi + (Vector3)(rightDir * (1.6f + Posi));
        Vector3 rightPosi = pullPosi - (Vector3)(rightDir * (1.6f + Posi));

        Quaternion rotate = transform.rotation;
        Quaternion leftRotate = rotate * Quaternion.Euler(0, 0, 30.6f);
        Quaternion rightRotate = rotate * Quaternion.Euler(0, 0, -30.6f);

        motion.Append(transform.DOLocalRotateQuaternion(leftRotate, 0f));
        motion.Append(transform.DOMove(leftPosi, 0.05f));
        motion.Append(transform.DOLocalRotateQuaternion(rotate, 0f));
        motion.Append(transform.DOMove(pullMainPosi, 0.1f));
        motion.Append(transform.DOLocalRotateQuaternion(rightRotate, 0f));
        motion.Append(transform.DOMove(rightPosi, 0.1f));
        motion.Append(transform.DOLocalMove(nowTrans, 0.05f));
        motion.OnComplete (() => { playerController.SetWeaponArm(); });
    }
    public void AttackHammerMotion(Transform targetPosi)
    {
        DG.Tweening.Sequence motion = DOTween.Sequence();
        DG.Tweening.Sequence motion2 = DOTween.Sequence();
        DG.Tweening.Sequence motion3 = DOTween.Sequence();
        DG.Tweening.Sequence motionAll = DOTween.Sequence();
        Vector2 nowTrans = transform.localPosition;
        Vector2 direction = (Vector2)targetPosi.position - (Vector2)parentTrans.transform.position;
        Vector2 basePosi = direction.normalized;
        Vector2 rightDir = new Vector2(-basePosi.y, basePosi.x);

        float Posi = playerWeapon.GetStatUpgradeSize();
        Posi = Posi / 10;
        Vector3 pullPosi = targetPosi.position - (Vector3)(direction * 1.2f);
        Vector3 pullMainPosi = targetPosi.position - (Vector3)(direction * 0.2f);


        Vector3 leftPosi = pullPosi + (Vector3)(rightDir * (0.2f + Posi));
        Vector3 rightPosi = pullPosi - (Vector3)(rightDir * (0.2f + Posi));

        Quaternion rotate = transform.rotation;
        Quaternion leftRotate = rotate * Quaternion.Euler(0, 0, 90.6f);
        Quaternion rightRotate = rotate * Quaternion.Euler(0, 0, -90.6f);

        if (transform.rotation.z >= -90f && transform.rotation.z <= 90f)
        {
            motion.Join(transform.DOLocalRotateQuaternion(leftRotate, 0f));
            motion.Join(transform.DOMove(pullPosi, 0f));
        }
        else if(transform.rotation.z <= -90f || transform.rotation.z >= 90f)
        {
            motion.Join(transform.DOLocalRotateQuaternion(rightRotate, 0f));
            motion.Join(transform.DOMove(pullPosi, 0f));
        }
        motion2.Join(transform.DOMove(pullMainPosi, 0.4f));
        motion2.Join(transform.DOLocalRotateQuaternion(rotate, 0.4f));
        
        

        motionAll.Append(motion);
        motionAll.Append(motion2);
        motionAll.Append(transform.DOLocalMove(nowTrans, 0.0f));
        motionAll.OnComplete(() => { playerController.SetWeaponArm(); });
    }
    public void AttackKatanaMotion(Transform targetPosi)
    {
        DG.Tweening.Sequence motion = DOTween.Sequence();
        DG.Tweening.Sequence motion2 = DOTween.Sequence();
        DG.Tweening.Sequence motion3 = DOTween.Sequence();
        DG.Tweening.Sequence motionAll = DOTween.Sequence();
        Vector2 nowTrans = transform.localPosition;
        Vector2 direction = (Vector2)targetPosi.position - (Vector2)parentTrans.transform.position;
        Vector2 basePosi = direction.normalized;
        Vector2 rightDir = new Vector2(-basePosi.y, basePosi.x);

        float Posi = playerWeapon.GetStatUpgradeSize();
        Posi = Posi / 10;
        Vector3 pullPosi = targetPosi.position - (Vector3)(direction * 1f);
        Vector3 pullMainPosi = targetPosi.position - (Vector3)(direction * 0.2f);


        Vector3 leftPosi = pullPosi + (Vector3)(rightDir * (0.8f + Posi));
        Vector3 rightPosi = pullPosi - (Vector3)(rightDir * (0.8f + Posi));

        Quaternion rotate = transform.rotation;
        Quaternion leftRotate = rotate * Quaternion.Euler(0, 0, 70.6f);
        Quaternion rightRotate = rotate * Quaternion.Euler(0, 0, -70.6f);

        motion.Join(transform.DOLocalRotateQuaternion(leftRotate, 0.1f));
        motion.Join(transform.DOMove(leftPosi, 0.1f));
        
        
        motion2.Join(transform.DOLocalRotateQuaternion(rotate, 0.1f));
        motion2.Join(transform.DOMove(pullMainPosi, 0.1f));
        

        motion3.Join(transform.DOLocalRotateQuaternion(rightRotate, 0.1f));
        motion3.Join(transform.DOMove(rightPosi, 0.1f));

        motionAll.Append(motion);
        motionAll.Append(motion2);
        motionAll.Append(motion3);
        
        motionAll.Append(transform.DOLocalMove(nowTrans, 0.0f));
        motionAll.OnComplete(() => { playerController.SetWeaponArm(); });
    }
    public void AttackRotateMotion(Transform targetPosi)
    {
        DG.Tweening.Sequence motion = DOTween.Sequence();
        Vector2 nowTrans = transform.localPosition;
        Vector2 direction = (Vector2)targetPosi.position - (Vector2)parentTrans.transform.position;
        Vector2 basePosi = direction.normalized;
        Vector2 rightDir = new Vector2(-basePosi.y, basePosi.x);
        float radius = direction.magnitude;
        float angle = 0f;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0f) angle += 360f;
        float startAngle = angle;
        float endAngle = angle + 360f;

        motion.Join(transform.DORotate(new Vector3(0f, 0f, 1080f), 0.6f, RotateMode.FastBeyond360).SetEase(Ease.Linear));
        motion.Join(
            DOTween.To(() => startAngle, x => startAngle = x, endAngle, 0.6f).SetEase(Ease.Linear).OnUpdate(() =>
            {
                float rad = startAngle * Mathf.Deg2Rad;
                Vector3 next = playerController.transform.position + new Vector3(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius, 0);
                transform.position = next;
            }));
        motion.Play();
        motion.OnComplete(() => { playerController.SetWeaponArm(); });
    }
    public void AttackStingMotion(Transform targetPosi)
    {
        DG.Tweening.Sequence motion = DOTween.Sequence();
        Vector2 nowTrans = transform.localPosition;
        Vector2 direction = (Vector2)targetPosi.position - (Vector2)parentTrans.transform.position;
        Vector2 basePosi = direction.normalized;

        Vector3 pullPosi = targetPosi.position + (Vector3)(direction * 0.6f);

        Quaternion rotate = transform.rotation;

        motion.Append(transform.DOMove(pullPosi, 0.15f));
        motion.Append(transform.DOLocalMove(nowTrans, 0.15f));
        motion.OnComplete(() => { playerController.SetWeaponArm(); });
    }
    IEnumerator Attack(Collider2D other)
    {
        if (playerWeapon.weaponType.ToString() == "Sword" 
            || playerWeapon.weaponType.ToString() == "Axe" 
            || playerWeapon.weaponType.ToString() == "Shield")
        {
            isAttackCo = true;
            nowAttack = true;
            childBox.enabled = true;
            AttackSwingMotion(other.transform);
            yield return new WaitForSecondsRealtime(0.3f);
            childBox.enabled = false;
            nowAttack = false;
            yield return new WaitForSecondsRealtime(nowAttackSpeed);
            isAttackCo = false;
            attackco = null;
        }
        else if (playerWeapon.weaponType.ToString() == "Hammer")
        {
            isAttackCo = true;
            nowAttack = true;
            AttackHammerMotion(other.transform);
            yield return new WaitForSecondsRealtime(0.3f);
            childCircle.enabled = true;
            yield return new WaitForSecondsRealtime(0.1f);
            childCircle.enabled = false;
            nowAttack = false;
            yield return new WaitForSecondsRealtime(nowAttackSpeed);
            isAttackCo = false;
            attackco = null;
        }
        else if(playerWeapon.weaponType.ToString() == "Katana")
        {
            isAttackCo = true;
            nowAttack = true;
            childBox.enabled = true;
            AttackKatanaMotion(other.transform);
            yield return new WaitForSecondsRealtime(0.3f);
            childBox.enabled = false;
            nowAttack = false;
            yield return new WaitForSecondsRealtime(nowAttackSpeed);
            isAttackCo = false;
            attackco = null;
        }
        else if (playerWeapon.weaponType.ToString() == "TwinBlade")
        {
            isAttackCo = true;
            nowAttack = true;
            childBox.enabled = true;
            AttackRotateMotion(other.transform);
            yield return new WaitForSecondsRealtime(0.6f);
            childBox.enabled = false;
            nowAttack = false;
            yield return new WaitForSecondsRealtime(nowAttackSpeed);
            isAttackCo = false;
            attackco = null;
        }
        else if (playerWeapon.weaponType.ToString() == "Spear")
        {
            isAttackCo = true;
            nowAttack = true;
            childBox.enabled = true;
            AttackStingMotion(other.transform);
            yield return new WaitForSecondsRealtime(0.3f);
            childBox.enabled = false;
            nowAttack = false;
            yield return new WaitForSecondsRealtime(nowAttackSpeed);
            isAttackCo = false;
            attackco = null;
        }
        else if (playerWeapon.weaponType.ToString() == "Bow" || playerWeapon.weaponType.ToString() == "CrossBow")
        {
            isAttackCo = true;
            childBox.enabled = false;
            Vector2 direction = other.transform.position - transform.position;
            float rotz = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Arrow arrow = arrowPooling.ArrowPool();
            arrow.transform.SetParent(transform);
            arrow.transform.position = transform.position;
            arrow.transform.Rotate(0f, 0f, rotz + 45f);
            arrow.transform.localScale = new Vector3(nowSize / 2, nowSize / 2, nowSize / 2);
            yield return new WaitForSecondsRealtime(nowAttackSpeed);
            isAttackCo = false;
            attackco = null;
        }
    }
}
