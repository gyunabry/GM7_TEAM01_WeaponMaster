using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Void Event", menuName = "Game/Event/Void Event Channel")]
public class VoidEventChannel : ScriptableObject
{
    public event Action OnEventRaised;
    
    public void RaiseEvent()
    {
        OnEventRaised?.Invoke();
    }

    private void OnDisable()
    {
        OnEventRaised = null;
    }
}
