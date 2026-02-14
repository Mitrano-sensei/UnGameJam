using System.Collections;
using PrimeTween;
using TMPro;
using UnityEngine;

public class StartCombatAnimationOverlay : AnimationOverlay
{
    [Header("Start Combat References")]
    [SerializeField] private TextMeshProUGUI text;

    [Header("Start Combat Settings")]
    [SerializeField, Range(0, 10)] private int countdown = 3;
    [SerializeField] private TweenSettings countdownTweenSettings;
    
    public IEnumerator PlayAnimation(System.Action onEndAnimation = null)
    {
        if (countdown <= 0)
        {
            onEndAnimation?.Invoke();
            yield break;
        }
        
        yield return FadeIn();

        text.text = countdown.ToString();
        for (int i = countdown; i > 0; i--)
        {
            var sequence = Sequence
                .Create()
                .Chain(Tween.Scale(text.transform, new TweenSettings<float>(startValue: 1f, endValue: 0f, countdownTweenSettings)))
                .ChainCallback(() => text.text = i.ToString())
                .Chain(Tween.Scale(text.transform, new TweenSettings<float>(startValue: 0f, endValue: 1f, countdownTweenSettings)))
                .ChainDelay(1f - (countdownTweenSettings.duration * 2));
            yield return sequence.ToYieldInstruction();
        }

        yield return FadeOut();
        onEndAnimation?.Invoke();
    }
}