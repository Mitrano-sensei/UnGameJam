using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

public class SlotManager : MonoBehaviour
{
    [Header("References")]
    private CardBody draggedCardBody;
    private CardBody hoveredCardBody;

    [SerializeField] private CardSlot slotPrefab;
    [SerializeField] private List<CardData> exampleCardsData; // TODO : Remove that
    private RectTransform _rectTransform;

    [Header("Spawn Settings")]
    [SerializeField] private int cardsToSpawn = 7;
    public List<CardBody> cards;

    [Header("Misc")]
    [SerializeField] private bool tweenCardReturn = true;
    private bool _isCrossing;

    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        cards = new List<CardBody>();

        for (int i = 0; i < cardsToSpawn; i++)
        {
            CardSlot slot = Instantiate(slotPrefab, transform);
            cards.Add(slot.GetComponentInChildren<CardBody>());
        }

        foreach (CardBody card in cards)
        {
            card.PointerEnterEvent.AddListener(CardPointerEnter);
            card.PointerExitEvent.AddListener(CardPointerExit);
            card.BeginDragEvent.AddListener(CardBeginDrag);
            card.EndDragEvent.AddListener(CardEndDrag);

            card.SetCardData(exampleCardsData.GetRandom());
        }
    }

    private void Update()
    {
        if (!draggedCardBody || _isCrossing)
            return;

        for (int i = 0; i < cards.Count; i++)
        {
            bool isRight = draggedCardBody.transform.position.x > cards[i].transform.position.x;
            bool isLeft = draggedCardBody.transform.position.x < cards[i].transform.position.x;
            bool isSupposedRight = draggedCardBody.ParentIndex > cards[i].ParentIndex;
            bool isSupposedLeft = draggedCardBody.ParentIndex < cards[i].ParentIndex;

            if (isRight && isSupposedLeft)
            {
                Swap(i);
            }
            else if (isLeft && isSupposedRight)
            {
                Swap(i);
            }
        }
    }

    void Swap(int i)
    {
        _isCrossing = true;

        Transform draggedParent = draggedCardBody.transform.parent;
        Transform crossedParent = cards[i].transform.parent;

        draggedCardBody.transform.SetParent(crossedParent);
        cards[i].transform.SetParent(draggedParent);
        cards[i].ReturnToOrigin(tweenCardReturn, .2f);

        _isCrossing = false;
        
        // TODO : Visuals
    }

    #region Card Events

    private void CardPointerEnter(CardBody cardBody)
    {
        hoveredCardBody = cardBody;
    }

    private void CardPointerExit(CardBody cardBody)
    {
        hoveredCardBody = null;
    }

    private void CardBeginDrag(CardBody cardBody)
    {
        draggedCardBody = cardBody;
    }

    private void CardEndDrag(CardBody cardBody)
    {
        if (draggedCardBody == null)
            return;

        // Return the card to its original position
        draggedCardBody.ReturnToOrigin(tweenCardReturn);

        _rectTransform.sizeDelta += Vector2.right;
        _rectTransform.sizeDelta -= Vector2.right;

        draggedCardBody = null;
    }

    #endregion
}