using UnityEngine;
using DG.Tweening;

public class DifficultySelectUI : MonoBehaviour, ICardPanel
{
    [Header("패널")]
    [SerializeField] private GameObject difficultySelectPanel;
    [Header("투명도")]
    [SerializeField] private CanvasGroup panelCavasGroup;
    [Header("난이도 선택")]
    [SerializeField] private SelectMapCardUI[] difficultyCards;
   

    private bool isOpen;
    private bool isSelected;

    private void Awake()
    {
        for (int i = 0; i < difficultyCards.Length; i++)
        {
            if (difficultyCards[i] != null)
            {
                difficultyCards[i].InitPanel(this);
            }
        }
    }

    void Start()
    {
        if (isOpen) return;
        CloseInstant();
    }

    public void Open()
    {
        if (isOpen) return;
        isOpen = true;
        isSelected = false;
        difficultySelectPanel.SetActive(true);

       
        panelCavasGroup.alpha = 0.0f;
        panelCavasGroup.blocksRaycasts = true;
        panelCavasGroup.interactable = false;

        panelCavasGroup.DOFade(1.0f, 0.25f).OnComplete(() =>
        {
            panelCavasGroup.interactable = true;
            PlayCardOpenTween(); 
        });
    }

    private void PlayCardOpenTween()
    {
        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < difficultyCards.Length; i++)
        {
            int index = i;
            sequence.AppendCallback(() =>
            {
                if (difficultyCards[index] != null)
                {
                    difficultyCards[index].PlayOpenTween(); 
                }
            });
            sequence.AppendInterval(0.12f);
        }
    }

    public void SelectCard(SelectMapCardUI selectCard)
    {
        if (isSelected) return;
        isSelected = true;
        panelCavasGroup.interactable = false;

        Sequence sequence = DOTween.Sequence();
        int chosenDifficultyIndex = 0;

        for (int i = 0; i < difficultyCards.Length; i++)
        {
            SelectMapCardUI card = difficultyCards[i];
            if (card == selectCard)
            {
                sequence.Join(card.PlaySelectTween());
                chosenDifficultyIndex = i;
            }
            else if (card != null)
            {
                sequence.Join(card.PlayHideTween());
            }
        }

        sequence.OnComplete(() =>
        {
            GameSceneData.SelectedDifficulty = (Difficulty)chosenDifficultyIndex;
            // GameSceneManager를 통해 씬 타입으로 씬 로드
            GameSceneManager.Instance.LoadScene(SceneType.Game);
        });
    }

    private void CloseInstant()
    {
        isOpen = false;
        isSelected = false;
        difficultySelectPanel.SetActive(false);
        panelCavasGroup.alpha = 0.0f;
        panelCavasGroup.blocksRaycasts = false;
        panelCavasGroup.interactable = false;
    }
}