using System.Collections.Generic;
using UnityEngine;

/*
- 월드를 일정한 크기의 셀로 이루어진 격자로 나눈다. 
- 몬스터의 현재 위치를 변환해 저장한다.
- 해당 몬스터가 위치한 셀과 인접한 셀 안의 몬스터와의 충돌을 검사한다.
*/

public class SpatialHashGrid : MonoBehaviour
{
    public static SpatialHashGrid Instance { get; private set; }

    [SerializeField] private float cellSize = 1.5f;

    // 각 좌표에 어떤 적들이 있는지 저장 - 주변 적 갱신용
    private readonly Dictionary<Vector2Int, List<EnemyController>> cells = new();
    // 각 적이 어느 좌표에 있는지 저장 - 상태 갱신용
    private readonly Dictionary<EnemyController, Vector2Int> enemyCells = new();

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

    // 유닛이 생성될 때 호출해 등록
    public void Register(EnemyController enemy)
    {
        Vector2Int cell = WorldToCell(enemy.transform.position);
        enemyCells[enemy] = cell;

        
        if (!cells.TryGetValue(cell, out var list))
        {
            list = new List<EnemyController>();
            cells[cell] = list;
        }

        list.Add(enemy);
    }

    // 유닛이 죽었을 때 호출해 제거
    public void Unregister(EnemyController enemy)
    {
        if (!enemyCells.TryGetValue(enemy, out var cell)) return;

        if (cells.TryGetValue(cell, out var list))
        {
            list.Remove(enemy);
        }

        enemyCells.Remove(enemy);
    }

    // 유닛이 이동해 해당 셀을 벗어났을 때 호출해 갱신
    public void UpdateEnemyCell(EnemyController enemy)
    {
        Vector2Int newCell = WorldToCell(enemy.transform.position);

        if (enemyCells.TryGetValue(enemy, out var preCell) && preCell == newCell)
        {
            return;
        }

        Unregister(enemy);
        Register(enemy);
    }

    public Vector2 GetSeparationForce(EnemyController enemy, float separationRadius)
    {
        Vector2 push = Vector2.zero;
        Vector2 enemyPos = enemy.transform.position;
        Vector2Int centerCell = WorldToCell(enemyPos);
        float radiusSqr = separationRadius * separationRadius;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2Int cell = centerCell + new Vector2Int(x, y);

                if (!cells.TryGetValue(cell, out var list)) continue;

                foreach (EnemyController other in list)
                {
                    if (other == enemy || other.IsDead) continue;
                    
                    Vector2 diff = enemyPos - (Vector2)other.transform.position;
                    float sqrDistance = diff.sqrMagnitude;

                    if (sqrDistance > 0.001f && sqrDistance < radiusSqr)
                    {
                        // 가까울 수록 강하게 밀도록 설정
                        push += diff.normalized / sqrDistance;
                    }
                }
            }
        }

        return push;
    }

    // 적 위치를 셀 사이즈로 나눠 좌표값 반환
    private Vector2Int WorldToCell(Vector2 position)
    {
        return new Vector2Int(
            Mathf.FloorToInt(position.x / cellSize),
            Mathf.FloorToInt(position.y / cellSize)
        );
    }
}
