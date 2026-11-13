using UnityEngine;

/*
- GameSceneData로부터 StageData를 가져와 해당하는 타일맵 프리팹을 생성
 */

public class StageManager : MonoBehaviour
{
    private void Start()
    {
        InitializeStage();
    }

    // GameSceneData에서 현재 스테이지 데이터를 가져와 게임씬 초기화
    public void InitializeStage()
    {
        StageData currentStage = GameSceneData.SelectedStage;
        if (currentStage == null) return;

        // 선택된 스테이지의 타일맵 생성
        GameObject tileMap = Instantiate(currentStage.mapPrefab, Vector3.zero, Quaternion.identity);
    }
}
