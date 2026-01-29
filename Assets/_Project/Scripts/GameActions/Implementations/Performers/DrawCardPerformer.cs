using System.Collections;
using UnityEngine;

public class DrawCardPerformer : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<DrawCardGA>(DrawCardPerform);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<DrawCardGA>();
    }

    private IEnumerator DrawCardPerform(DrawCardGA drawCardGA)
    {
        // TODO: Impl
        Debug.Log($"Draw {drawCardGA.Amount} Card");
        yield return null;
    }
}