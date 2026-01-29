using System.Collections;
using UnityEngine;

public class MovementPerformer : MonoBehaviour
{
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
        // TODO: Impl
        var movementType = movementGA.Movement == MovementGA.MovementType.UP ? "UP" : "DOWN";
        
        Debug.Log($"Move {movementType}");
        yield return null;
    }
}