using System.Collections;
using UnityEngine;

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
        // TODO: Impl
        Debug.Log($"Speed Change: {speedChangeGA.Amount}");
        yield return null;
    }
}