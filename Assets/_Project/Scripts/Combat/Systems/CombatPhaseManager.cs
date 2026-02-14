using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

public class CombatPhaseManager : MonoBehaviour, ILoadable
{
    [Header("Refrences")]
    [SerializeField] private RelicPreviewHolder _relicPreviewRef;
    [SerializeField] private StartCombatAnimationOverlay startCombatAnimationOverlay;
    [SerializeField] private EndCombatAnimationOverlay endCombatAnimationOverlay;
    [SerializeField] private GameOverAnimationOverlay gameOverAnimationOverlay;

    private GameState currentState;

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

        // Initialize Health Listener
        var healthSystem = Registry<HealthSystem>.GetFirst();
        if (healthSystem == null) Debug.LogError("No Health System registered :(");
        else healthSystem.AddOnHealthChangedListener(OnHealthChangedListener);

        ChangeState(GameState.START_ANIMATION);
    }

    public void UnLoadWithScene()
    {
        Debug.Log("Call Unload combat phase manager");
        
        // Relic System
        var relicSystem = Registry<RelicSystem>.GetFirst();
        if (relicSystem == null) Debug.LogWarning("No Relic System registered :/");
        else relicSystem.RemoveEffectRelics();

        Registry<CombatPhaseManager>.TryRemove(this);
    }

    private void OnHealthChangedListener(int oldH, int newH)
    {
        if (newH > 0) return;

        ChangeState(GameState.GAME_OVER);
    }

    public void OnCombatStart()
    {
        // DPS Initialization
        var deckHandler = Registry<DeckHandler>.GetFirst();
        if (deckHandler == null) Debug.LogError("No Deck Handler registered :(");
        else deckHandler.DPSMeter.StartDPS();

        // Enemies Initialization TODO


        // Distance Manager Initialization TODO


        // 
    }

    private void OnPlayAnimation()
    {
        StartCoroutine(startCombatAnimationOverlay.PlayAnimation(onEndAnimation: () => ChangeState(GameState.START_COMBAT)));
    }
    
    public void OnCombatEnd()
    {
        // Money
        var moneySystem = Registry<MoneySystem>.GetFirst();
        if (moneySystem == null) Debug.LogWarning("No Money System registered :/");
        else moneySystem.AddMoney(3);
        
        // Relic
        var relicSystem = Registry<RelicSystem>.GetFirst();
        if (relicSystem == null) Debug.LogWarning("No Relic System registered :/");
        else relicSystem.ApplyOnEndOfCombat();
        
        // Deck
        var deckSystem = Registry<DeckSystem>.GetFirst();
        if (deckSystem == null) Debug.LogWarning("No Deck System registered :/");
        else deckSystem.OnEndCombatPhase();
        
        // Overlay Animation
        StartCoroutine(endCombatAnimationOverlay.PlayAnimation(onEndAnimation: CombatSceneManager.SwitchToShop));
    }

    public void OnGameOver()
    {
        StartCoroutine(gameOverAnimationOverlay.PlayAnimation(onEndAnimation: CombatSceneManager.EndSession));
    } 


    #region State Machine

    private void ChangeState(GameState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case GameState.START_ANIMATION:
                OnPlayAnimation();
                break;
            case GameState.START_COMBAT:
                OnCombatStart();
                break;
            case GameState.END_COMBAT:
                OnCombatEnd();
                break;
            case GameState.GAME_OVER:
                OnGameOver();
                break;
        }
    }

    private enum GameState
    {
        START_ANIMATION,
        START_COMBAT,
        END_COMBAT,
        GAME_OVER,
    }

    #endregion
}