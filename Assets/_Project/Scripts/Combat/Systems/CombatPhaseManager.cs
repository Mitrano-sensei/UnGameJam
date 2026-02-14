using UnityEngine;
using Utilities;

public class CombatPhaseManager : MonoBehaviour, ILoadable
{
    [Header("Refrences")]
    [SerializeField] private RelicPreviewHolder _relicPreviewRef;

    public void LoadWithScene()
    {
        Registry<CombatPhaseManager>.RegisterSingletonOrLogError(this);
        
        // Relic Holder
        _relicPreviewRef.GeneratePreviews();
        
        // Relics
        var relicSystem = Registry<RelicSystem>.GetFirst();
        if (relicSystem == null) Debug.LogError("No Relic System registered :(");
        else relicSystem.ApplyEffectRelics();
        
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
        // Deck System
        var deckSystem = Registry<DeckSystem>.GetFirst();
        if (deckSystem == null) Debug.LogWarning("No Deck System registered :/");
        else deckSystem.OnEndCombatPhase();
        
        // Relic System
        var relicSystem = Registry<RelicSystem>.GetFirst();
        if (relicSystem == null) Debug.LogWarning("No Relic System registered :/");
        else relicSystem.RemoveEffectRelics();
        
        Registry<CombatPhaseManager>.TryRemove(this);
    }
}