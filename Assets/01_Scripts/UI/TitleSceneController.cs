using UnityEngine;
using UnityEngine.UI;

public class TitleSceneController : MonoBehaviour
{
    [Header("UI 그룹")]
    [SerializeField] private CanvasGroup titleCG;
    [SerializeField] private CanvasGroup optionCG;

    [Header("타이틀 버튼 참조")]
    [SerializeField] Button startButton;
    [SerializeField] Button optionButton;
    [SerializeField] Button exitButton;

    private void Start()
    {
        // 버튼 온클릭 리스너 추가
        if (startButton != null)
            startButton.onClick.AddListener(() => OnClickStartButton());
        if (optionButton != null)
            optionButton.onClick.AddListener(() => OnClickOptionButton());
        if (exitButton != null)
            exitButton.onClick.AddListener(() => OnClickExitButton());

        // 캔버스 그룹 초기화
        if (titleCG != null)
            CanvasGroupController.EnableCG(titleCG);
        if (optionCG != null)
            CanvasGroupController.DisableCG(optionCG);
    }

    public void OnClickStartButton()
    {
        GameSceneManager.Instance.LoadScene(SceneType.Game);
    }

    public void OnClickOptionButton()
    {
        // TODO: 옵션 UI 구성 후 활성화되도록 설정
        CanvasGroupController.DisableCG(titleCG);
        CanvasGroupController.EnableCG(optionCG);
    }

    public void OnClickExitButton()
    {
        Application.Quit();
    }
}
