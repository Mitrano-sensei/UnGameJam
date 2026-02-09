using System;
using UnityEngine;

[Serializable]
public class MovementGA : GameAction
{
    [SerializeField] private MovementType movementType;
    [SerializeField] private int amount = 1;
    
    public MovementType Movement
    {
        get => movementType;
        set => movementType = value;
    }
    public int Amount
    {
        get => amount;
        set => amount = value;
    }
    public bool IsUp => movementType == MovementType.UP;

    public enum MovementType
    {
        UP,
        DOWN
    }
}