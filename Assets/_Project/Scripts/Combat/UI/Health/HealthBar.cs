using System;
using TMPro;
using UnityEngine;
using Utilities;

public class HealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BOTWSlider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;

    private void Start()
    {
        var healthSystem = Registry<HealthSystem>.GetFirst();
        if (!healthSystem)
        {
            Debug.LogError("Health System not found");
            return;
        }
        
        healthBar.SetMaxValue(healthSystem.MaxHealth);
        healthText.text = healthSystem.CurrentHealth.ToString();
        
        healthSystem.AddOnHealthChangedListener(OnHealthChanged);
        healthSystem.AddOnMaxHealthChangedListener(OnMaxHealthChanged);
    }
    
    private void OnDestroy()
    {
        var healthSystem = Registry<HealthSystem>.GetFirst();
        if (!healthSystem)
        {
            Debug.LogWarning("Health System not found");
            return;
        }
        
        healthSystem.RemoveOnHealthChangedListener(OnHealthChanged);
        healthSystem.RemoveOnMaxHealthChangedListener(OnMaxHealthChanged);
    }
    
    private void OnHealthChanged(int oldH, int newH)
    {
        healthBar.SetCurrentValue(newH);
        healthText.text = newH.ToString();
    }
    
    private void OnMaxHealthChanged(int oldH, int newH)
    {
        healthBar.SetMaxValue(newH);
    }
}
