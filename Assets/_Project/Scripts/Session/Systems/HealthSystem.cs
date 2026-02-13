using EditorAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Utilities;

public class HealthSystem : MonoBehaviour, ILoadable
{
    [FormerlySerializedAs("maxHealth")]
    [Header("Settings")]
    [SerializeField] private int baseMaxHealth = 5;
    [SerializeField, Unity.Collections.ReadOnly] private int currentHealth = 5;

    private int _maxHealth;

    public int BaseMaxHealth => baseMaxHealth;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => _maxHealth;

    [Header("Events")]
    [SerializeField] private UnityEvent<int, int> onHealthChanged = new(); // Old Value, New Value
    [SerializeField] private UnityEvent<int, int> onMaxHealthChanged = new(); // Old Value, New Value

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

    public void AddOnHealthChangedListener(UnityAction<int, int> listener) => onHealthChanged.AddListener(listener);
    public void RemoveOnHealthChangedListener(UnityAction<int, int> listener) => onHealthChanged.RemoveListener(listener);

    public void AddOnMaxHealthChangedListener(UnityAction<int, int> listener) => onMaxHealthChanged.AddListener(listener);
    public void RemoveOnMaxHealthChangedListener(UnityAction<int, int> listener) => onMaxHealthChanged.RemoveListener(listener);

    public void SetHealth(int value)
    {
        var oldHealth = currentHealth;
        currentHealth = Mathf.Clamp(value, 0, _maxHealth);
        onHealthChanged.Invoke(oldHealth, currentHealth);
    }

    public void AddHealth(int value) => SetHealth(currentHealth + value);
    public void FullHeal() => AddHealth(MaxHealth);
    public void Damage(int value) => SetHealth(currentHealth - value);


    #region Debug

    [Header("Debug")]
    [ShowInInspector, ReadOnly] public int CurrentHealthDebug => CurrentHealth;
    [ShowInInspector, ReadOnly] public int MaxHealthDebug => MaxHealth;


    [Button]
    public void HealOneHealth() => AddHealth(1);

    [Button]
    public void DamageOneHealth() => Damage(1);

    #endregion
}