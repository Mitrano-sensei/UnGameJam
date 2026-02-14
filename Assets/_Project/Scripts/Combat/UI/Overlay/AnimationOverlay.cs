using System.Collections;
using EditorAttributes;
using PrimeTween;
using TMPro;
using UnityEngine;

public class AnimationOverlay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Settings")]
    [SerializeField, Suffix("seconds")] private float fadeTime = .5f;
    
    protected IEnumerator FadeIn() => Tween.Custom(startValue: 0f, endValue: 1f, duration: fadeTime, f => canvasGroup.alpha = f, Ease.OutSine).ToYieldInstruction();
    protected IEnumerator FadeOut() => Tween.Custom(startValue: 1f, endValue: 0f, duration: fadeTime, f => canvasGroup.alpha = f, Ease.OutSine).ToYieldInstruction();
}