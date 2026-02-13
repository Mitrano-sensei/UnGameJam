using System;
using EditorAttributes;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

public class DPSMeter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BOTWSlider slider;
    
    [Header("Settings")]
    [SerializeField, Suffix("draw per second")] private float startDPS = .5f;
    
    private float _currentDPS;
    private DeckSystem _deckSystem;
    private LoopTimer _loopTimer;
    
    private float DrawPeriod => 1f / _currentDPS;
    
    
    [Header("Events")]
    [HideInInspector] public UnityEvent OnDPSFinished = new UnityEvent();

    [Header("Misc")]
    [SerializeField] private bool drawOnInit;
    
    [Header("Debug")]
    [SerializeField] private bool isDebug;

    private void Update()
    {
        if (_loopTimer == null) return;
        _loopTimer.Tick(Time.deltaTime);
        slider.SetCurrentValue(_loopTimer.GetCurrentTime());
    }

    public void Initialize(DeckSystem deckSystem)
    {
        var statSystem = Registry<StatSystem>.GetFirst();
        if (statSystem == null)
        {
            Debug.LogError("No Stat System registered :(");
            return;
        }
        _currentDPS = startDPS + statSystem.GetStatModifierValue(StatSystem.StatType.DrawPerSecond);
        statSystem.AddStatListener(OnDPSChanged);
        
        slider.SetMaxValue(DrawPeriod);
        slider.SetCurrentValue(0f);

        _loopTimer = new LoopTimer(DrawPeriod);
        _loopTimer.OnLoop += () => OnDPSFinished?.Invoke();
        
        // Debug
        _loopTimer.OnTimerStart += () =>
        {
            if (!isDebug) return;
            Debug.Log("Starting");
        };
        _loopTimer.OnLoop += () =>
        {
            if (!isDebug) return;
            Debug.Log("Loop");
        };
        _loopTimer.OnLoopDenied += () =>
        {
            if (!isDebug) return;
            Debug.Log("Loop Denied");
        };
        
        _deckSystem = deckSystem;
        
        OnDPSFinished.AddListener(HandleDraw);
        _loopTimer.LoopCondition.Add(IsDrawPossible);
    }

    private void OnDPSChanged(StatSystem.StatType type, int old, int newV)
    {
        if (type != StatSystem.StatType.DrawPerSecond) return;
        _currentDPS = startDPS + newV;
        
        slider.SetMaxValue(DrawPeriod);
        slider.SetCurrentValue(0f);
        _loopTimer.Start(DrawPeriod);
    }

    private bool IsDrawPossible()
    {
        return _deckSystem.CanDraw();
    }

    private void HandleDraw()
    {
        _deckSystem.Draw();
    }

    [Button]
    public void StartDPS()
    {
        if (!Application.IsPlaying(this)) return;
        
        _loopTimer.Start();
    }

    [Button]
    public void StopDPS()
    {
        if (!Application.IsPlaying(this)) return;
        
        _loopTimer.Stop();
    }

    [Button]
    public void PauseDPS()
    {
        if (!Application.IsPlaying(this)) return;
        
        _loopTimer.Pause();
    }
    
    [Button]
    public void ResumeDPS()
    {
        if (!Application.IsPlaying(this)) return;
        
        _loopTimer.Resume();
    }
}
