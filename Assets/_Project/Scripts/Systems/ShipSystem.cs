using UnityEngine;
using Utilities;

public class ShipSystem : Singleton<ShipSystem>
{
    [Header("References")]
    [SerializeField] private ShipSetup _shipSetup;

    public ShipController ShipController => _shipSetup.ShipController;

}
