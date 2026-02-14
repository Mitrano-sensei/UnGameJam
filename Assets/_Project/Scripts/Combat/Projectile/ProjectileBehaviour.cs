using System;
using EditorAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

[Serializable]
public abstract class ProjectileBehaviour
{
    public abstract void Initialize(Projectile projectile);
    public abstract void OnSpawn();
    public abstract void Update(float deltaTime);
}

[Serializable]
public class CustomizableProjectileBehaviour : ProjectileBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector3 spawnForceDirection = Vector3.right;
    [SerializeField] private float spawnForceSpeed = 1f;
    [SerializeField] private ForceMode spawnForceMode = ForceMode.Impulse;
    [SerializeField] private float lifeTime = 1f;
    
    [SerializeField] private bool useGravity = false;
    [SerializeField] private float addGravityForce = 0f;
    
    [SerializeField] private bool isKinematic = true;

    private ProjectileSettings _projectileSettings;
    private Rigidbody _rb;
    private Projectile _projectile;


    public override void Initialize(Projectile projectile)
    {
        var rb = projectile.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody is null");
            return;
        }

        _projectile = projectile;
        _rb = rb;
        projectile.SetLifeTime(lifeTime);
    }

    public override void OnSpawn()
    {
        _rb.AddForce(spawnForceDirection * spawnForceSpeed, spawnForceMode);
        _rb.isKinematic = isKinematic;
        _rb.useGravity = useGravity;
        
        _projectile.StartLifeTimeTimer();
    }

    public override void Update(float deltaTime)
    {
        if (useGravity && addGravityForce > 0f) _rb.AddForce(Vector3.down * addGravityForce, ForceMode.Acceleration);
    }
    
}