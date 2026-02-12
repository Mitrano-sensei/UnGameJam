using System;
using System.Linq;
using EditorAttributes;
using UnityEngine;
using Utilities;

public class DeckHandler : MonoBehaviour, ILoadable
{
    [Header("Settings")]
    [SerializeField] private Transform _returnPosition;
    [SerializeField, Required] private DPSMeter dpsMeter;
    
    public DPSMeter DPSMeter => dpsMeter;
    
    public Transform ReturnPosition => _returnPosition;

    public void LoadWithScene()
    {
        Registry<DeckHandler>.RegisterSingletonOrLogError(this);
        
        if (dpsMeter != null) dpsMeter.Initialize(Registry<DeckSystem>.GetFirst());
    }

    public void UnLoadWithScene()
    {
        Registry<DeckHandler>.TryRemove(this);
    }
}
