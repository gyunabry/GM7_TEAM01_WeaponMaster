using UnityEngine;

public interface ICollectable
{
    // 매개변수로 플레이어 스탯 받음
    void Collect(PlayerController player);

    // 플레이어의 획득 범위 안에 있을 때 끌려가는 로직
    // target : 끌려가는 대상
    // pullSpeed : 끌려가는 속도
    void Pull(Transform target, float pullSpeed);
}