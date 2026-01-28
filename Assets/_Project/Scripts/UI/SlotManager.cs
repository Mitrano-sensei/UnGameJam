using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Math = System.Math;

public class SlotManager : MonoBehaviour
{
    [Header("References")]
    private CardBody _draggedCardBody;

    [SerializeField] private CardSlot slotPrefab;
    [SerializeField] private List<CardData> exampleCardsData; // TODO : Remove that
    [SerializeField] private InputReader inputReader;
    private RectTransform _rectTransform;

    [Header("Spawn Settings")]
    [SerializeField] private int cardsToSpawn = 7;
    public List<CardBody> cards;

    [Header("Selection")]
    [SerializeField, Range(0f, 1f)] private float movementThreshhold = .1f;
    [SerializeField, Range(0f, .3f)] private float minimumTimeBetweenSelection = .15f;
    private CardBody _selectedCardBody;
    private float _lastSelectionTime;
    

    
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

        inputReader.Move += OnMoveInput;
    }
    
    private void Update()
    {
        if (!_draggedCardBody || _isCrossing)
            return;

        for (int i = 0; i < cards.Count; i++)
        {
            bool isRight = _draggedCardBody.transform.position.x > cards[i].transform.position.x;
            bool isLeft = _draggedCardBody.transform.position.x < cards[i].transform.position.x;
            bool isSupposedRight = _draggedCardBody.ParentIndex > cards[i].ParentIndex;
            bool isSupposedLeft = _draggedCardBody.ParentIndex < cards[i].ParentIndex;

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

        Transform draggedParent = _draggedCardBody.transform.parent;
        Transform crossedParent = cards[i].transform.parent;

        _draggedCardBody.transform.SetParent(crossedParent);
        cards[i].transform.SetParent(draggedParent);
        cards[i].ReturnToOrigin(tweenCardReturn, .2f);

        _isCrossing = false;
        
        // TODO : Visuals
    }

    #region Card Events

    private void CardPointerEnter(CardBody cardBody)
    {
        _selectedCardBody = cardBody;
    }

    private void CardPointerExit(CardBody cardBody)
    {
        _selectedCardBody = null;
    }

    private void CardBeginDrag(CardBody cardBody)
    {
        _draggedCardBody = cardBody;
    }

    private void CardEndDrag(CardBody cardBody)
    {
        if (_draggedCardBody == null)
            return;

        // Return the card to its original position
        _draggedCardBody.ReturnToOrigin(tweenCardReturn);

        _rectTransform.sizeDelta += Vector2.right;
        _rectTransform.sizeDelta -= Vector2.right;

        _draggedCardBody = null;
    }
    
    private void OnMoveInput(Vector2 movement)
    {
        if (Mathf.Abs(movement.x) < movementThreshhold) return;
        if (Time.time - _lastSelectionTime < minimumTimeBetweenSelection) return;

        _lastSelectionTime = Time.time;
        int currentSelectedIndex = _selectedCardBody == null ? -1 : _selectedCardBody.ParentIndex;

        if (currentSelectedIndex == -1)
        {
            SelectCard(movement.x > 0 ? 0 : cards.Count - 1);
        }
        
        int direction = movement.x > 0 ? 1 : -1;
        var newSelectedIndex = MathMod(currentSelectedIndex + direction, cards.Count);
        SelectCard(newSelectedIndex);
    }
    
    private void SelectCard(int index)
    {
        _selectedCardBody = cards[index];
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].SetSelected(i == index);
        }
    }
    
    /**
     * Calculates the modulo of two integers, the mathematical way (instead of strange C# % operator).
     */
    private static int MathMod(int a, int b) {
        return (Math.Abs(a * b) + a) % b;
    }


    #endregion
}