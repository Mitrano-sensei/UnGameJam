using System.Collections;
using UnityEngine;

public class AttackPerformer : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<AttackGA>(AttackPerform);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<AttackGA>();
    }

    private IEnumerator AttackPerform(AttackGA drawCardGA)
    {
        // TODO: Impl
        Debug.Log($"Attack for {drawCardGA.Damage} Damages");
        yield return null;
    }
}