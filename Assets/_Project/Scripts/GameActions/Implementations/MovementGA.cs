using System;
using UnityEngine;

[Serializable]
public class MovementGA : GameAction
{
    [SerializeField] private MovementType movementType;
    
    public MovementType Movement => movementType;

    public enum MovementType
    {
        UP,
        DOWN
    }
}