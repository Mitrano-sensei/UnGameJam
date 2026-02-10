using System;
using UnityEngine;
using Utilities;

public class ShipSystem : Singleton<ShipSystem>
{
    public ShipController ShipController
    {
        get
        {
            var shipSetup = Registry<ShipSetup>.GetFirst();
            if (shipSetup) return shipSetup.ShipController;
            
            Debug.LogWarning("Getting ShipController but there is no ship setup in the scene");
            return null;
        }
    }
}