using System.Collections;
using UnityEngine;
using Utilities;

public class DrawCardPerformer : MonoBehaviour
{
    private DeckSystem _deckSystem;

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
        Debug.Log($"Draw {drawCardGA.Amount} Card");
        var amount = drawCardGA.Amount;
        _deckSystem ??= Registry<DeckSystem>.GetFirst();
        _deckSystem.Draw(amount);
        yield return null;
    }
}