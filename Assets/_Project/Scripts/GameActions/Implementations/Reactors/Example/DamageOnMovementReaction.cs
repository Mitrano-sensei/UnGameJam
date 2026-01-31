using EditorAttributes;
using UnityEngine;

public class DamageOnMovementReaction : MonoBehaviour
{
    [SerializeField, ReadOnly] private bool _isSubscribed;
    
    [Button]
    public void Subscribe()
    {
        if (_isSubscribed) return;
        
        _isSubscribed = true;
        ActionSystem.SubscribeReaction<MovementGA>(OnMovement, ReactionTiming.POST);
    }

    [Button]
    public void UnSubscribe()
    {
        if (!_isSubscribed) return;
        
        _isSubscribed = false;
        ActionSystem.UnsubscribeReaction<MovementGA>(OnMovement, ReactionTiming.POST);
    }
    
    private void OnMovement(MovementGA movementGA)
    {
        if (movementGA.Movement == MovementGA.MovementType.UP)
            Debug.Log("Damage on up movement: 2");
        else
            Debug.Log("Moved down, no damage");
    }

}
