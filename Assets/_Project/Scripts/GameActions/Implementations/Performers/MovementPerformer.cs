using System.Collections;
using UnityEngine;

public class MovementPerformer : MonoBehaviour
{
    private ShipSystem _shipSystem;

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<MovementGA>(MovementPerform);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<MovementGA>();
    }
    
    private IEnumerator MovementPerform(MovementGA movementGA)
    {
        _shipSystem ??= ShipSystem.Instance;
        
        var movementType = movementGA.Movement == MovementGA.MovementType.UP ? "UP" : "DOWN";
        Debug.Log($"Move {movementType}");

        _shipSystem.ShipController.PerformMovement(movementGA);
        yield return null;
    }
}