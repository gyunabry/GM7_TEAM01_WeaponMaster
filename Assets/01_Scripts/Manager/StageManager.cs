using NavMeshPlus.Components;
using Unity.AI.Navigation;
using UnityEngine;

/*
- GameSceneData로부터 StageData를 가져와 해당하는 타일맵 프리팹을 생성
- NavMesh를 구워 Agent가 움직일 수 있게끔 설정
 */

public class StageManager : MonoBehaviour
{
    [Header("NavMesh 참조")]
    [SerializeField] private NavMeshPlus.Components.NavMeshSurface navMeshSurface;

    private void Start()
    {
        InitializeStage();
    }

    // GameSceneData에서 현재 스테이지 데이터를 가져와 게임씬 초기화
    public void InitializeStage()
    {
        // 기존에 있던 NavMesh 데이터 삭제 
        navMeshSurface.RemoveData();

        StageData currentStage = GameSceneData.SelectedStage;
        if (currentStage == null) return;

        // 선택된 스테이지의 타일맵 생성
        GameObject tileMap = Instantiate(currentStage.mapPrefab, Vector3.zero, Quaternion.identity);

        // NavMesh bake
        navMeshSurface.BuildNavMesh();

        
    }
}
