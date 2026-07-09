using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillCardUI : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    [Header("레벨업 UI")]
    [SerializeField] private LevelUpUI levelUpUI;
    [Header("카드 투명도")]
    [SerializeField] private CanvasGroup canvasGroup;
    [Header("등장 시작 위치")]
    [SerializeField] private float startYOffset = -80.0f;
    [Header("마우스 오버 크기")]
    [SerializeField] private float hoveScale = 1.09f;

    private RectTransform rectTransform;

    private Vector2 originalPosition;
    private Vector3 originalScale;
    private bool isClickable;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;

        originalScale = transform.localScale;
    }
    //카드를 숨기는 녀석
    public void HideInstant()
    {
        isClickable = false;
        //DOKill : 현재 오브젝트에서 실행중인 DoTween애니메이션을 즉시 종료하는 녀석
        transform.DOKill();
        canvasGroup.DOKill();
        rectTransform.DOKill();
        canvasGroup.alpha = 0.0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        //카드를 원래 위치보다 아래쪽으로 이동시킨다.
        rectTransform.anchoredPosition = originalPosition + new Vector2(0.0f, startYOffset);

        transform.localScale = Vector3.zero;
    }
    //카드 등장 애니메이션을 실행
    public void PlayOpenTween()
    {
        isClickable = true;//카드 등장하면 클릭 가능한 상태
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        Sequence sequence =  DOTween.Sequence();
       
        sequence.Join(canvasGroup.DOFade(1.0f, 0.25f));//투명도를 0.25초안 1로 만든다.
        sequence.Join(rectTransform.DOAnchorPos(originalPosition, 0.35f).SetEase(Ease.OutCubic));
        sequence.Join(transform.DOScale(originalScale, 0.35f).SetEase(Ease.OutBack));

    }
    //카드가 선택되었을때 실행할 녀석
    public Tween PlaySelectTween()
    {
        isClickable = false;
        transform.DOKill();

        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOScale(originalScale * 1.2f, 0.2f).SetEase(Ease.OutBack));
        sequence.Append(transform.DOScale(originalScale, 0.15f));

        return sequence;

    }
    //선택되지 않은 카드가 사라질때
    public Tween PlayHideTween()
    {
        isClickable = false;
        transform.DOKill();
        canvasGroup.DOKill();

        Sequence sequence = DOTween.Sequence();

        sequence.Join(canvasGroup.DOFade(0.0f, 0.2f));
        sequence.Join(transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack));
        return sequence;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isClickable) return;

        transform.DOKill();

        transform.DOScale(originalScale * hoveScale, 0.15f).SetEase(Ease.OutQuad);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isClickable) return;
        transform.DOKill();
        transform.DOScale(originalScale, 0.15f).SetEase(Ease.OutQuad);
    }

    //카드가 클릭되었을때 Button의 OnClick에서 호출할 녀석
    public void OnClickCard()
    {
        if (!isClickable) return;
        levelUpUI.SelectCard(this);
    }
}
