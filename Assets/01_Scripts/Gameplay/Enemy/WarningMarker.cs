using System;
using UnityEngine;

public class WarningMarker : MonoBehaviour
{
    private float timer;
    private float duration;
    private Action onComplete;
    private bool isActive;

    public void PlayWarningEffect(float duration)
    {
        this.duration = duration;
        this.timer = 0f;
        this.isActive = true;
    }

    private void Update()
    {
        if (!isActive) return;

        timer += Time.deltaTime;

        if (timer >= duration)
        {
            isActive = false;

            // 유예 시간이 끝났을 때 
            onComplete?.Invoke();

            // 오브젝트 풀로 반환
            PoolManager.Instance.ReturnPool(this);
        }
    }
}
