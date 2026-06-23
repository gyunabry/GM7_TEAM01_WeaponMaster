using UnityEngine;
using UnityEngine.UI;

public class TitleSceneController : MonoBehaviour
{
    [Header("UI 그룹")]
    [SerializeField] private CanvasGroup titleCG;
    [SerializeField] private CanvasGroup optionCG;
    [SerializeField] private GameObject selectCanvas;

    [Header("타이틀 버튼 참조")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button exitButton;

    //[Header("옵션 버튼 참조")]
    //[SerializeField] private Button optionButton;

    [Header("맵 선택 UI 버튼 참조")]
    [SerializeField] private Button closeButton;

    private void Start()
    {
        // 캔버스 그룹 초기화
        if (titleCG != null)
            CanvasGroupController.EnableCG(titleCG);
        if (optionCG != null)
            CanvasGroupController.DisableCG(optionCG);

        // 타이틀 UI 버튼
        if (startButton != null)
            startButton.onClick.AddListener(() => OnClickStartButton());
        if (optionButton != null)
            optionButton.onClick.AddListener(() => OnClickOptionButton());
        if (exitButton != null)
            exitButton.onClick.AddListener(() => OnClickExitButton());
        
        // 맵 선택 UI 버튼
        if (closeButton != null)
            closeButton.onClick.AddListener(() => OnClickCloseButton()); 
    }

    #region 타이틀 UI 버튼 액션
    public void OnClickStartButton()
    {
        // GameSceneManager.Instance.LoadScene(SceneType.Game);
        selectCanvas.SetActive(true);
        CanvasGroupController.DisableCG(titleCG);
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
    #endregion

    #region 맵 선택 UI 버튼 액션
    public void OnClickCloseButton()
    {
        selectCanvas?.SetActive(false);
        CanvasGroupController.EnableCG(titleCG);
    }
    #endregion
}
