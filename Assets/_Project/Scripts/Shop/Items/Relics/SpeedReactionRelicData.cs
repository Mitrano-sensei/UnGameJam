using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
[CreateAssetMenu(fileName = "NewSpeedReactionRelic", menuName = "Relic/Speed Reaction Relic")]
public class SpeedReactionRelicData : ReactionRelicData<SpeedChangeGA>
{
    [SerializeField] private UnityEvent<SpeedChangeGA> onSpeedChange;
    protected override UnityEvent<SpeedChangeGA> Reaction => onSpeedChange;
}