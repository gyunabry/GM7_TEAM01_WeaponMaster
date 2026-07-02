using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapCardUI : MonoBehaviour, ICardPanel
{
    [Header("패널")]
    [SerializeField] private GameObject mapSelectPanel;
    [Header("투명도")]
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [Header("맵 카드")]
    [SerializeField] private SelectMapCardUI[] mapCards;

    [Header("다음 단계 UI")]
    [SerializeField] private DifficultySelectUI difficultySelectUI;

    [Header("이 맵의 웨이브 데이터들")]
    [SerializeField] private List<WaveData> mapWaveDataList;
    [SerializeField] private GameObject selectButton;

    private bool isOpen;
    private bool isSelected;
    private Coroutine co;

    private void Awake()
    {
        for (int i = 0; i < mapCards.Length; i++)
        {
            if (mapCards[i] != null)
            {
                mapCards[i].InitPanel(this);
            }
        }
    }

    private void OnEnable()
    {
        if (!isOpen)
        {
            Open();
        }

        co = StartCoroutine(FirstSelectCard());
    }

    private void OnDisable()
    {
        ResetPanelState();
    }

    public void Open()
    {
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
            return;
        }

        if (isOpen)
        {
            RestoreSelectionState();
            return;
        }
        isOpen = true;
        isSelected = false;
        DOTween.Kill(this);
        ResetCards();
        mapSelectPanel.SetActive(true);

        panelCanvasGroup.alpha = 0.0f;
        panelCanvasGroup.blocksRaycasts = true;
        panelCanvasGroup.interactable = true;

        
        panelCanvasGroup.DOFade(1.0f, 0.25f).SetTarget(this).OnComplete(() =>
        {
            panelCanvasGroup.interactable = true;
            PlayCardOpenTween();
        });
    }

    IEnumerator FirstSelectCard()
    {
        yield return null;
        if (EventSystem.current != null && selectButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(selectButton);
        }
    }

    private void PlayCardOpenTween()
    {
        for (int i = 0; i < mapCards.Length; i++)
        {
            if (mapCards[i] != null)
            {
                
                float delay = i * 0.12f;
                int index = i;

                DOVirtual.DelayedCall(delay, () => {
                    if (mapCards[index] != null)
                    {
                        mapCards[index].PlayOpenTween();
                    }
                }, false).SetTarget(this);
            }
        }
    }

    public void SelectCard(SelectMapCardUI selectCard)
    {
       
        if (isSelected) return;
        isSelected = true;
        panelCanvasGroup.interactable = false;
        panelCanvasGroup.blocksRaycasts = false;

        selectCard.PlaySelectTween();

        DOVirtual.DelayedCall(0.26f, () =>
        {
            // GameSceneData에 선택된 스테이지의 데이터 저장
            GameSceneData.SelectedStage = selectCard.MapStageData;

            // CloseInstant();

            if (difficultySelectUI != null)
            {
               
                difficultySelectUI.Open();
            }
            
        }, false).SetTarget(this);
    }
    public void SelectCard(DeSelectCardUI selectCard)
    {

    }

    public void Close()
    {
        CloseInstant();
    }

    private void CloseInstant()
    {
        ResetPanelState();
        mapSelectPanel.SetActive(false);
    }

    private void ResetPanelState()
    {
        isOpen = false;
        isSelected = false;
        DOTween.Kill(this);
        panelCanvasGroup.alpha = 0.0f;
        panelCanvasGroup.blocksRaycasts = false;
        panelCanvasGroup.interactable = false;
        ResetCards();

        if (co != null)
        {
            StopCoroutine(co);
            co = null;
        }
    }

    private void RestoreSelectionState()
    {
        isSelected = false;
        DOTween.Kill(this);
        panelCanvasGroup.alpha = 1.0f;
        panelCanvasGroup.blocksRaycasts = true;
        panelCanvasGroup.interactable = true;
        ResetCards();
        PlayCardOpenTween();
    }

    private void ResetCards()
    {
        for (int i = 0; i < mapCards.Length; i++)
        {
            if (mapCards[i] != null)
            {
                mapCards[i].HideInstant();
            }
        }
    }
}
