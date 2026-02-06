using System.Collections.Generic;
using System.Linq;
using EditorAttributes;
using UnityEngine;
using Utilities;
using Math = System.Math;

public class HandManager : MonoBehaviour
{
    [Header("References")]
    private CardBody _draggedCardBody;

    [SerializeField, Required] private CardSlot slotPrefab;
    [SerializeField, Required] private InputReader inputReader;
    
    private RectTransform _rectTransform;
    [SerializeField, ReadOnly] private readonly List<CardBody> _currentHand = new();

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
        inputReader.Move += OnMoveInput;
        inputReader.Interact += OnInteractInput;
    }

    private void Update()
    {
        if (!_draggedCardBody || _isCrossing)
            return;

        foreach (var card in _currentHand.OrderBy(c => c.CardIndex))
        {
            bool isRight = _draggedCardBody.transform.position.x > card.transform.position.x;
            bool isLeft = _draggedCardBody.transform.position.x < card.transform.position.x;
            bool isSupposedRight = _draggedCardBody.CardIndex > card.CardIndex;
            bool isSupposedLeft = _draggedCardBody.CardIndex < card.CardIndex;
            
            if (isRight && isSupposedLeft)
            {
                SwapDraggedCard(card.CardIndex);
            }
            else if (isLeft && isSupposedRight)
            {
                SwapDraggedCard(card.CardIndex);
            }
        }
    }

    public void AddCardToHand(CardData cardData)
    {
        CardSlot slot = Instantiate(slotPrefab, transform);
        CardBody card = slot.GetComponentInChildren<CardBody>();
        _currentHand.Add(card);
        
        InitializeCard(card, cardData, _currentHand.Count - 1);
    }

    private void InitializeCard(CardBody card, CardData cardData, int index)
    {
        card.PointerEnterEvent.AddListener(CardPointerEnter);
        card.PointerExitEvent.AddListener(CardPointerExit);
        card.BeginDragEvent.AddListener(CardBeginDrag);
        card.EndDragEvent.AddListener(CardEndDrag);

        card.SetCardData(cardData);
        card.SetCardIndex(index);
    }

    void SwapDraggedCard(int i)
    {
        _isCrossing = true;
        
        var crossedCard = _currentHand.Find(c => c.CardIndex == i);

        Transform draggedParent = _draggedCardBody.transform.parent;
        Transform crossedParent = crossedCard.transform.parent;
        
        int fromIndex = _draggedCardBody.CardIndex;
        int toIndex = crossedCard.CardIndex;
        
        _draggedCardBody.transform.SetParent(crossedParent);
        crossedCard.transform.SetParent(draggedParent);
        crossedCard.ReturnToOrigin(tweenCardReturn, .2f);
        
        _draggedCardBody.OnChangeParent(draggedParent, crossedParent);
        
        _draggedCardBody.SetCardIndex(toIndex);
        crossedCard.SetCardIndex(fromIndex);

        _isCrossing = false;
    }

    #region Card Events

    private void CardPointerEnter(CardBody cardBody)
    {
    }

    private void CardPointerExit(CardBody cardBody)
    {
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
        int currentSelectedIndex = _selectedCardBody == null ? -1 : _selectedCardBody.CardIndex;

        if (currentSelectedIndex == -1)
        {
            SelectCard(movement.x > 0 ? 0 : _currentHand.Count - 1);
        }
        
        int direction = movement.x > 0 ? 1 : -1;
        var newSelectedIndex = MathMod(currentSelectedIndex + direction, _currentHand.Count);
        SelectCard(newSelectedIndex);
    }

    private void SelectCard(int index)
    {
        _selectedCardBody = _currentHand.Find(c => c.CardIndex == index);
        foreach (var card in _currentHand)
        {
            int cardIndex = card.CardIndex;
            card.SetSelected(cardIndex == index);
        }
    }

    private void OnInteractInput()
    {
        if (_selectedCardBody == null) return;
        _selectedCardBody.EffectHandler.PerformEffects();
    }

    /**
     * Calculates the modulo of two integers, the mathematical way (instead of strange C# % operator).
     */
    private static int MathMod(int a, int b) {
        return (Math.Abs(a * b) + a) % b;
    }

    #endregion
}