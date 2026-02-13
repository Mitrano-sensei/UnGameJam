using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
[CreateAssetMenu(fileName = "NewMovementReactionRelic", menuName = "Relic/Movement Reaction Relic")]
public class MovementReactionRelicData : ReactionRelicData<MovementGA>
{
    [SerializeField] private UnityEvent<MovementGA> onMove;
    protected override UnityEvent<MovementGA> Reaction => onMove;
}