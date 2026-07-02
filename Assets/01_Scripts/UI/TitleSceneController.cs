using UnityEngine;
using UnityEngine.UI;

public class TitleSceneController : MonoBehaviour
{
    [Header("UI 그룹")]
    [SerializeField] private CanvasGroup titleCG;
    [SerializeField] private CanvasGroup optionCG;
    [SerializeField] private GameObject mapPanel;
    [SerializeField] private GameObject difficultPanel;

    [Header("타이틀 버튼 참조")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button exitButton;

    //[Header("옵션 버튼 참조")]
    //[SerializeField] private Button optionButton;

    [Header("맵 선택 UI 버튼")]
    [SerializeField] private Button mapCloseButton;

    [Header("난이도 선택 UI 버튼")]
    [SerializeField] private Button difficultCloseButton;

    private MapCardUI mapCardUI;
    private DifficultySelectUI difficultySelectUI;

    private void Start()
    {
        if (mapPanel != null)
        {
            mapCardUI = mapPanel.GetComponentInChildren<MapCardUI>(true);
        }

        if (difficultPanel != null)
        {
            difficultySelectUI = difficultPanel.GetComponentInChildren<DifficultySelectUI>(true);
        }

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
        if (mapCloseButton != null)
            mapCloseButton.onClick.AddListener(() => OnClickMapCloseButton());
        if (difficultCloseButton != null)
            difficultCloseButton.onClick.AddListener(() => OnClickDifficultCloseButton());
    }

    #region 타이틀 UI 버튼 액션
    public void OnClickStartButton()
    {
        // GameSceneManager.Instance.LoadScene(SceneType.Game);
        if (difficultySelectUI != null && difficultySelectUI.gameObject.activeInHierarchy)
        {
            difficultySelectUI.Close();
        }
        else if (difficultPanel != null)
        {
            difficultPanel.SetActive(false);
        }

        if (mapCardUI != null)
        {
            mapCardUI.Open();
        }
        else if (mapPanel != null)
        {
            mapPanel.SetActive(true);
        }

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
    public void OnClickMapCloseButton()
    {
        if (mapCardUI != null)
        {
            mapCardUI.Close();
        }
        else if (mapPanel != null)
        {
            mapPanel.SetActive(false);
        }

        CanvasGroupController.EnableCG(titleCG);
    }

    public void OnClickDifficultCloseButton()
    {
        if (difficultySelectUI != null)
        {
            difficultySelectUI.Close();
        }
        else if (difficultPanel != null)
        {
            difficultPanel.SetActive(false);
        }

        if (mapCardUI != null)
        {
            mapCardUI.Open();
        }
        else if (mapPanel != null)
        {
            mapPanel.SetActive(true);
        }
    }
    #endregion
}
