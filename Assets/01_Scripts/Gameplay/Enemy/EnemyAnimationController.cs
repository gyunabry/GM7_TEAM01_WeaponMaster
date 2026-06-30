using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    private Animator animator;

    // 성능 최적화를 위해 문자열을 해시값으로 변환해 캐싱
    private readonly int isMovingHash = Animator.StringToHash("IsMove");
    private readonly int attackHash = Animator.StringToHash("Attack");
    private readonly int dieHash = Animator.StringToHash("Die");

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // 애니메이터의 런타임 애니메이터 컨트롤러를 교체
    public void SetupAnimator(RuntimeAnimatorController controller)
    {
        if (controller != null)
        {
            animator.runtimeAnimatorController = controller;

            animator.Rebind();
            ResetAllParameters();
        }
    }

    public void PlayMove(bool isMove)
    {
        animator.SetBool(isMovingHash, isMove);
    }

    public void TriggerAttack()
    {
        animator.SetTrigger(attackHash);
    }

    public void TriggerDead()
    {
        animator.SetTrigger(dieHash);
    }

    // 오브젝트 풀에서 꺼내어질 때 파라미터 초기화하는 메서드
    public void ResetAllParameters()
    {
        animator.SetBool(isMovingHash, false);
        animator.ResetTrigger(attackHash);
        animator.ResetTrigger(dieHash);
    }
}
