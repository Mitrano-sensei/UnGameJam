using System;
using System.Collections.Generic;
using EditorAttributes;
using PrimeTween;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    [Header("Movements")]
    [SerializeField] private TweenSettings movementTweenSettings;

    private ShipSetup _shipSetup;
    private bool _isMoving;

    public int CurrentIndex { get; set; }

    private readonly Queue<MovementGA> _moveQueue = new Queue<MovementGA>();


    private void Update()
    {
        if (!_isMoving && _moveQueue.Count != 0) ManageMovement();
    }

    private void ManageMovement()
    {
        var movement = _moveQueue.Dequeue();
        Move((movement.Movement == MovementGA.MovementType.UP ? 1 : -1) * movement.Amount);
    }

    private void Move(int movementAmount)
    {
        var newIndex = CurrentIndex + movementAmount;
        newIndex = Mathf.Clamp(newIndex, 0, _shipSetup.ShipRows.Length - 1);
        if (newIndex == CurrentIndex) return;

        _isMoving = true;
        CurrentIndex = newIndex;

        Tween.PositionY(transform, new(_shipSetup.ShipRows[CurrentIndex].transform.position.y, movementTweenSettings)).OnComplete(() => _isMoving = false);
    }

    public void Initialize(ShipSetup shipSetup, int startIndex)
    {
        this._shipSetup = shipSetup;

        CurrentIndex = startIndex;
    }

    [Button]
    public void GoUp()
    {
        if (!Application.IsPlaying(this)) return;        
        
        var movement = new MovementGA
        {
            Movement = MovementGA.MovementType.UP,
            Amount = 1
        };

        _moveQueue.Enqueue(movement);
    }

    [Button]
    public void GoDown()
    {
        if (!Application.IsPlaying(this)) return;        

        var movement = new MovementGA
        {
            Movement = MovementGA.MovementType.DOWN,
            Amount = 1
        };

        _moveQueue.Enqueue(movement);
    }

    public void PerformMovement(MovementGA movementGa)
    {
        _moveQueue.Enqueue(movementGa);
    }
}