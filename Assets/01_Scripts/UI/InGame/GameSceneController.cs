using System.Collections;
using System.Runtime.CompilerServices;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Timeline.DirectorControlPlayable;

public class GameSceneController : MonoBehaviour
{
    // 현재 인게임 UI 상태를 한 번에 관리하기 위해 열거형으로 선언
    public enum GameSceneState
    {
        Playing,
        PauseMenu,
        OptionMenu,
        GameOver,
        LevelUp,
        Result
    }

    public static GameSceneController Instance { get; private set; }

    [Header("플레이어 사망 이벤트")]
    [SerializeField] private VoidEventChannel playerDeadEvent;
    [Header("보스 클리어 이벤트")]
    [SerializeField] private VoidEventChannel bossClearEvent;

    [Header("결과창 컨트롤러")]
    [SerializeField] private ResultUIController resultUIController;

    [Header("레벨업 시 띄울 오브젝트")]
    [SerializeField] private GameObject levelUpPanel;

    [Header("패널")]
    [SerializeField] private GameObject pausePanel;   // 일시정지 시 보여줄 패널
    [SerializeField] private GameObject optionPanel;  // 일시정지 메뉴 중 옵션을 선택했을 때 보여줄 패널
    [SerializeField] private GameObject gameoverPanel; // 게임오버 시 보여줄 패널
    [SerializeField] private CanvasGroup gameoverCG;

    [Header("일시정지")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button exitButton;

    [Header("게임종료")]
    [SerializeField] private TMP_Text gameoverText;
    [SerializeField] private Button goRestartButton;
    [SerializeField] private Button goTitleButton;

    [Header("옵션")]
    [SerializeField] private Button exitOptionButton;

    public GameSceneState gameSceneState = GameSceneState.Playing;
    private InputAction pauseAction;

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

        pauseAction = InputSystem.actions.FindAction("Pause");
    }

    private void Start()
    {
        playerDeadEvent.OnEventRaised += OnPlayerDead;
        bossClearEvent.OnEventRaised += OnBossClear;

        if (resumeButton != null)
            resumeButton.onClick.AddListener(() => OnClickResumeButton());
        if (restartButton != null)
            restartButton.onClick.AddListener(() => OnClickRestartButton());
        if (optionButton != null)
            optionButton.onClick.AddListener(() => OnClickOptionButton());
        if (exitButton != null)
            exitButton.onClick.AddListener(() => OnClickExitButton());

        if (goRestartButton != null)
            goRestartButton.onClick.AddListener(() => OnClickRestartButton());
        if (goTitleButton != null)
            goTitleButton.onClick.AddListener(() => OnClickExitButton());

        if (exitOptionButton != null)
            exitOptionButton.onClick.AddListener(()=> OnClickOptionExitButton());
    }

    private void OnDisable()
    {
        playerDeadEvent.OnEventRaised -= OnPlayerDead;
        bossClearEvent.OnEventRaised -= OnBossClear;
    }

    private void Update()
    {
        if (pauseAction.WasPressedThisFrame())
        {
            HandlePauseInput();
        }
    }

    // esc를 눌렀을 때 처리할 UI 작업
    private void HandlePauseInput()
    {
        switch (gameSceneState)
        {
            case GameSceneState.Playing:
                ShowPauseUI();
                break;

            case GameSceneState.PauseMenu:
                ClosePauseUI();
                break;

            case GameSceneState.OptionMenu:
                CloseOptionUI();
                break;

            case GameSceneState.LevelUp:
                break;
        }
    }

    #region 인게임 UI 조작
    private void ShowPauseUI()
    {
        optionPanel.SetActive(false);
        pausePanel.SetActive(true);
        gameSceneState = GameSceneState.PauseMenu;
        GameManager.Instance.PauseGame();
    }

    private void ClosePauseUI()
    {
        optionPanel.SetActive(false);
        pausePanel.SetActive(false);
        gameSceneState = GameSceneState.Playing;
        GameManager.Instance.ResumeGame();
    }

    private void ShowOptionUI()
    {
        optionPanel.SetActive(true);
        pausePanel.SetActive(false);
        gameSceneState = GameSceneState.OptionMenu;
    }

    private void CloseOptionUI()
    {
        optionPanel.SetActive(false);
        pausePanel.SetActive(true);
        gameSceneState = GameSceneState.PauseMenu;
    }

    private void ShowResultUI()
    {
        optionPanel.SetActive(false);
        pausePanel.SetActive(false);
        gameSceneState = GameSceneState.Result;
        resultUIController.ShowResult();
    }

    public void ShowLevelUpUI()
    {
        GameManager.Instance.PauseGame();
        gameSceneState = GameSceneState.LevelUp;
        levelUpPanel.SetActive(true);
    }

    public void CloseLevelUpUI()
    {
        levelUpPanel.SetActive(false);
        gameSceneState = GameSceneState.Playing;
        GameManager.Instance.ResumeGame();
    }
    #endregion

    #region 일시정지 UI 버튼 온클릭
    private void OnClickResumeButton()
    {
        ClosePauseUI();
    }

    private void OnClickRestartButton()
    {
        ClosePauseUI();

        // 해당 난이도로 재시작
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnClickOptionButton()
    {
        ShowOptionUI();
    }

    private void OnClickExitButton()
    {
        GameManager.Instance.ResumeGame();
        GameSceneManager.Instance.LoadScene(SceneType.Title);
    }
    #endregion

    #region 환경설정 메뉴
    private void OnClickOptionExitButton()
    {
        CloseOptionUI();
    }
    #endregion

    #region 이벤트 수신 시 실행할 메서드
    private void OnPlayerDead()
    {
        PoolManager.Instance.ReturnAllActiveObjects();

        gameoverPanel.SetActive(true);
        optionPanel.SetActive(false);
        pausePanel.SetActive(false);

        StartCoroutine(GameOverSequence());

        gameoverText.text = "YOU DIED";
        gameoverText.color = Color.red;
    }

    private void OnBossClear()
    {
        ShowResultUI();
    }

    private IEnumerator GameOverSequence()
    {
        Time.timeScale = 0.25f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        yield return new WaitForSecondsRealtime(0.8f);

        gameoverPanel.SetActive(true);
        gameoverCG.alpha = 0f;
        gameoverCG.DOFade(1f, 1.2f).SetUpdate(true);

        yield return new WaitForSecondsRealtime(1.2f);

        Time.timeScale = 0f;
    }
    #endregion
}
