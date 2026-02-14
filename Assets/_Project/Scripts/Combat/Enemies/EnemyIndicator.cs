using UnityEngine;
using Utilities;

public class EnemyIndicator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BOTWSlider timerSlider;
    
    private CountdownTimer _countDownTimer;

    private bool _isStarted = false;

    private void Update()
    {
        if (!_isStarted) return;
        _countDownTimer.Tick(Time.deltaTime);
        timerSlider.SetCurrentValue(_countDownTimer.GetCurrentTime());
    }

    public void SetTimer(float time, bool startTimer = false)
    {
        timerSlider.SetMaxValue(time);
        _countDownTimer = new(time);
        _countDownTimer.Start();
        _countDownTimer.OnTimerStop += () => Destroy(gameObject);
        _isStarted = startTimer;
    }

    public void StartTimer()
    {
        _isStarted = true;
    }
}