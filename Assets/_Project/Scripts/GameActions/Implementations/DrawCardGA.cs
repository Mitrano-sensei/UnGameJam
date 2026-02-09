using System;
using UnityEngine;

[Serializable]
public class DrawCardGA : GameAction
{
    [SerializeField] private int amount;
    
    public int Amount => amount;
}