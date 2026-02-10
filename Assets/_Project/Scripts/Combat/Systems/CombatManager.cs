using System;
using System.Linq;
using UnityEngine;
using Utilities;

public class CombatManager : MonoBehaviour, ILoadable
{

    public void LoadWithScene()
    {
        if (Registry<CombatManager>.All.Any())
        {
            Debug.LogError("There is already a combat manager in the scene, only one is allowed at a time");
            return;
        }
        Registry<CombatManager>.TryAdd(this);
        
        var deckSystem = Registry<DeckSystem>.GetFirst();
        if (deckSystem == null)
        {
            Debug.LogError("No Deck System registered :(");
            return;
        }
        deckSystem.Initialize();
    }

    public void UnLoadWithScene()
    {
        Registry<CombatManager>.TryRemove(this);
    }

    private void Start()
    {
        
    }
}