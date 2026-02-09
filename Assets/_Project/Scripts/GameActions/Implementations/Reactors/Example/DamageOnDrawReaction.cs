using EditorAttributes;
using UnityEngine;

public class DamageOnDrawReaction : MonoBehaviour
{
    [SerializeField, ReadOnly] private bool _isSubscribed;
    
    [Button]
    public void Subscribe()
    {
        if (_isSubscribed) return;
        
        _isSubscribed = true;
        ActionSystem.SubscribeReaction<DrawCardGA>(OnDraw, ReactionTiming.POST);
    }

    [Button]
    public void UnSubscribe()
    {
        if (!_isSubscribed) return;
        
        _isSubscribed = false;
        ActionSystem.UnsubscribeReaction<DrawCardGA>(OnDraw, ReactionTiming.POST);
    }
    
    private void OnDraw(DrawCardGA drawCardGA)
    {
        Debug.Log($"Damage on draw. Drew {drawCardGA.Amount}, dealing {drawCardGA.Amount * 2} damages");
    }
    
}
