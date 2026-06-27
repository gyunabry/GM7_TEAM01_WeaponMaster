using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hp Event", menuName = "Game/Event/Hp Event Channel")]
public class HpEventChannel : ScriptableObject
{
    public event Action<float, float> OnEventRaised;

    public void RaiseEvent(float currentHp, float maxHp)
    {
        OnEventRaised?.Invoke(currentHp, maxHp);
    }

    private void OnDisable()
    {
        OnEventRaised = null;
    }
}
