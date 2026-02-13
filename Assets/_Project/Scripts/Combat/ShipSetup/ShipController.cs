using System;
using System.Collections.Generic;
using EditorAttributes;
using PrimeTween;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

public class ShipController : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private Transform modelTransform;
    
    [Header("Movements")]
    [SerializeField] private TweenSettings movementTweenSettings;

    [Header("Fire")]
    [SerializeField] private TweenSettings fireScaleTweenSetting;
    [SerializeField] private float fireTweenScaleFactor = .6f;
    [SerializeField] private SimpleBullet simpleBulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 10f;
    
    private ShipSetup _shipSetup;
    private bool _isMoving;

    public int CurrentIndex { get; set; }

    private readonly Queue<MovementGA> _moveQueue = new Queue<MovementGA>();
    private HealthSystem _healthSystem;


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

    public void PerformMovement(MovementGA movementGa)
    {
        _moveQueue.Enqueue(movementGa);
    }

    [Button]
    public void Fire(int damage = 1)
    {
        Sequence.Create()
            .Chain(Tween.Scale(modelTransform, new TweenSettings<float>(endValue:fireTweenScaleFactor, settings:fireScaleTweenSetting)))
            .Chain(Tween.Scale(modelTransform, new TweenSettings<float>(endValue: 1f, settings: fireScaleTweenSetting)));
        
        var bullet = Instantiate(simpleBulletPrefab, firePoint.position, Quaternion.identity);
        bullet.SetDamage(damage);
        Rigidbody bulletRb = bullet.RigidBody;
        bulletRb.linearVelocity = Vector2.right * bulletSpeed;
    }

    public void Damage(int amount)
    {
        _healthSystem ??= Registry<HealthSystem>.GetFirst();
        _healthSystem.Damage(amount);
    }

    public bool IsShip() => true;
}