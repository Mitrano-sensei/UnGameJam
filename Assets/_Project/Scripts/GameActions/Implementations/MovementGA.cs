using System;
using UnityEngine;

[Serializable]
public class MovementGA : GameAction
{
    [SerializeField] private MovementType movementType;
    [SerializeField] private int amount = 1;
    
    public MovementType Movement => movementType;
    public int Amount => amount;
    public bool IsUp => movementType == MovementType.UP;

    public enum MovementType
    {
        UP,
        DOWN
    }
}