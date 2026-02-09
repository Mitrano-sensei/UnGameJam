using UnityEngine;
using Utilities;

public class ShipSystem : Singleton<ShipSystem>
{
    [Header("References")]
    private ShipSetup _shipSetup;

    public ShipController ShipController
    {
        get
        {
            _shipSetup ??= Registry<ShipSetup>.GetFirst();
            if (!_shipSetup)
            {
                Debug.LogError("No ship setup found");
                return null;
            }
            return _shipSetup.ShipController;
        }
    }
}
