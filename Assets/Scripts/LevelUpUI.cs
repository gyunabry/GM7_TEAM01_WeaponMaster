using DG.Tweening;
using UnityEngine;

public class LevelUpUI : MonoBehaviour
{
    [Header("패널")]
    //레벨업 UI전체 패널오브젝트
    [SerializeField] private GameObject levelUpPanel;
    [Header("투명도")]
    [SerializeField] private CanvasGroup panleCanvasGroup;
    [Header("타이틀")]
    [SerializeField] private RectTransform levelUpText;
    [Header("스킬카드")] 
    [SerializeField] private SkillCardUI[] skillCards;

    //패널이 열려있냐?
    private bool isOpen;

    //카드 선택용
    private bool isSelected;
    void Start()
    {
        CloseInstant();
    }
    public void Open()
    {
        if (isOpen) return;//이미 패널이 열려 있으면 다시 열지 말자.

        isOpen = true;//열린상태로 변경
        isSelected = false;//아직 어떤 카드도 선택하지 않은 상태임.
        levelUpPanel.SetActive(true);//레벨업 패널 오브젝트 활성화
        panleCanvasGroup.alpha = 0.0f;
        panleCanvasGroup.blocksRaycasts = true;
        panleCanvasGroup.interactable = true;

        //패널 전체 투명도를 0.25초동안 1로 만든다.
        //화면이 자연스럽게 나타나는 페이드 인 효과임.
        panleCanvasGroup.DOFade(1.0f, 0.25f);

        //레벨업 타이틀 등장 애니메이션 
        PlayTitleTween();
        //스킬 카드 3개 등작 애니메이션
        PlayCardOpenTween();

    }
    private void PlayTitleTween()
    {
        levelUpText.localScale = Vector3.zero;
        //타이틀 크기를 0.4초 동안 원래 크기로 키운다.
        //Ease.OutBack는 살짝 튕기듯 커지는 느낌을 준다.
        levelUpText.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
    }
    private void PlayCardOpenTween()
    {
        //우선 모든 카드를 먼저 숨기자.

        for(int i = 0;i<skillCards.Length;i++)
        {
            skillCards[i].HideInstant();
        }

        //카드 등장 순서를 제어하기 위한 시퀀스를 만든다.
        //시퀀스를 사용하면 여러 트윈이나 콜백을 순서대로 실행할수 있음
        Sequence sequence =  DOTween.Sequence();

        for (int i = 0; i < skillCards.Length; i++)
        {
            int index = i;

            //현재 순서의 카드 등장 애니메이션을 실행한다.
            //AppendCallback 트윈은 아니지만 시퀀스 중간에 함수를 실행할수 있게 해준다.
            sequence.AppendCallback(() =>
            {
                skillCards[index].PlayOpenTween();
            });

            //다음 카드가 등장하기 전까지 0.12초 기다린다.
            //이 간격때문에 카드가 하나씩 순서대로 등장하는 느낌을 볼수 있음
            sequence.AppendInterval(0.12f);

        }

    }
    //카드가 클릭되었을때
    public void SelectCard(SkillCardUI selectCard)
    {
        if (isSelected) return;
        isSelected = true;
        Sequence sequence =  DOTween.Sequence();

        for(int i = 0;i<skillCards.Length;i++)
        {
            SkillCardUI card = skillCards[i];//배열에서 현재 카드를 가져오고

            if(card==selectCard)
            {
                sequence.Join(card.PlaySelectTween());
            }
            else
            {
                sequence.Join(card.PlayHideTween());
            }
        }
        sequence.AppendInterval(0.5f);

        sequence.OnComplete(() =>
        {
            Close();
        });
    }

    public void Close()
    {
        panleCanvasGroup.DOFade(0.0f, 0.25f).OnComplete(() =>
        {
            CloseInstant();
        });
    }
    private void CloseInstant()
    {
        isOpen = false;//패널이 닫힌상태
        isSelected = false;//카드 선택 상태 초기화
        levelUpPanel.SetActive(false);//오브젝트 비활성화
        panleCanvasGroup.alpha = 0.0f;//패널 투명도를 0으로
        panleCanvasGroup.blocksRaycasts = false;//패널이 마우스 입력을 받지 않게 하자.
        panleCanvasGroup.interactable = false;//패널안의 UI 상호작용을 막자.


        //여기서 카드도 숨긴상태로 초기화하자.

    }
}
