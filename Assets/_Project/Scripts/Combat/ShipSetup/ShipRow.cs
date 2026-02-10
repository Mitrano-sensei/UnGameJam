using UnityEngine;

public class ShipRow : MonoBehaviour
{
    [SerializeField] private Transform _shipPosition;
    [SerializeField] private Transform _spawnPosition;
    
    public Transform ShipPosition => _shipPosition;
    public Transform SpawnPosition => _spawnPosition;
    
    public int Index { get; set; }

    private void Start()
    {
        if (_shipPosition == null || _spawnPosition == null)
            Debug.LogError("No ship position or spawn position on ship row");
    }
}