using System.Collections;
using UnityEngine;

public class EnemyVisualController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private EnemyAnimationController animationController;
    private Material originMaterial;

    private Coroutine flashRoutine;

    private float flashDuration = 0.1f;

    private WaitForSeconds flashWait;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animationController = GetComponent<EnemyAnimationController>();

        if (spriteRenderer != null)
        {
            originMaterial = spriteRenderer.material;
        }

        // WaitForSeconds 객체를 최초 1회만 생성
        flashWait = new WaitForSeconds(flashDuration);
    }

    // 적에게 런타임 애니메이터 컨트롤러 주입
    public void SetupAnimator(RuntimeAnimatorController controller)
    {
        animationController?.SetupAnimator(controller);
    }

    public void UpdateVisual(Transform target, bool isMoving)
    {
        FlipToTarget(target);
        PlayMove(isMoving);
    }

    public void StopAllVisuals()
    {
        PlayMove(false);

        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }

        RestoreMaterial();
    }

    // 몬스터에서 피격 시 호출
    public void PlayHitFlash(Material flashMaterial)
    {
        if (spriteRenderer == null || flashMaterial == null)
        {
            return;
        }

        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(FlashSpriteCo(flashMaterial));
    }

    // 피격 시 변경된 마테리얼을 복구
    public void RestoreMaterial()
    {
        if (spriteRenderer != null && originMaterial != null)
        {
            spriteRenderer.material = originMaterial;
        }
    }

    public void TriggerAttack()
    {
        animationController?.TriggerAttack();
    }

    public void TriggerDead()
    {
        animationController?.TriggerDead();
    }

    private void FlipToTarget(Transform target)
    {
        if (spriteRenderer == null || target == null)
        {
            return;
        }

        spriteRenderer.flipX = target.position.x > transform.position.x;
    }

    private void PlayMove(bool isMoving)
    {
        animationController?.PlayMove(isMoving);
    }

    private IEnumerator FlashSpriteCo(Material flashMaterial)
    {
        spriteRenderer.material = flashMaterial;

        yield return flashWait;

        RestoreMaterial();
        flashRoutine = null;
    }
}
