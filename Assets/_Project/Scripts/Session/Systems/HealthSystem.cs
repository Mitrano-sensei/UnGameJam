using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Utilities;

public class HealthSystem : MonoBehaviour, ILoadable
{
    [FormerlySerializedAs("maxHealth")]
    [Header("Settings")]
    [SerializeField] private int baseMaxHealth = 5;
    [SerializeField, ReadOnly] private int currentHealth = 5;

    private int _maxHealth;
    
    public int BaseMaxHealth => baseMaxHealth;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => _maxHealth;

    [Header("Events")]
    [SerializeField] private UnityEvent<int, int> onHealthChanged = new();      // Old Value, New Value
    [SerializeField] private UnityEvent<int, int> onMaxHealthChanged = new();   // Old Value, New Value
    
    [Header("References")]
    private StatSystem _statSystem;
    
    public void LoadWithScene()
    {
        Registry<HealthSystem>.RegisterSingletonOrLogError(this);
        _statSystem = Registry<StatSystem>.GetFirst();
        
        // Max Health
        _maxHealth = baseMaxHealth + _statSystem.GetStatModifierValue(StatSystem.StatType.MaxHealth);
        currentHealth = _maxHealth;
        _statSystem.AddStatListener(OnMaxHealthStatChanged);
        
        onHealthChanged.AddListener((oldH, newH) => Debug.Log($"Health changed from {oldH} to {newH}"));
    }

    private void OnMaxHealthStatChanged(StatSystem.StatType type, int oldValue, int newValue)
    {
        if (type != StatSystem.StatType.MaxHealth) return;
        
        _maxHealth = baseMaxHealth + _statSystem.GetStatModifierValue(StatSystem.StatType.MaxHealth);
        onMaxHealthChanged.Invoke(baseMaxHealth + oldValue, baseMaxHealth + newValue);
    }

    public void UnLoadWithScene()
    {
        Registry<HealthSystem>.TryRemove(this);
    }
}