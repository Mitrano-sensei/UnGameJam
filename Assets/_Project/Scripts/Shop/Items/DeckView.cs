using System.Collections.Generic;
using EditorAttributes;
using PrimeTween;
using UnityEngine;
using Utilities;

public class DeckView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup deckViewPanel;
    [SerializeField] private Transform contentParent;

    [Header("Settings")]
    [SerializeField] private float fadeInTime = 0.3f;
    [SerializeField] private float fadeOutTime = 0.3f;


    private readonly List<APreview> _previewRefs = new();

    public void FadeIn() {
        deckViewPanel.alpha = 0;
        Tween.Custom(startValue: 0f, endValue: 1f, duration: fadeInTime, f => deckViewPanel.alpha = f, Ease.OutSine);
        deckViewPanel.blocksRaycasts = true;

        InitializeCardPreviews();
    }

    public void FadeOut() {
        deckViewPanel.alpha = 1;
        Tween.Custom(startValue: 1f, endValue: 0f, duration: fadeOutTime, f => deckViewPanel.alpha = f, Ease.OutSine);
        deckViewPanel.blocksRaycasts = false;

        RemoveCardPreviews();
    }

    private void RemoveCardPreviews()
    {
        foreach (APreview preview in _previewRefs)
        {
            preview.DestroySelf();
        }
        
        _previewRefs.Clear();
    }

    private void InitializeCardPreviews()
    {
        var deckSystem = Registry<DeckSystem>.GetFirst();
        if (!deckSystem)
        {
            Debug.LogError("No Deck System registered");
            return;
        }
        
        foreach (var card in deckSystem.GetFullDeck())
        {
            APreview preview = card.GeneratePreview();
            preview.SetCanBuy(false);
            preview.transform.SetParent(contentParent, false);
            
            _previewRefs.Add(preview);
        }
    }
}
