using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using EditorAttributes;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

public class StatSystem : MonoBehaviour, ILoadable
{
    [SerializeField, SerializedDictionary("Stat Type", "Value")] 
    private Dictionary<StatType, int> _statModifiers = new();
    private UnityEvent<StatType, int, int> _onStatChanged = new();
    
    [SerializeField] private bool isDebug;
    
    public void LoadWithScene()
    {
        Registry<StatSystem>.RegisterSingletonOrLogError(this);
        
        _onStatChanged.AddListener((type, o, n) =>
        {
            if (!isDebug) return;
            
            Debug.Log($"Stat {type} changed from {o} to {n}");
        });
    }

    public void UnLoadWithScene()
    {
        Registry<StatSystem>.TryRemove(this);
    }

    public int GetStatModifierValue(StatType type)
    {
        return _statModifiers.GetValueOrDefault(type, 0);
    }
    
    public void AddStatModifier(StatType type, int value)
    {
        int oldValue = _statModifiers.GetValueOrDefault(type, 0);
        _statModifiers[type] = oldValue + value;

        _onStatChanged.Invoke(type, oldValue, oldValue + value);
    }
    public void RemoveStatModifier(StatType statType, int amount) => AddStatModifier(statType, -amount);
    
    public void AddStatListener(UnityAction<StatType, int, int> listener) => _onStatChanged.AddListener(listener);
    public void RemoveStatListener(UnityAction<StatType, int, int> listener) => _onStatChanged.RemoveListener(listener);
    
    public enum StatType
    {
        MaxHealth,
        DrawPerSecond,
        HandSize,
        MoneyPerCombat
    }

}