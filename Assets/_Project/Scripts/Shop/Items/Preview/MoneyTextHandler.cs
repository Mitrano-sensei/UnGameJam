using System;
using TMPro;
using UnityEngine;
using Utilities;

public class MoneyTextHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI moneyText;
    
    [Header("Settings")]
    [SerializeField] private String format = "Money: {0}g";
    
    private void Start()
    {
        var moneySystem = Registry<MoneySystem>.GetFirst();
        if (!moneySystem)
        {
            Debug.LogError("No Money System found");
            return;
        }

        moneySystem.OnMoneyChanged.AddListener(e => moneyText.text = string.Format(format, e.NewMoney));
        moneyText.text = string.Format(format, moneySystem.Money);
    }
}
