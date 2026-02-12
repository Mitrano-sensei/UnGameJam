using System;
using System.Linq;
using UnityEngine;
using Utilities;

public class CombatManager : MonoBehaviour, ILoadable
{

    public void LoadWithScene()
    {
        Registry<CombatManager>.RegisterSingletonOrLogError(this);
        
        // Initialize Deck
        var deckSystem = Registry<DeckSystem>.GetFirst();
        if (deckSystem == null) Debug.LogError("No Deck System registered :(");
        else deckSystem.Initialize();
        
        // DPS
        var deckHandler = Registry<DeckHandler>.GetFirst();
        if (deckHandler == null) Debug.LogError("No Deck Handler registered :(");
        else deckHandler.DPSMeter.StartDPS();
    }

    public void UnLoadWithScene()
    {
        var deckSystem = Registry<DeckSystem>.GetFirst();
        if (deckSystem == null)
        {
            Debug.LogWarning("No Deck System registered :/");
        }
        else deckSystem.OnEndCombatPHase();
        
        Registry<CombatManager>.TryRemove(this);
    }
}