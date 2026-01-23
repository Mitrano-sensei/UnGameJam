using System;
using PrimeTween;
using UnityEngine;

public class CardSlot : MonoBehaviour
{
    private Card card;

    private void Start()
    {
        card = GetComponentInChildren<Card>();
        
        card.EndDragEvent.AddListener(OnEndDrag);
    }
    
    private void OnEndDrag(Card eCard)
    {
        // Use PrimeTween to tween the card back to its original position (anchoredPosition = Vector2.zero)
        Tween.UIAnchoredPosition(eCard.RectTransform, Vector2.zero, .5f);
    }

}
