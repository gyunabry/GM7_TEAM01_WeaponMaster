using System.Collections;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    public static InGameUIManager Instance {  get; private set; }

    [Header("구독할 이벤트")]
    [SerializeField] private VoidEventChannel playerDeadEvent;
    [SerializeField] private VoidEventChannel bossEncounterEvent;
    [SerializeField] private HpEventChannel bossDamagedEvent;

    [Header("플레이어")]
    [SerializeField] private PlayerController player;

    [Header("HP UI")]
    [SerializeField] private Image hpFill;
    [SerializeField] private Image hpEase;
    [SerializeField] private TextMeshProUGUI hpText;
    private float lerpSpeed = 5f;
    private Coroutine hpEaseCoroutine;

    [Header("보스 UI")]
    [Tooltip("보스 출현 시 보여줄 UI")]
    [SerializeField] private CanvasGroup bossUI;
    [SerializeField] private TMP_Text bossNameText;
    [SerializeField] private Image bossHpFill;
    [SerializeField] private float fadeInDuration = 0.5f;

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
        if (bossEncounterEvent != null)
        {
            bossEncounterEvent.OnEventRaised += ShowBossUI;
        }
        if (bossDamagedEvent != null)
        {
            bossDamagedEvent.OnEventRaised += UpdateBossHp;
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
        UpdateExpUI(GameManager.Instance.CurrentExp, GameManager.Instance.RequireExp[GameManager.Instance.Level]);

        // 첫 시작 시 보스 UI는 비활성화
        CanvasGroupController.DisableCG(bossUI);
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnHpChanged -= UpdateHpUI;
        }
        if (bossEncounterEvent != null)
        {
            bossEncounterEvent.OnEventRaised -= ShowBossUI;
        }
        if (bossDamagedEvent != null)
        {
            bossDamagedEvent.OnEventRaised -= UpdateBossHp;
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
        if(hpFill != null && maxHp > 0)
        {
            hpFill.fillAmount = currentHp / maxHp;

            if (hpEaseCoroutine != null)
            {
                StopCoroutine(hpEaseCoroutine);
            }
            hpEaseCoroutine = StartCoroutine(HpEaseCo());
        }
        
        if (hpText != null)
        {
            hpText.text = currentHp.ToString();
        }
    }

    private IEnumerator HpEaseCo()
    {
        // 피격되고 잠깐의 대기시간 이후 체력바 감소
        yield return new WaitForSeconds(0.2f);

        // Lerp는 계속 목표값으로 작아지기 때문에 그 차이가 매우 작아지면 중단
        while (Mathf.Abs(hpEase.fillAmount - hpFill.fillAmount) > 0.001f)
        {
            hpEase.fillAmount = Mathf.Lerp(hpEase.fillAmount, hpFill.fillAmount, lerpSpeed * Time.deltaTime);
            yield return null;
        }

        hpEase.fillAmount = hpFill.fillAmount;
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

    #region 보스 관련
    private void ShowBossUI()
    {
        CanvasGroupController.EnableCG(bossUI);
        bossUI.DOFade(1f, fadeInDuration).SetUpdate(true);
    }

    // 보스 초기화 시 호출해 이름 설정
    public void SetBossInfo(string bossName, float currentHp, float maxHp)
    {
        if (bossNameText != null)
        {
            bossNameText.text = bossName;
        }

        if (bossHpFill != null && maxHp > 0f)
        {
            bossHpFill.fillAmount = currentHp / maxHp;
        }
    }

    private void UpdateBossHp(float currentHp, float maxHp)
    {
        bossHpFill.fillAmount = (float)currentHp / maxHp;
    }
    #endregion

    #region 웨이브
    private void UpdateWaveUI(int curretnWave)
    {
        if(waveCountText!=null)
        {
            //웨이브 데이터 0으로 대기시간 만들때 웨이브 카운트 오류 개선용
            waveCountText.text =$"Wave {curretnWave-1}";
        }
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
    #endregion
}
