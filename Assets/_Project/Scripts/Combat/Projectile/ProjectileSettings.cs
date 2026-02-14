using System;
using SerializeReferenceEditor;
using UnityEngine;

[Serializable]
public class ProjectileSettings
{
    [Header("Prefab")]
    [SerializeField] private Projectile projectilePrefab;
    
    [Header("Model")]
    [SerializeField] private Sprite model;
    [SerializeField] private float modelScaleFactor = 1f;
    
    [Header("Behaviour")]
    [SerializeField, SerializeReference, SR] private ProjectileBehaviour behaviour;
    [SerializeField] private int damage = 1;
    public Projectile ProjectilePrefab => projectilePrefab;

    private Projectile _projectileRef;
    
    public void Spawn(Projectile projectile)
    {
        if (behaviour == null)
        {
            Debug.LogError("Projectile behaviour is null");
            return;
        }

        _projectileRef = projectile;
        projectile.SetModel(model);
        projectile.SetScale(modelScaleFactor);
        
        behaviour.Initialize(projectile);
        behaviour.OnSpawn();
        projectile.OnHit += enemy => enemy.InflictDamageSelf(damage);
        projectile.OnUpdate += Update;
    }

    public void DestroyProjectile()
    {
        _projectileRef.DestroySelf();
    }

    public void Update(float deltaTime)
    {
        if (behaviour == null)
        {
            Debug.LogError("Projectile behaviour is null");
            return;
        }

        behaviour.Update(deltaTime);
    }
}