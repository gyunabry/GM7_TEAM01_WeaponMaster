using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    public static InGameUIManager Instance {  get; private set; }

    [Header("구독할 이벤트")]
    [SerializeField] private VoidEventChannel playerDeadEvent;

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

    [Header("킬 카운트")]
    [SerializeField] private TMP_Text killCountText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 모든 오브젝트의 Awake가 끝난 시점인 Start에서 이벤트를 구독
    private void Start()
    {
        // 이벤트 구독
        if (player != null)
        {
            player.OnHpChanged += UpdateHpUI;
        }
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnWaveStarted += UpdateWaveUI;
            WaveManager.Instance.OnTimeChanged += UpdateTimerUI;
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnExpChanged += UpdateExpUI;
            GameManager.Instance.OnKillEnemy += UpdateKillCount;
        }

        // 게임 첫 시작 시 UI 초기화
        UpdateKillCount(GameManager.Instance.GetKillCount());
        UpdateWaveUI(WaveManager.Instance.CurrentWave);
        UpdateTimerUI(WaveManager.Instance.WaveTime);
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnHpChanged -= UpdateHpUI;
        }
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnWaveStarted -= UpdateWaveUI;
            WaveManager.Instance.OnTimeChanged -= UpdateTimerUI;
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnExpChanged -= UpdateExpUI;
            GameManager.Instance.OnKillEnemy -= UpdateKillCount;
        }
    }

    private void UpdateHpUI(float currentHp, float maxHp)
    {
        if(hpSlider != null && maxHp > 0)
        {
            hpSlider.maxValue = maxHp;
            hpSlider.value = currentHp;
        }
        if(hpText!=null)
        {
            hpText.text = $"{Mathf.RoundToInt(currentHp)}/{Mathf.RoundToInt(maxHp)}";
        }
    }

    private void UpdateExpUI(int currentExp, int maxExp)
    {
        if(expSlider != null && maxExp > 0)
        {
            // 슬라이더의 최대값 설정
            expSlider.maxValue = maxExp;

            // 슬라이더에 현재값을 적용
            expSlider.value = currentExp;
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
            //웨이브 데이터 0으로 대기시간 만들때 웨이브 카운트 오류 개선용
            waveCountText.text =$"Wave {curretnWave-1}";
        }
    }

    private void OpenUIwithWaveStart(int currentWave)
    {
        // if (waveUIs != null) waveUIs.SetActive(true);
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

    private void UpdateKillCount(int count)
    {
        if (killCountText != null)
        {
            killCountText.text = count.ToString();
        }
    }
}
