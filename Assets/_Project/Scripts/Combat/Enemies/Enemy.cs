using System;
using UnityEngine;
using Utilities;
using Random = System.Random;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Enemy : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private GameObject healthBar; // TODO
    
    [Header("Settings")]
    private EnemyBehaviour _enemyBehaviour;
    private int _health;
    private int _speed;
    private float _lifeSpan;
    
    private bool _isSpawned;
    private bool _hasInflectedDamage;
    
    private CountdownTimer _lifeTimer;

    private void Update()
    {
        if (!_isSpawned) return;
        
        _enemyBehaviour.Update(transform, _speed);
        _lifeTimer.Tick(Time.deltaTime);
    }

    public void Spawn()
    {
        _enemyBehaviour ??= new DefaultEnemyBehaviour();
        _health = Math.Max(_health, 1);
        _speed = Math.Max(_speed, 1);
        _lifeSpan = Math.Max(_lifeSpan, 2f);
        _lifeTimer = new CountdownTimer(_lifeSpan);
        
        _lifeTimer.OnTimerStop += () => Destroy(gameObject);
        _lifeTimer.Start();
        
        _isSpawned = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Impact");
        var playerDamageable = other.gameObject.GetComponent<IDamageable>();
        if (playerDamageable == null)
        {
            Debug.Log("Not IDamageable");
            return;
        }
        
        if (!playerDamageable.IsShip())
        {
            Debug.Log("Not player");
            return;
        }
        
        Debug.Log("Impact with player");
        
        if (!_hasInflectedDamage) playerDamageable.InflictDamageSelf(1);
        _hasInflectedDamage = true;     // To avoid inflicting damage multiple times for one simple enemy
        Destroy(gameObject);
    }

    public void InflictDamageSelf(int amount)
    {
        _health -= amount;
        
        if (_health < 0)
            Destroy(gameObject); // TODO: Animation
    }

    public Enemy SetEnemyBehaviour(EnemyBehaviour enemyBehaviour)
    {
        this._enemyBehaviour = enemyBehaviour;
        return this;
    }

    public Enemy SetHealth(int health)
    {
        this._health = health;
        return this;
    }

    public Enemy SetSpeed(int speed)
    {
        this._speed = speed;
        return this;
    }
    
    public Enemy SetLifeSpan(float lifeSpan)
    {
        this._lifeSpan = lifeSpan;
        return this;
    }
}
