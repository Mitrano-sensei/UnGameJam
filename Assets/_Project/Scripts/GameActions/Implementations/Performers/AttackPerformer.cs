using System.Collections;
using UnityEngine;
using Utilities;

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

    private IEnumerator AttackPerform(AttackGA attackGA)
    {
        var shipSetup = Registry<ShipSetup>.GetFirst();
        var shipController = shipSetup.ShipController;

        shipController.Fire();
        
        Debug.Log($"Attack for {attackGA.Damage} Damages");
        yield return null;
    }
}