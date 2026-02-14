using System;
using System.Collections;
using EditorAttributes;
using UnityEngine;

public class GameOverAnimationOverlay : AnimationOverlay
{
    [Header("Game over Settings")]
    [SerializeField, Suffix("seconds before Main Menu")] private float timeOfGameOver = 2f;

    public IEnumerator PlayAnimation(Action onEndAnimation = null)
    {
        yield return FadeIn();
        yield return new WaitForSeconds(timeOfGameOver);
        onEndAnimation?.Invoke();
    }
}