using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

/**
 * From https://youtu.be/ls5zeiDCfvI?list=PLABo1vVOfXp7cTg9vbHDh-4cOmjQkwJxw
 */
public class ActionSystem : Singleton<ActionSystem>
{
    private List<GameAction> reactions = null;
    public bool IsPerforming { get; private set; } = false;
    
    private static Dictionary<Type, List<Action<GameAction>>> preSubs = new();
    private static Dictionary<Type, List<Action<GameAction>>> postSubs = new();
    private static Dictionary<Type, Func<GameAction,IEnumerator>> performers = new();
    
    private static readonly Dictionary<(Type, ReactionTiming, Delegate), Action<GameAction>> _wrapperReferences = new();

    public void Perform(GameAction action, Action OnPerformFinished = null, bool manageIsPerforming = true)
    {
        if (manageIsPerforming && IsPerforming) return;
        if (manageIsPerforming) IsPerforming = true;

        StartCoroutine(Flow(action, () =>
        {
            if (manageIsPerforming) IsPerforming = false;
            OnPerformFinished?.Invoke();
        }));
    }

    public void Perform(List<GameAction> actions, Action OnPerformFinished = null, Action OnEachPerformFinished = null)
    {
        if (IsPerforming) return;
        IsPerforming = true;
        Action OnLastActionFinished = () =>
        {
            OnEachPerformFinished?.Invoke();
            OnPerformFinished?.Invoke();
            IsPerforming = false;
        };
        
        for (int i = 0; i < actions.Count; i++)
        {
            var action = actions[i];
            bool isLast = i == actions.Count - 1;
            Perform(action, isLast ? OnLastActionFinished : OnEachPerformFinished, false);
        }
    }

    public void AddReaction(GameAction gameAction)
    {
        reactions?.Add(gameAction);
    }

    private IEnumerator Flow(GameAction action, System.Action OnFlowFinished = null)
    {
        reactions = action.PreReactions;
        PerformSubscribers(action, preSubs);
        yield return PerformReactions();

        reactions = action.PerformReactions;
        yield return PerformPerformer(action);
        yield return PerformReactions();

        reactions = action.PostReactions;
        PerformSubscribers(action, postSubs);
        yield return PerformReactions();

        OnFlowFinished?.Invoke();
    }

    private IEnumerator PerformPerformer(GameAction action)
    {
        Type type = action.GetType();

        if (performers.ContainsKey(type))
        {
            yield return performers[type](action);
        }
    }

    private void PerformSubscribers(GameAction action, Dictionary<Type, List<Action<GameAction>>> subs)
    {
        Type type = action.GetType();
        if (subs.ContainsKey(type))
        {
            foreach (Action<GameAction> subscriber in subs[type])
            {
                subscriber(action);
            }
        }
    }

    private IEnumerator PerformReactions()
    {
        foreach (var reaction in reactions)
        {
            yield return Flow(reaction);
        }
    }

    public static void AttachPerformer<T>(Func<T, IEnumerator> performer) where T : GameAction
    {
        Type type = typeof(T);
        IEnumerator wrappedPerformer(GameAction action) => performer((T)action);
        if (performers.ContainsKey(type)) performers[type] = wrappedPerformer;
        else performers.Add(type, wrappedPerformer);
    }

    public static void DetachPerformer<T>() where T : GameAction
    {
        Type type = typeof(T);
        if (performers.ContainsKey(type)) performers.Remove(type);
    }
    
    public static void SubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
        var type = typeof(T);
        void wrappedReaction(GameAction action) => reaction((T)action);
        _wrapperReferences.Add((type, timing, reaction), wrappedReaction);
        
        if (!subs.ContainsKey(type))
        {
            subs.Add(type, new());
            subs[type].Add(wrappedReaction);
        }
        else subs[type].Add(wrappedReaction);
    }

    public static void UnsubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
        var type = typeof(T);
        if (!subs.ContainsKey(type)) return;
        
        var wrappedReaction = _wrapperReferences[(type, timing, reaction)];
        if (wrappedReaction == null)
        {
            Debug.LogError("No reaction found to unsubscribe");
            return;
        }
        subs[type].Remove(wrappedReaction);
        _wrapperReferences[(type, timing, reaction)] = null;
    }
}