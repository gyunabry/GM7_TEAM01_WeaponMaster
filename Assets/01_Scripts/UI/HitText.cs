using DG.Tweening;
using TMPro;
using UnityEngine;

public class HitText : MonoBehaviour
{
    [Header("타격 텍스트 설정")]
    [SerializeField] private float floatHeight = 1.2f; // 떠오르는 높이
    [SerializeField] private float duration = 0.5f; // 애니메이션 지속시간

    [Header("색상 설정")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color critColor = Color.yellow;
    [SerializeField] private Color playerColor = Color.red;

    public TMP_Text damageText;

    private Color textColor;
    private Vector3 baseScale;

    private void Awake()
    {
        // 처음 시작 시 원래 크기를 저장
        baseScale = transform.localScale;
    }

    private void OnEnable()
    {
        // 페이드인 효과를 주기 위해 활성화 시 알파값을 0.5로 설정
        damageText.alpha = 0.5f;
    }

    public void ShowDamage(float damage, Vector2 spawnPos, bool isCrit = false, bool isPlayer = false)
    {
        // 오브젝트 풀링 적용을 위해 기본 값으로 초기화
        transform.position = spawnPos;
        transform.localScale = baseScale;

        // 플레이어라면 빨간색
        // 아니라면 치명타 여부 확인 후 색 설정
        if (isPlayer) textColor = playerColor;
        else textColor = isCrit ? critColor : normalColor;

        // 데미지 표시
        damageText.text = damage.ToString();
        damageText.color = textColor;

        // DOTween 시퀀스 생성
        Sequence sequence = DOTween.Sequence();

        // 상승 효과
        sequence.Join(transform.DOMoveY(transform.position.y + floatHeight, duration).SetEase(Ease.OutCirc));
        // 펀치 효과
        sequence.Join(transform.DOPunchScale(Vector3.one * 0.5f, duration, 1, 0.5f));
        // 페이드아웃
        sequence.Join(damageText.DOFade(0f, duration).SetEase(Ease.InExpo));

        // 애니메이션 종료 후 풀에 반환
        sequence.OnComplete(() =>
        {
            ReturnToPool();
        });
    }

    private void ReturnToPool()
    {
        PoolManager.Instance.ReturnPool(this);
    }
}
