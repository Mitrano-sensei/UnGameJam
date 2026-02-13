using System.Collections;
using UnityEngine;
using Utilities;

public class SpeedChangePerformer : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<SpeedChangeGA>(SpeedChangePerform);
    }
    
    private void OnDisable()
    {
        ActionSystem.DetachPerformer<SpeedChangeGA>();
    }
    
    private IEnumerator SpeedChangePerform(SpeedChangeGA speedChangeGA)
    {
        var statSystem = Registry<StatSystem>.GetFirst();
        statSystem.AddStatModifier(StatSystem.StatType.Speed, speedChangeGA.Amount);
        Debug.Log($"Speed Change: {speedChangeGA.Amount}");
        yield return null;
    }
}