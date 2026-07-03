using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneController : MonoBehaviour
{
    public static GameSceneController Instance { get; private set; }

    [Header("플레이어 사망 이벤트")]
    [SerializeField] private VoidEventChannel playerDeadEvent;
    [Header("보스 클리어 이벤트")]
    [SerializeField] private VoidEventChannel bossClearEvent;

    [Header("결과창 컨트롤러")]
    [SerializeField] private ResultUIController resultUIController;

    [Header("패널")]
    [SerializeField] private GameObject pausePanel;   // 일시정지 시 보여줄 패널
    [SerializeField] private GameObject optionPanel;  // 일시정지 메뉴 중 옵션을 선택했을 때 보여줄 패널
    [SerializeField] private GameObject gameoverPanel; // 게임오버 시 보여줄 패널

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

    #region 일시정지 메뉴
    public void ShowPauseUI()
    {
        optionPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void ClosePauseUI()
    {
        optionPanel.SetActive(false);
        pausePanel.SetActive(false);
    }

    public void ShowOptionUI()
    {
        optionPanel.SetActive(true);
        pausePanel.SetActive(false);
    }

    public void CloseOptionUI()
    {
        optionPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    private void OnClickResumeButton()
    {
        GameManager.Instance.ResumeFromPause();
    }

    private void OnClickRestartButton()
    {
        ClosePauseUI();

        // 해당 난이도로 재시작
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnClickOptionButton()
    {
        GameManager.Instance.OpenOptionMenu();
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
        GameManager.Instance.CloseOptionToPause();
    }
    #endregion

    private void OnPlayerDead()
    {
        gameoverPanel.SetActive(true);
        optionPanel.SetActive(false);
        pausePanel.SetActive(false);

        gameoverText.text = "YOU DIED";
        gameoverText.color = Color.red;
    }

    private void OnBossClear()
    {
        optionPanel.SetActive(false);
        pausePanel.SetActive(false);

        //gameoverText.text = "클리어!";
        //gameoverText.color = Color.green;

        GameManager.Instance.OpenResultUI();
        resultUIController.ShowResult();
    }
}
