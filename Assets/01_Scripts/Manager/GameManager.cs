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

    public void OnPlayerDead()
    {
        GameOver();
    }

    public void GameOver()
    {
        Debug.Log($"You died\n{KillCount} kills");
    }
}
