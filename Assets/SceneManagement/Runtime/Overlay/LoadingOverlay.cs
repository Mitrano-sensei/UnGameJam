using System;
using System.Collections;
using EditorAttributes;
using UnityEngine;

public class LoadingOverlay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    
    [Header("Settings")]
    [SerializeField, Suffix("in seconds")] private float fadeInTime = 0.5f;
    [SerializeField, Suffix("in seconds")] private float fadeOutTime = 0.5f;
    
    public virtual IEnumerator FadeIn()
    {
        yield return FadeTo(1f, fadeInTime);
    }

    public virtual IEnumerator FadeOut()
    {
        yield return FadeTo(0f, fadeOutTime);
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        canvasGroup.alpha = targetAlpha;
    }
}