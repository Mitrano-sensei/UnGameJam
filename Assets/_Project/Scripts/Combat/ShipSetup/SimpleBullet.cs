using EditorAttributes;
using UnityEngine;

public class SimpleBullet : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Required] private Rigidbody rb;
    
    public Rigidbody RigidBody => rb;

    private int _damage = 1;

    private void Update()
    {
        if (transform.position.x > 120f)
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        var enemyDamageable = other.gameObject.GetComponent<IDamageable>();
        if (enemyDamageable == null || enemyDamageable.IsShip()) return;
        
        enemyDamageable.InflictDamageSelf(_damage);
        Destroy(gameObject);
    }
    
    public void SetDamage(int damage) => _damage = damage;
}

public interface IDamageable
{
    public bool IsShip()
    {
        return false;
    }
    void InflictDamageSelf(int amount);
}