using System;
using UnityEngine;

[Serializable]
public class SpeedChangeGA : GameAction
{
    [SerializeField] private int amount;
    public int Amount => amount;
}