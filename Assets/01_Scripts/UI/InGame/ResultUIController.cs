using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultUIController : MonoBehaviour
{
    [Header("결과창 UI")]
    [SerializeField] private GameObject resultRoot;

    [Header("텍스트")]
    [SerializeField] private TMP_Text stageText;
    [SerializeField] private TMP_Text clearTimeText;
    [SerializeField] private TMP_Text playerLevelText;
    [SerializeField] private TMP_Text killCountText;

    [Header("무기 통계")]
    [SerializeField] private Transform weaponSlotParent;
    [SerializeField] private List<ResultWeaponSlotUI> fixedWeaponSlots = new List<ResultWeaponSlotUI>();
    [SerializeField] private bool hideEmptyWeaponSlots = true;

    [Header("버튼")]
    [SerializeField] private Button lobbyButton;

    [Header("참조")]
    [SerializeField] private PlayerController player;

    private void Awake()
    {
        if (lobbyButton != null)
        {
            lobbyButton.onClick.AddListener(GoToLobby);
        }

        Hide();
    }

    private void OnDestroy()
    {
        if (lobbyButton != null)
        {
            lobbyButton.onClick.RemoveListener(GoToLobby);
        }
    }

    public void ShowResult()
    {
        Show(BuildResultData());
    }

    public void Show(ResultData resultData)
    {
        if (resultData == null) return;

        if (resultRoot != null)
        {
            resultRoot.SetActive(true);
        }
        SetText(stageText, resultData.stageName + " - " + resultData.difficulty.ToString());
        SetText(clearTimeText, FormatTime(resultData.clearTime));
        SetText(playerLevelText, resultData.playerLevel.ToString());
        SetText(killCountText, resultData.killCount.ToString());

        ShowWeaponResults(resultData.weapons);
    }

    public void Hide()
    {
        if (resultRoot != null)
        {
            resultRoot.SetActive(false);
        }
    }

    private ResultData BuildResultData()
    {
        StageData selectedStage = GameSceneData.SelectedStage;

        ResultData resultData = new ResultData
        {
            stageName = selectedStage.stageName,
            difficulty = GameSceneData.SelectedDifficulty,
            clearTime = Time.timeSinceLevelLoad,
            playerLevel = GameManager.Instance != null ? GameManager.Instance.Level : 0,
            killCount = GameManager.Instance != null ? GameManager.Instance.KillCount : 0,
            weapons = BuildWeaponResults()
        };

        return resultData;
    }

    private List<ResultWeaponData> BuildWeaponResults()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<PlayerController>();
        }

        if (player == null)
        {
            return new List<ResultWeaponData>();
        }

        Dictionary<PlayerWeaponSO.WeaponType, PlayerWeaponSO> weaponMap = player.GetWeaponList();
        if (weaponMap == null)
        {
            return new List<ResultWeaponData>();
        }

        return weaponMap.Values
            .Where(weapon => weapon != null && weapon.weaponType != PlayerWeaponSO.WeaponType.Null)
            .OrderBy(weapon => weapon.weaponId)
            .Select(weapon => new ResultWeaponData
            {
                icon = weapon.weaponIcon,
                level = weapon.upgradeCount,
                damage = weapon.totalDamage
            })
            .ToList();
    }

    private void ShowWeaponResults(List<ResultWeaponData> weapons)
    {
        if (weapons == null)
        {
            weapons = new List<ResultWeaponData>();
        }

        ShowFixedWeaponSlots(weapons);
    }

    private void ShowFixedWeaponSlots(List<ResultWeaponData> weapons)
    {
        for (int i = 0; i < fixedWeaponSlots.Count; i++)
        {
            ResultWeaponSlotUI slot = fixedWeaponSlots[i];
            if (slot == null) continue;

            bool hasData = i < weapons.Count;
            slot.gameObject.SetActive(hasData || !hideEmptyWeaponSlots);

            if (hasData)
            {
                slot.Set(weapons[i]);
            }
            else
            {
                slot.Clear();
            }
        }
    }

    private void GoToLobby()
    {
        Time.timeScale = 1f;

        if (GameSceneManager.Instance != null)
        {
            GameSceneManager.Instance.LoadScene(SceneType.Title);
            return;
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"{minutes:D2} : {seconds:D2}";
    }

    private void SetText(TMP_Text text, string value)
    {
        if (text != null)
        {
            text.text = value;
        }
    }
}
