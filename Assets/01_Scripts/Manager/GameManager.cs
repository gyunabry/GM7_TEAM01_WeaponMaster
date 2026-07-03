using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    // 현재 메뉴 상태를 한 번에 관리하기 위해 열거형으로 선언
    private enum PauseMenuState
    {
        Playing,
        PauseMenu,
        OptionMenu,
        GameOver,
        LevelUp,
        Result
    }

    public static GameManager Instance { get; private set; }

    [Header("구독할 이벤트")]
    [SerializeField] private VoidEventChannel playerDeadEvent;
    [SerializeField] private VoidEventChannel bossClearEvent;

    [Header("레벨업 시 띄울 오브젝트")]
    [SerializeField] private GameObject levelUpButton;

    public int KillCount { get; private set; }
    public int Gold { get; private set; }

    public event Action<int> OnKillEnemy;
    public event Action<int> OnGoldChanged;
    public event Action<int, int> OnExpChanged;

    // 레벨
    private int level;
    private int[] requireExp = new int[100];
    private int currentExp;

    private InputAction pauseAction;

    private PauseMenuState pauseMenuState = PauseMenuState.Playing;

    public int Level => level;
    public int CurrentExp => currentExp;
    public int[] RequireExp => requireExp;

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

        pauseAction = InputSystem.actions.FindAction("Pause");
    }

    private void Start()
    {
        level = 0;
        currentExp = 0;
        KillCount = 0;
        Gold = 0;
        SetNeedExp();
    }

    private void OnEnable()
    {
        if (playerDeadEvent != null)
        {
            playerDeadEvent.OnEventRaised += OnPlayerDead;
        }

        if (bossClearEvent != null)
        {
            bossClearEvent.OnEventRaised += OnBossDead;
        }
    }

    private void OnDisable()
    {
        if (playerDeadEvent != null)
        {
            playerDeadEvent.OnEventRaised -= OnPlayerDead;
        }

        if (bossClearEvent != null)
        {
            bossClearEvent.OnEventRaised -= OnBossDead;
        }
    }

    private void Update()
    {
        if (pauseAction.WasPressedThisFrame())
        {
            HandlePauseInput();
        }
    }

    private void HandlePauseInput()
    {
        switch (pauseMenuState)
        {
            case PauseMenuState.Playing:
                OpenPauseMenu();
                break;

            case PauseMenuState.PauseMenu:
                ResumeFromPause();
                break;

            case PauseMenuState.OptionMenu:
                CloseOptionToPause();
                break;
        }
    }

    public void OpenPauseMenu()
    {
        PauseGame();
        pauseMenuState = PauseMenuState.PauseMenu;
        GameSceneController.Instance.ShowPauseUI();
    }

    public void ResumeFromPause()
    {
        pauseMenuState = PauseMenuState.Playing;
        GameSceneController.Instance.ClosePauseUI();
        ResumeGame();
    }
    
    public void OpenOptionMenu()
    {
        pauseMenuState = PauseMenuState.OptionMenu;
        GameSceneController.Instance.ShowOptionUI();
    }

    public void CloseOptionToPause()
    {
        pauseMenuState = PauseMenuState.PauseMenu;
        GameSceneController.Instance.CloseOptionUI();
    }

    public void OpenResultUI()
    {
        pauseMenuState = PauseMenuState.Result;
    }

    public void SetNeedExp()
    {
        for (int i = 0; i < requireExp.Length; i++) 
        { 
            if(i == 0)
            {
                requireExp[i] = 20;
            }
            else if(i == 1)
            {
                requireExp[i] = 50;
            }
            else if (i <= 20)
            {
                requireExp[i] = 60 + (i - 1) * 60;
            }
            else if (i <= 40)
            {
                requireExp[i] = 2000 + (i - 20) * 70;
            }
            else if (i <= 60)
            {
                requireExp[i] = 4500 + (i - 40) * 80;
            }
            else if (i <= 80)
            {
                requireExp[i] = 8000 + (i - 60) * 90;
            }
            else if (i <= 100)
            {
                requireExp[i] = 13000 + (i - 80) * 100;
            }
        }
    }

    public void AddKillCount()
    {
        KillCount++;
        OnKillEnemy?.Invoke(KillCount);
    }

    public int GetKillCount()
    {
        return KillCount;
    }

    public void AddGold()
    {
        Gold++;
        OnGoldChanged?.Invoke(Gold);
    }

    public void AddExp(int amount)
    {
        currentExp += amount;
        CheckLevelUp();
        OnExpChanged?.Invoke(currentExp, requireExp[level]);
    }

    public void CheckLevelUp()
    {
        if (currentExp < 0) return;

        while (currentExp >= requireExp[level])
        {
            PauseGame();

            // 최고레벨 도달 시 더이상 레벨업 X
            if (level >= requireExp.Length)
            {
                // 현재, 최대 EXP를 레벨업에 필요한 수치로 전달해 갱신
                OnExpChanged?.Invoke(requireExp[level], requireExp[level]);
                level = requireExp.Length - 1;
                break;
            }

            // 레벨업 시 현재 경험치를 필요 경험치만큼 삭감
            currentExp -= requireExp[level];
            level++;
            levelUpButton.SetActive(true);

            // TODO:레벨업 효과 이벤트
        }
    }

    public void OnPlayerDead()
    {
        GameOver();
    }

    public void GameOver()
    {
        PauseGame();
    }

    public void OnBossDead()
    {
        StageClear();
    }

    public void StageClear()
    {
        PauseGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0.0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
    }
}
