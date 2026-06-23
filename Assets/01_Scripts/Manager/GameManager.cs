using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("구독할 이벤트")]
    [SerializeField] private VoidEventChannel playerDeadEvent;

    public int KillCount { get; private set; }
    public int Gold { get; private set; }

    public event Action<int> OnKillEnemy;
    public event Action<int> OnGoldChanged;
    public event Action<int, int> OnExpChanged;

    // 레벨
    private int level;
    private int[] requireExp = { 0, 25, 50, 75, 100, 125, 150, 175, 200 };
    private int currentExp;

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
    }

    private void Start()
    {
        level = 0;
        currentExp = 0;
        KillCount = 0;
        Gold = 0;
    }

    private void OnEnable()
    {
        if (playerDeadEvent != null)
        {
            playerDeadEvent.OnEventRaised += OnPlayerDead;
        }
    }

    private void OnDisable()
    {
        if (playerDeadEvent != null)
        {
            playerDeadEvent.OnEventRaised -= OnPlayerDead;
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

        if (currentExp >= requireExp[level])
        {
            // 레벨업 시 현재 경험치를 필요 경험치만큼 삭감
            currentExp -= requireExp[level];
            level++;
            // TODO:레벨업 효과 이벤트
        }
    }

    public void OnPlayerDead()
    {
        GameOver();
    }

    public void GameOver()
    {
        Debug.Log($"You died\n{KillCount} kills");
    }
}
