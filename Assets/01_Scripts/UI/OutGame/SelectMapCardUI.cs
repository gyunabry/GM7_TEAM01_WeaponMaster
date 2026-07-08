using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectMapCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler, ISubmitHandler
{
    [Header("카드 투명도")]
    [SerializeField] private CanvasGroup canvasGroup;
    [Header("등장 시작 위치")]
    [SerializeField] private float startOffset = -80.0f;
    [Header("마우스 호버 스케일")]
    [SerializeField] private float hoveScale = 1.09f;
    [Header("이 카드가 담을 스테이지 정보")]
    [SerializeField] private StageData stageData;
    [Header("외곽선 메테리얼")]
    [SerializeField] private Material mate;

    public StageData MapStageData => stageData;


    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Vector3 originalScale;

    private bool isClickable = false;
    private ICardPanel targetPanel;
    private bool hasCachedOriginalValues;

    private Image[] childUi;
    

    private void Awake()
    {
        CacheReferences();
        //targetPanel = GetComponentInParent<ICardPanel>();
        HideInstant();
    }

    private void CacheReferences()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        if (childUi == null || childUi.Length == 0)
        {
            childUi = GetComponentsInChildren<Image>();
        }

        if (!hasCachedOriginalValues && rectTransform != null)
        {
            originalPosition = rectTransform.anchoredPosition;
            originalScale = transform.localScale;
            hasCachedOriginalValues = true;
        }
    }

    public void InitPanel(ICardPanel panel)
    {
        targetPanel = panel;
    }

    public void HideInstant()
    {
        CacheReferences();

        if (childUi != null && childUi.Length > 1)
        {
            childUi[1].gameObject.SetActive(false);
        }

        isClickable = false;
        transform.DOKill();
        if (canvasGroup != null)
        {
            canvasGroup.DOKill();
            canvasGroup.alpha = 0.0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
        if (rectTransform != null)
        {
            rectTransform.DOKill();
            rectTransform.anchoredPosition = originalPosition + new Vector2(0.0f, startOffset);
        }
        transform.localScale = Vector3.zero;
    }

    public void PlayOpenTween()
    {
        isClickable = true;
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
            canvasGroup.alpha = 1.0f;
        }
        DG.Tweening.Sequence sequence = DOTween.Sequence();
        if (rectTransform != null) sequence.Join(rectTransform.DOAnchorPos(originalPosition, 0.35f).SetEase(Ease.OutCubic));
        sequence.Join(transform.DOScale(originalScale, 0.35f).SetEase(Ease.OutBack));
    }

    public Tween PlaySelectTween()
    {
        isClickable = false;
        transform.DOKill();
        transform.DOScale(originalScale, 0.01f);
        return transform.DOPunchScale(originalScale * 0.15f, 0.25f, 5, 0.5f);
    }

    public Tween PlayHideTween()
    {
        isClickable = false;
        transform.DOKill();
        if (canvasGroup != null) canvasGroup.DOKill();
        DG.Tweening.Sequence sequence = DOTween.Sequence();
        if (canvasGroup != null) sequence.Join(canvasGroup.DOFade(0.0f, 0.2f));
        sequence.Join(transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack));
        return sequence;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isClickable == false) return;
        childUi[1].gameObject.SetActive(true);
        transform.SetAsLastSibling();
        childUi[0].material = mate;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isClickable == false) return;
        childUi[1].gameObject.SetActive(false);
        transform.DOKill();
        childUi[0].material = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isClickable == false) return;
        if (targetPanel != null)
        {
            targetPanel.SelectCard(this);
        }
    }
    public void OnSelect(BaseEventData eventData)
    {
        if (isClickable == false) return;
        childUi[1].gameObject.SetActive(true);
        transform.SetAsLastSibling();
        childUi[0].material = mate;
    }
    public void OnDeselect(BaseEventData eventData)
    {
        if (isClickable == false) return;
        childUi[1].gameObject.SetActive(false);
        transform.DOKill();
        childUi[0].material = null;
    }
    public void OnSubmit(BaseEventData eventData)
    {
        if (isClickable == false) return;
        if (targetPanel != null)
        {
            targetPanel.SelectCard(this);
        }
    }

    public void ForceClickForButton()
    {
        if (isClickable == false) return;
        if (targetPanel != null) targetPanel.SelectCard(this);
    }
}
