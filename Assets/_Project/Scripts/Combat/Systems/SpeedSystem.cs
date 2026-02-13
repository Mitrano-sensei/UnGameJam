using System;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

public class SpeedSystem : MonoBehaviour, ILoadable
{
    [Header("Settings")]
    [SerializeField] private int baseSpeed = 5;
    private int _currentSpeed;
    
    [Header("Events")]
    public UnityEvent<int, int> OnSpeedChanged = new(); // Old, new
    
    public void LoadWithScene()
    {
        Registry<SpeedSystem>.RegisterSingletonOrLogError(this);
        
        var statSystem = Registry<StatSystem>.GetFirst();
        statSystem.AddStatListener(OnSpeedStatChanged);
    }

    public void UnLoadWithScene()
    {
        Registry<SpeedSystem>.TryRemove(this);
    }
    
    private void OnSpeedStatChanged(StatSystem.StatType type, int oldV, int newV)
    {
        if (type != StatSystem.StatType.Speed) return;
        SetCurrentSpeed(baseSpeed + newV);
    }
    
    public void SetCurrentSpeed(int speed)
    {
        var oldSpeed = _currentSpeed;
        _currentSpeed = Math.Max(speed, 1);
        
        OnSpeedChanged.Invoke(oldSpeed, _currentSpeed);
    }
}
