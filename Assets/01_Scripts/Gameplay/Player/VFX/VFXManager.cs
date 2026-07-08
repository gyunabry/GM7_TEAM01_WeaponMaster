using System;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    SpriteRenderer sprite;
    Animator anima;
    AudioSource audio;
    [SerializeField] VFXAdd vfxPrefab;
    [SerializeField] private List<VFXClass> vfxList = new List<VFXClass>();

    public void SpawnEffect(Transform trans, PlayerWeaponSO.WeaponType type)
    {
        VFXAdd effect = PoolManager.Instance.GetPool(vfxPrefab);
        sprite = effect.GetComponent<SpriteRenderer>();
        anima = effect.GetComponent<Animator>();
        audio = effect.GetComponent<AudioSource>();
        foreach(VFXClass vfx in vfxList)
        {
            if(type == vfx.hitType)
            {
                sprite.sprite = vfx.hitSprite;
                anima.runtimeAnimatorController = vfx.hitAnima;
                audio.clip = vfx.hitAudio;
            }
        }
        effect.transform.position = trans.position;
        audio.Play();
    }
}
[Serializable]
public class VFXClass
{
    public Sprite hitSprite;
    public RuntimeAnimatorController hitAnima;
    public AudioClip hitAudio;
    public PlayerWeaponSO.WeaponType hitType;
}