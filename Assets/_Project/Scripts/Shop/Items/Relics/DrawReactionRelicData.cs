using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
[CreateAssetMenu(fileName = "NewDrawReactionRelic", menuName = "Relic/Draw Reaction Relic")]
public class DrawReactionRelicData : ReactionRelicData<DrawCardGA>
{
    [SerializeField] private UnityEvent<DrawCardGA> onDraw;
    protected override UnityEvent<DrawCardGA> Reaction => onDraw;
}