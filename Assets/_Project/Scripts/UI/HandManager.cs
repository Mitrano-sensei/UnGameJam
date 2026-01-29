using System.Collections.Generic;
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

        for (int i = 0; i < _currentHand.Count; i++)
        {
            bool isRight = _draggedCardBody.transform.position.x > _currentHand[i].transform.position.x;
            bool isLeft = _draggedCardBody.transform.position.x < _currentHand[i].transform.position.x;
            bool isSupposedRight = _draggedCardBody.ParentIndex > _currentHand[i].ParentIndex;
            bool isSupposedLeft = _draggedCardBody.ParentIndex < _currentHand[i].ParentIndex;

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

    public void AddCardToHand(CardData cardData)
    {
        CardSlot slot = Instantiate(slotPrefab, transform);
        CardBody card = slot.GetComponentInChildren<CardBody>();
        _currentHand.Add(card);
        
        InitializeCard(card, cardData);
    }

    private void InitializeCard(CardBody card, CardData cardData)
    {
        card.PointerEnterEvent.AddListener(CardPointerEnter);
        card.PointerExitEvent.AddListener(CardPointerExit);
        card.BeginDragEvent.AddListener(CardBeginDrag);
        card.EndDragEvent.AddListener(CardEndDrag);

        card.SetCardData(cardData);
    }

    void Swap(int i)
    {
        _isCrossing = true;

        Transform draggedParent = _draggedCardBody.transform.parent;
        Transform crossedParent = _currentHand[i].transform.parent;

        _draggedCardBody.transform.SetParent(crossedParent);
        _currentHand[i].transform.SetParent(draggedParent);
        _currentHand[i].ReturnToOrigin(tweenCardReturn, .2f);

        _isCrossing = false;
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
            SelectCard(movement.x > 0 ? 0 : _currentHand.Count - 1);
        }
        
        int direction = movement.x > 0 ? 1 : -1;
        var newSelectedIndex = MathMod(currentSelectedIndex + direction, _currentHand.Count);
        SelectCard(newSelectedIndex);
    }

    private void SelectCard(int index)
    {
        _selectedCardBody = _currentHand[index];
        for (int i = 0; i < _currentHand.Count; i++)
        {
            _currentHand[i].SetSelected(i == index);
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