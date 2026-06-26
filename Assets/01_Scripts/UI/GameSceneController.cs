using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneController : MonoBehaviour
{
    [Header("플레이어 사망 이벤트")]
    [SerializeField] private VoidEventChannel playerDeadEvent;
    [Header("보스 사망 이벤트")]
    [SerializeField] private VoidEventChannel bossDeadEvent;

    [Header("패널")]
    [SerializeField] private CanvasGroup pauseCG;   // 일시정지 시 보여줄 패널
    [SerializeField] private CanvasGroup optionCG;  // 일시정지 메뉴 중 옵션을 선택했을 때 보여줄 패널
    [SerializeField] private CanvasGroup gameoverCG; // 게임오버 시 보여줄 패널

    [Header("일시정지")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button exitButton;

    [Header("게임종료")]
    [SerializeField] private TMP_Text gameoverText;
    [SerializeField] private Button goRestartButton;
    [SerializeField] private Button goTitleButton;

    private void Start()
    {
        playerDeadEvent.OnEventRaised += OnPlayerDead;
        bossDeadEvent.OnEventRaised += OnBossDead;

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
    }

    private void OnDisable()
    {
        playerDeadEvent.OnEventRaised -= OnPlayerDead;
        bossDeadEvent.OnEventRaised -= OnBossDead;
    }

    #region 일시정지 메뉴
    public void OnClickResumeButton()
    {
        GameManager.Instance.ResumeGame();
    }

    public void OnClickRestartButton()
    {
        // 해당 난이도로 재시작
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnClickOptionButton()
    {
        CanvasGroupController.EnableCG(optionCG);
        CanvasGroupController.DisableCG(pauseCG);
    }

    public void OnClickExitButton()
    {
        Debug.Log("타이틀로 돌아갑니다.");
        GameSceneManager.Instance.LoadScene(SceneType.Title);
    }
    #endregion

    private void OnPlayerDead()
    {
        CanvasGroupController.EnableCG(gameoverCG);
        CanvasGroupController.DisableCG(optionCG);
        CanvasGroupController.DisableCG(pauseCG);

        gameoverText.text = "YOU DIED";
        gameoverText.color = Color.red;
    }

    private void OnBossDead()
    {
        CanvasGroupController.EnableCG(gameoverCG);
        CanvasGroupController.DisableCG(optionCG);
        CanvasGroupController.DisableCG(pauseCG);

        gameoverText.text = "클리어!";
        gameoverText.color = Color.green;
    }
}
