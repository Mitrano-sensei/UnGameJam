using System;
using System.Linq;
using EditorAttributes;
using UnityEngine;
using Utilities;

public class DeckHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform _returnPosition;
    [SerializeField, Required] private DPSMeter dpsMeter;
    
    public Transform ReturnPosition => _returnPosition;
    
    public void Awake()
    {
        if (Registry<DeckHandler>.All.Any())
        {
            Debug.LogError("There is already a deck handler in the scene, only one is allowed at a time");
            return;
        }
        
        Registry<DeckHandler>.TryAdd(this);
    }

    public void Start()
    {
        if (dpsMeter != null) dpsMeter.Initialize(Registry<DeckSystem>.GetFirst());
    }
    
    private void OnDisable()
    {
        Registry<DeckHandler>.TryRemove(this);
    }
}
