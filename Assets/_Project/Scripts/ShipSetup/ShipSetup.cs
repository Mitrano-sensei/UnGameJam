using EditorAttributes;
using UnityEngine;

public class ShipSetup : MonoBehaviour
{
    [Header("Ship")]
    [SerializeField] private ShipController _shipPrefab;
    [SerializeField] private int _spawnPositionIndex = 1;
    
    private ShipController _ship;
    
    [Header("Rows")]
    [SerializeField] private ShipRow _shipRowPrefab;
    [SerializeField] private Transform _shipRowsParent;
    [SerializeField] private Transform _middleRowPosition;
    [SerializeField, OnValueChanged(nameof(OnDistanceBetweenRowsChanged))] private float _distanceBetweenRows = 1f;
    [SerializeField, Range(3, 7)] private int _rowsCount = 3;
    
    public ShipRow[] ShipRows => _shipRows;
    public ShipController ShipController => _ship;

    private ShipRow[] _shipRows;

    private void Start()
    {
        if (_rowsCount % 2 == 0)
        {
            Debug.LogWarning("Rows count must be odd");
            _rowsCount++;
        }
        
        if (_spawnPositionIndex > _rowsCount)
            _spawnPositionIndex = _rowsCount;
        
        _shipRows = new ShipRow[_rowsCount];
        _shipRows[_rowsCount / 2] = Instantiate(_shipRowPrefab, _middleRowPosition.position, Quaternion.identity, _shipRowsParent);

        for (int i = 0; i < _rowsCount / 2; i++)
        {
            _shipRows[(_rowsCount / 2) + i + 1] = Instantiate(_shipRowPrefab, _middleRowPosition.position + Vector3.up * _distanceBetweenRows * (i + 1), Quaternion.identity,
                _shipRowsParent);
            _shipRows[(_rowsCount / 2) - i - 1] = Instantiate(_shipRowPrefab, _middleRowPosition.position - Vector3.up * _distanceBetweenRows * (i + 1), Quaternion.identity,
                _shipRowsParent);
        }

        for (int i = 0; i < _rowsCount; i++)
        {
            _shipRows[i].Index = i;
        }
        
        _ship = Instantiate(_shipPrefab, _shipRows[_spawnPositionIndex].ShipPosition.position, Quaternion.identity, transform);
        _ship.Initialize(this, _spawnPositionIndex);
    }

    private void OnDistanceBetweenRowsChanged()
    {
        if (!Application.IsPlaying(this)) return;
        
        var middlePos = _shipRows[_rowsCount / 2].transform.position;

        for (int i = 0; i < _rowsCount / 2; i++)
        {
            _shipRows[(_rowsCount / 2) + i + 1].transform.position = middlePos + Vector3.up * _distanceBetweenRows * (i + 1);
            _shipRows[(_rowsCount / 2) - i - 1].transform.position = middlePos - Vector3.up * _distanceBetweenRows * (i + 1);
        }
    }
    
    
}