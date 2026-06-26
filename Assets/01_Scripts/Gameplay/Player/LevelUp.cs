using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class LevelUp : MonoBehaviour
{
    [SerializeField] private PlayerWeaponManager playerWeapon;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Button button;

    private Dictionary<PlayerWeaponSO.WeaponType, PlayerWeaponSO> equWeaponList = new Dictionary<PlayerWeaponSO.WeaponType, PlayerWeaponSO>();
    private PlayerWeaponSO[] weaponList;
    private Image[] weaponImage = new Image[3];
    private Button[] go = new Button[3];
    private Coroutine co;
    private PlayerAttack weaponStat;

    private string[] weaponDes = new string[3];
    private int[] ran = new int[3];
    private int i = 0;
    private int dho = 0;
    private int[] ranAvo = new int[3];
    private int[] ranUp = new int[3];
    private string iconName;
    private string iconNameAvo;
    
    private void OnEnable()
    {
        equWeaponList = playerController.GetWeaponList();
        weaponList = playerWeapon.GetWeaponList();
        if (transform.childCount == 3)
        {
            go[0].gameObject.SetActive(false);
            go[1].gameObject.SetActive(false);
            go[2].gameObject.SetActive(false);
        }
        GetImageTask();
    }

    public async Task GetImageTask()
    {
        for (i = 0; i < 3; i++)
        {
            while (true)
            {
                ran[i] = Random.Range(0, weaponList.Length);
                int upRan = Random.Range(0, 101);
                if (i == 1)
                {
                    if (ran[i - 1] == ran[i])
                    {
                        continue;
                    }
                }
                else if (i == 2)
                {
                    if (ran[i - 2] == ran[i] || ran[i - 1] == ran[i])
                    {
                        continue;
                    }
                }
                if (equWeaponList.TryGetValue(weaponList[ran[i]].weaponType, out var value))
                {
                    
                }
                else
                {
                    if (40 <= upRan || playerController.transform.childCount >= 8)
                    {
                        continue;
                    }
                }
                break;
            }
            PlayerWeaponSO pws;
            if (equWeaponList.TryGetValue(weaponList[ran[i]].weaponType, out pws))
            {
                weaponDes[i] = weaponList[ran[i]].weaponDes;
                iconName = weaponList[ran[i]].weaponIcon.ToString().Replace("_0 (UnityEngine.Sprite)", "");
                if (transform.childCount != 3)
                {
                    go[i] = Instantiate(button);
                }
                while (true)
                {
                    int randomUpgrade = Random.Range(0, 101);
                    if(0 <= randomUpgrade && randomUpgrade <= 35) // 대미지
                    {
                        ranUp[i] = 0;
                    }
                    else if(36 <= randomUpgrade && randomUpgrade <= 65) //공속
                    {
                        ranUp[i] = 2;
                    }
                    else if(71 <= randomUpgrade && randomUpgrade <= 90) //크확
                    {
                        ranUp[i] = 4;
                    }
                    else if(91 <= randomUpgrade && randomUpgrade <= 100) //범위
                    {
                        ranUp[i] = 3;
                    }
                    if (ranUp[i] == 2 && weaponList[ran[i]].GetStatUpgradeAttackSpeed() <= 0.2f)
                    {
                        continue;
                    }
                    break;
                }
                if (weaponList[ran[i]].upgradeCount == 8) //무기 진화 메서드
                {
                    if (i == 0)
                    {
                        go[0].onClick.AddListener(() => GetUpgrade(0));
                    }
                    else if (i == 1)
                    {
                        go[1].onClick.AddListener(() => GetUpgrade(1));
                    }
                    else if (i == 2)
                    {
                        go[2].onClick.AddListener(() => GetUpgrade(2));
                    }
                }
                else //무기 업글 메서드
                {
                    if (i == 0)
                    {
                        go[0].onClick.AddListener(() => GetStatUpgrade(0));
                    }
                    else if (i == 1)
                    {
                        go[1].onClick.AddListener(() => GetStatUpgrade(1));
                    }
                    else if (i == 2)
                    {
                        go[2].onClick.AddListener(() => GetStatUpgrade(2));
                    }
                }
                ranAvo[i] = Random.Range(0, weaponList[ran[i]].upgrades.Count);
                TextMeshProUGUI text = go[i].GetComponentInChildren<TextMeshProUGUI>();
                TextMeshProUGUI[] upText = go[i].GetComponentsInChildren<TextMeshProUGUI>();
                if (weaponList[ran[i]].upgradeCount != 0)
                {
                    upText[1].text = $"Level {weaponList[ran[i]].upgradeCount}";
                }
                else if(weaponList[ran[i]].upgradeCount == 0)
                {
                    upText[1].text = "";
                }
                if (weaponList[ran[i]].upgradeCount == 8) // 무기 진화 텍스트
                {
                    iconNameAvo = weaponList[ran[i]].upgrades[ranAvo[i]].upgradeSprite.ToString().Replace("_0 (UnityEngine.Sprite)", "");
                    text.text = $"{weaponList[ran[i]].upgrades[ranAvo[i]].upgradeDes}";
                    Image[] childImageAvo = go[i].GetComponentsInChildren<Image>();
                    weaponImage[i] = childImageAvo[1];
                    Sprite spriteAvo = await Addressables.LoadAssetAsync<Sprite>(iconNameAvo).Task;
                    weaponImage[dho].sprite = spriteAvo;
                    dho++;
                }
                else // 무기 업글 텍스트
                {
                    if (ranUp[i] == 0)
                    {
                        text.text = $"공격력 {weaponList[ran[i]].GetUpValueDamage()} 증가";
                    }
                    else if (ranUp[i] == 1)
                    {
                        text.text = $"방어력 관통 {weaponList[ran[i]].GetUpValueArmorPiercing()} 증가";
                    }
                    else if (ranUp[i] == 2)
                    {
                        text.text = $"공격 속도 {weaponList[ran[i]].GetUpValueAttackSpeed()} 증가";
                    }
                    else if (ranUp[i] == 3)
                    {
                        text.text = $"범위 {weaponList[ran[i]].GetUpValueRange()} 증가";
                    }
                    else if (ranUp[i] == 4)
                    {
                        text.text = $"크리티컬 확률 {weaponList[ran[i]].GetUpValueCri()} 증가";
                    }
                    else if (ranUp[i] == 5)
                    {
                        text.text = $"크기 {weaponList[ran[i]].GetUpValueSize()} 증가";
                    }
                    Image[] childImage = go[i].GetComponentsInChildren<Image>();
                    weaponImage[i] = childImage[1];
                    Sprite sprite = await Addressables.LoadAssetAsync<Sprite>(iconName).Task;
                    weaponImage[dho].sprite = sprite;
                    dho++;
                }
            }
            else //무기 생성
            {
                weaponDes[i] = weaponList[ran[i]].weaponDes;
                iconName = weaponList[ran[i]].weaponIcon.ToString().Replace("_0 (UnityEngine.Sprite)", "");
                if (transform.childCount != 3)
                {
                    go[i] = Instantiate(button);
                }
                Image[] childImage = go[i].GetComponentsInChildren<Image>();
                weaponImage[i] = childImage[1];
                Sprite sprite = await Addressables.LoadAssetAsync<Sprite>(iconName).Task;
                weaponImage[dho].sprite = sprite;
                dho++;
                go[i].transform.SetParent(transform);
                if (i == 0)
                {
                    go[0].onClick.AddListener(() => GetWeapon(0));
                }
                else if (i == 1)
                {
                    go[1].onClick.AddListener(() => GetWeapon(1));
                }
                else if (i == 2)
                {
                    go[2].onClick.AddListener(() => GetWeapon(2));
                }
                TextMeshProUGUI text = go[i].GetComponentInChildren<TextMeshProUGUI>();
                text.text = weaponDes[i] + "";
                TextMeshProUGUI[] upText = go[i].GetComponentsInChildren<TextMeshProUGUI>();
                upText[1].text = "";
                
            }
            if (i == 0)
            {
                go[i].transform.localPosition = new Vector3(-300f, 0f, 0f);
            }
            else if (i == 1)
            {
                go[i].transform.localPosition = new Vector3(0f, 0f, 0f);
            }
            else if (i == 2)
            {
                go[i].transform.localPosition = new Vector3(300f, 0f, 0f);
            }
            go[i].gameObject.SetActive(true);
        }
    }
    //무기 획득
    public void GetWeapon(int jk)
    {
        playerController.OnWeaponArm(weaponList[ran[jk]].weaponType);
        i = 0;
        dho = 0;
        go[0].onClick.RemoveAllListeners();
        go[1].onClick.RemoveAllListeners();
        go[2].onClick.RemoveAllListeners();
        gameObject.SetActive(false);

        playerController.SetWeaponArm();
        GameManager.Instance.ResumeGame();
    }

    //스탯업그레이드
    public void GetStatUpgrade(int jk)
    {
        if (ranUp[jk] == 0)
        {
            weaponList[ran[jk]].AddStatUpgradeDamage(weaponList[ran[jk]].GetUpValueDamage());
        }
        else if (ranUp[jk] == 1)
        {
            weaponList[ran[jk]].AddStatUpgradeArmorPiercing(weaponList[ran[jk]].GetUpValueArmorPiercing());
        }
        else if (ranUp[jk] == 2)
        {
            weaponList[ran[jk]].AddStatUpgradeAttackSpeed(weaponList[ran[jk]].GetUpValueAttackSpeed());
        }
        else if (ranUp[jk] == 3)
        {
            weaponList[ran[jk]].AddStatUpgradeRange(weaponList[ran[jk]].GetUpValueRange());
        }
        else if (ranUp[jk] == 4)
        {
            weaponList[ran[jk]].AddStatUpgradeCri(weaponList[ran[jk]].GetUpValueCri());
        }
        else if (ranUp[jk] == 5)
        {
            weaponList[ran[jk]].AddStatUpgradeSize(weaponList[ran[jk]].GetUpValueSize());
        }
        weaponList[ran[jk]].upgradeCount++;
        Dictionary<PlayerWeaponSO.WeaponType, GameObject> imsiList = playerController.GetArm();
        if (imsiList.TryGetValue(weaponList[ran[jk]].weaponType, out var arm))
        {
            weaponStat = arm.GetComponent<PlayerAttack>();
            weaponStat.SetWeaponStat(0);
        }
        i = 0;
        dho = 0;
        go[0].onClick.RemoveAllListeners();
        go[1].onClick.RemoveAllListeners();
        go[2].onClick.RemoveAllListeners();
        gameObject.SetActive(false);

        playerController.SetWeaponArm();
        GameManager.Instance.ResumeGame();
    }
    //무기 진화
    public void GetUpgrade(int jk)
    {
        Dictionary<PlayerWeaponSO.WeaponType, GameObject> imsiList = playerController.GetArm();
        if (imsiList.TryGetValue(weaponList[ran[jk]].weaponType, out var weapon)){
            PlayerAttack pa = weapon.GetComponent<PlayerAttack>();
            pa.GetUpgrade(ranAvo[jk]);
            weaponList[ran[jk]].upgradeCount++;
        }
        i = 0;
        dho = 0;
        go[0].onClick.RemoveAllListeners();
        go[1].onClick.RemoveAllListeners();
        go[2].onClick.RemoveAllListeners();
        gameObject.SetActive(false);

        playerController.SetWeaponArm();
        GameManager.Instance.ResumeGame();
    }
}
    
