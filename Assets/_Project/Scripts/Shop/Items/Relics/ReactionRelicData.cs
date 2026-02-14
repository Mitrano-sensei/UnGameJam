using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[Serializable]
public abstract class ReactionRelicData<T> : RelicData where T : GameAction
{
    [Header("Reaction")]
    [SerializeField] private ReactionTiming timing;
    protected abstract UnityEvent<T> Reaction { get; }

    private Action<T> _reactionRef;
    public override void Apply()
    {
        if (_reactionRef != null)
        {
            Debug.LogError("Trying to apply the same relic twice");
            return;
        }
        
        _reactionRef = ga =>
        {
            OnUseRelic?.Invoke();
            Reaction?.Invoke(ga);
        };
        ActionSystem.SubscribeReaction<T>(_reactionRef, timing);
    }

    public override void Remove()
    {
        if (_reactionRef == null)
        {
            Debug.LogError("Trying to remove a non-applied relic");
            return;
        }
        ActionSystem.UnsubscribeReaction<T>(_reactionRef, timing);
        _reactionRef = null;
    }

    public override void OnBuyEffect() { }
}