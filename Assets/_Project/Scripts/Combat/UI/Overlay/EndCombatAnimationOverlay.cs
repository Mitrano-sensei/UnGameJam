using System.Collections;
using EditorAttributes;
using UnityEngine;

public class EndCombatAnimationOverlay : AnimationOverlay
{
    [Header("End combat settings")]
    [SerializeField, Suffix("seconds before shop")] private float timeToStayOnOverlay = 2f;
    
    public IEnumerator PlayAnimation(System.Action onEndAnimation = null)
    {
        yield return FadeIn();
        yield return new WaitForSeconds(timeToStayOnOverlay);
        onEndAnimation?.Invoke();
    }
    
}