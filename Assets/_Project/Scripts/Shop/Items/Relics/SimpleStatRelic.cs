using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

[CreateAssetMenu(menuName = "Relic/Simple Stat Relic", fileName = "Simple Stat Relic", order = 0)]
public class SimpleStatRelic : RelicData
{
    [SerializeField] private List<StatChangeData> statChanges;
    
    public override void ApplyOnStartOfCombat() => OnUseRelic?.Invoke();

    public override void RemoveOnStartOfCombat() { }

    public override void OnBuyEffect()
    {
        var statSystem = Registry<StatSystem>.GetFirst();
        if (!statSystem)
        {
            Debug.LogError("No Stat System registered");
            return;
        } 
        statChanges.ForEach(x => statSystem.AddStatModifier(x.StatType, x.Amount));
    }

    public override void OnEndOfCombat() { }
}

[Serializable]
public class StatChangeData
{
    [SerializeField] private StatSystem.StatType statType;
    [SerializeField] private int amount;
    
    public StatSystem.StatType StatType => statType;
    public int Amount => amount;
}