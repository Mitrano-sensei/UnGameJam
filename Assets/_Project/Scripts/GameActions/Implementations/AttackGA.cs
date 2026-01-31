using System;
using UnityEngine;

[Serializable]
public class AttackGA : GameAction
{
    [SerializeField] private int damage = 1;
    
    public int Damage => damage;
}