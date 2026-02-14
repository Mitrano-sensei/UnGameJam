using EditorAttributes;
using UnityEngine;
using Utilities;

public class ShipRow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _shipPosition;
    
    [Header("Settings")]
    [HelpBox("-0.05 corresponds to 5% of the screen width to the left, 0.05 corresponds to 5% of the screen width to the right")]
    [SerializeField] private float percentToIndicator = -0.05f; // 0.05f is 5% of the screen width
    [SerializeField] private float percentToSpawn = 0.1f; 
    
    public Vector3 ShipPosition => _shipPosition.position;
    public Vector3 SpawnPosition => GetSpawnPosition();
    public Vector3 IndicatorPosition => GetIndicatorPosition();
    
    public int Index { get; set; }

    private void Start()
    {
        if (_shipPosition == null)
            Debug.LogError("No ship position or spawn position on ship row");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(IndicatorPosition, 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(SpawnPosition, 0.1f);
    }

    private Vector3 GetIndicatorPosition()
    {
        var cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("No camera found");
            return Vector3.zero;
        }
        
        // Check if camera is Ortho or perspective
        bool isOrtho = cam.orthographic;
        var worldPos = GetXPositionFromViewport(cam, percentToIndicator, isOrtho);
        return cam.WorldToScreenPoint(worldPos);
    }

    private Vector3 GetSpawnPosition()
    {
        var cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("No camera found");
            return Vector3.zero;
        }
        
        // Check if camera is Ortho or perspective
        bool isOrtho = cam.orthographic;
        return GetXPositionFromViewport(cam, percentToSpawn, isOrtho);
    } 
    
    private Vector3 GetXPositionFromViewport(Camera cam, float percent, bool isOrtho)
    {
        float distanceToCamera = 0f;
        
        if (!isOrtho)
        {
            distanceToCamera = ShipPosition.z - cam.transform.position.z;
        }
        
        Vector3 viewPortPoint = new Vector3(1f + percent, 1f, distanceToCamera);
        Vector3 worldPoint = cam.ViewportToWorldPoint(viewPortPoint).With(y:ShipPosition.y, z:ShipPosition.z);
        return worldPoint;
    }
    
}