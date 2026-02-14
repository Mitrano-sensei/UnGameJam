using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Utilities;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Projectile : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    [Header("Settings")]
    [SerializeField] private float scaleFactor = 1f;
    
    public event System.Action<IDamageable> OnHit;
    public event System.Action<float> OnUpdate;
    
    private CountdownTimer _countdown;

    private void Update()
    {
        OnUpdate?.Invoke(Time.deltaTime);
        _countdown?.Tick(Time.deltaTime);
    }
    
    private void OnValidate()
    {
        spriteRenderer.transform.localScale = Vector3.one * scaleFactor;
    }

    private void OnCollisionEnter(Collision other)
    {
        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable == null || damageable.IsShip())
            return;
        
        OnHit?.Invoke(damageable);
    }
    
    public void SetModel(Sprite model)
    {
        spriteRenderer.sprite = model;
        spriteRenderer.transform.localScale = Vector3.one * scaleFactor;
    }

    public void SetScale(float scale)
    {
        scaleFactor = scale;
        spriteRenderer.transform.localScale = Vector3.one * scale;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
    
    public void SetLifeTime(float lifeTime)
    {
        _countdown = new(lifeTime);
        _countdown.OnTimerStop += DestroySelf;
    }

    public void StartLifeTimeTimer()
    {
        _countdown.Start();
    }
}