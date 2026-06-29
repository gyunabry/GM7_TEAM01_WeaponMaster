using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Audio;

public class VFXManager : MonoBehaviour
{
    SpriteRenderer sprite;
    Animator anima;
    AudioSource audio;
    [SerializeField] VFXAdd vfxPrefab;
    [SerializeField] Sprite HitEffect;
    [SerializeField] Sprite ArrowHitEffect;

    [SerializeField] AnimatorController HitAnima;
    [SerializeField] AnimatorController ArrowHitAnima;

    [SerializeField] AudioClip HitAudio;
    [SerializeField] AudioClip ArrowHitAudio;

    public void SpawnEffect(Transform trans, bool arrow)
    {
        VFXAdd effect = PoolManager.Instance.GetPool(vfxPrefab);
        sprite = effect.GetComponent<SpriteRenderer>();
        anima = effect.GetComponent<Animator>();
        audio = effect.GetComponent<AudioSource>();
        if (arrow == true)
        {
            sprite.sprite = ArrowHitEffect;
            anima.runtimeAnimatorController = ArrowHitAnima;
            audio.clip = ArrowHitAudio;
        }
        else
        {
            sprite.sprite = HitEffect;
            anima.runtimeAnimatorController = HitAnima;
            audio.clip = HitAudio;
        }
        effect.transform.position = trans.position;
        audio.Play();
    }
}
