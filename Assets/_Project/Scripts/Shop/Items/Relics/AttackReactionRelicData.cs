using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
[CreateAssetMenu(fileName = "NewAttackReactionRelic", menuName = "Relic/Attack Reaction Relic")]
public class AttackReactionRelicData : ReactionRelicData<AttackGA>
{
    [SerializeField] private UnityEvent<AttackGA> onAttack;
    protected override UnityEvent<AttackGA> Reaction => onAttack;
}