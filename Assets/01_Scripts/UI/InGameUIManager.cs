using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    public static InGameUIManager Instance {  get; private set; }

    [Header("상점 진입하면 끌 UI")]
    [SerializeField] private GameObject waveUIs;//골드 제외하고 모든 UI 잠시 끄기위해

    [Header("플레이어")]
    [SerializeField] private PlayerController player;

    [Header("HP UI")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpText;

    [Header("경험치 UI")]
    [SerializeField] private Slider expSlider;
    [SerializeField] private TextMeshProUGUI expText;

    [Header("보유 골드 및 스테이지 UI")]
    [SerializeField] private TextMeshProUGUI goldAmountText;
    [SerializeField] private TextMeshProUGUI waveCountText;
    [SerializeField] private TextMeshProUGUI waveTimerText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        
    }
    private void OnEnable()
    {
        if(player != null)
        {
            //player.OnHpChanged += UpdateHpUI;
            //player.OnExpChanged += UpdateEXPUI;
            //player.OnGoldChanged += UpdateGoldUI;
        }
        if(WaveManager.Instance != null)
        {
            WaveManager.Instance.OnWaveStarted += OpenUIwithWaveStart;
            WaveManager.Instance.OnTimeChanged += UpdateTimerUI;
            WaveManager.Instance.OnShopOpened += CloseUIwithWaveEnds;
        }
    }

    private void OnDisable()
    {
        if(player!=null)
        {
            //player.OnHpChanged -= UpdateHpUI;
            //player.OnExpChanged -= UpdateEXPUI;
            //player.OnGoldChanged -= UpdateGoldUI;
        }
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnWaveStarted -= OpenUIwithWaveStart;
            WaveManager.Instance.OnTimeChanged -= UpdateTimerUI;
            WaveManager.Instance.OnShopOpened -= CloseUIwithWaveEnds;
        }
    }
    
    private void UpdateHpUI(float currentHp, float maxHp)
    {
        if(hpSlider != null && maxHp>0)
        {
            hpSlider.value = currentHp / maxHp;
        }
        if(hpText!=null)
        {
            hpText.text = $"{Mathf.RoundToInt(currentHp)}/{Mathf.RoundToInt(maxHp)}";
        }
    }
    private void UpdateEXPUI(float currentExp, float maxExp)
    {
        if(expSlider != null && maxExp>0)
        {
            expSlider.value = currentExp / maxExp;
        }
        if(expText!=null)
        {
            expText.text =$"{Mathf.RoundToInt(currentExp)}/{Mathf.RoundToInt(maxExp)}";
        }
    }

    private void UpdateGoldUI(int moneyPcket)
    {
        if(goldAmountText!=null && moneyPcket >=0)
        {
            goldAmountText.text = moneyPcket.ToString("N0");
        }
    }
    private void UpdateWaveUI(int curretnWave)
    {
        if(waveCountText!=null)
        {
            waveCountText.text =$"Wave {curretnWave}";
        }
    }

    private void OpenUIwithWaveStart(int currentWave)
    {
        if (waveUIs != null) waveUIs.SetActive(true);
        if (waveCountText != null) waveCountText.text = $"Wave {currentWave}";
        if (waveTimerText != null) waveTimerText.text = "00 : 00";
    }
    private void UpdateTimerUI(float playTime)
    {
        if (waveTimerText == null) return;
       ;
        int minutes = Mathf.FloorToInt(playTime / 60f);
        int seconds = Mathf.FloorToInt(playTime % 60f);
        waveTimerText.text = $"{minutes:D2} : {seconds:D2}";
    }
    private void CloseUIwithWaveEnds()
    {
        if(waveUIs != null)
        {
            waveUIs.SetActive(false);
        }
    }
}
