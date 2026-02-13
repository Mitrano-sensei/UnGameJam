using System;
using EditorAttributes;
using PrimeTween;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utilities;

public class CardBody : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler,
    IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    [Header("References")]
    [SerializeField, Required] private InputReader inputReader;
    [SerializeField, Required] private CardVisual cardVisual;
    [SerializeField, Required] private CardEffectHandler cardEffectHandler;
    private Canvas _canvas;
    private Image _image;
    private RectTransform _rectTransform;
    private RectTransform _canvasRectTransform;
    private CardData _cardData;
    private CanvasScaler _scaler;
    private DeckSystem _deckSystem;

    public RectTransform RectTransform => _rectTransform;
    public CardEffectHandler EffectHandler => cardEffectHandler;

    [Header("Dragging")]
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float yDeltaBeforePlayableScreenSpace = 100f;
    private Vector2 offset;
    private Vector2 _mouseScreenPosition;
    private Vector2 _oldPosition;

    private bool _isDragging;

    [Header("Selection")]
    private bool _isSelected;
    [SerializeField, ReadOnly] private int _cardIndex;

    public int CardIndex => _cardIndex;

    [Header("Events")]
    [HideInInspector] public UnityEvent<CardBody> PointerEnterEvent;
    [HideInInspector] public UnityEvent<CardBody> PointerExitEvent;
    [HideInInspector] public UnityEvent<CardBody, bool> PointerUpEvent;
    [HideInInspector] public UnityEvent<CardBody> PointerDownEvent;
    [HideInInspector] public UnityEvent<CardBody> BeginDragEvent;
    [HideInInspector] public UnityEvent<CardBody, bool> EndDragEvent; // Bool to check if the card was played
    [HideInInspector] public UnityEvent<CardBody, bool> SelectEvent;
    [HideInInspector] public UnityEvent<CardBody, Vector2> OnMoveEvent;

    [Header("Draw")]
    [SerializeField] private TweenSettings drawScaleTweenSettings;
    
    [Header("Debug")]
    [ShowInInspector, HideInEditMode] private Vector2 Position
    {
        get { return _rectTransform != null ? _rectTransform.anchoredPosition : Vector2.zero; }
    }
    [ShowInInspector, HideInEditMode] private Vector2 MousePos => _mouseScreenPosition;
    [ShowInInspector, HideInEditMode] private Vector2 Offset => offset;

    public int ParentIndex => transform.parent.GetSiblingIndex();

    private void Awake()
    {
        _image = this.GetComponentOrException<Image>();
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        _canvas = Registry<MainUICanvas>.GetFirst().GetComponent<Canvas>();

        inputReader.Point += (p, isMouse) => _mouseScreenPosition = isMouse ? p : Vector2.zero;
    }

    private void OnDisable()
    {
        Tween.StopAll(transform);
    }

    private void Update()
    {
        if (_isDragging) HandleDrag();
    }

    private void HandleDrag()
    {
        _scaler ??= _canvas.GetComponent<CanvasScaler>();
        bool isScreenSpaceOverlay = _canvas.renderMode == RenderMode.ScreenSpaceOverlay;
        var localMousePoint = !isScreenSpaceOverlay 
            ? UIHelpers.GetLocalCoordsFromMouseScreenPosition(_rectTransform, _mouseScreenPosition, _canvas) 
            : new(_mouseScreenPosition.x, _mouseScreenPosition.y);
        
        _oldPosition = _rectTransform.anchoredPosition;
        var targetPosition = localMousePoint + offset;

        if (isScreenSpaceOverlay)
            targetPosition = new(targetPosition.x * _scaler.referenceResolution.x / Screen.width, targetPosition.y * _scaler.referenceResolution.y / Screen.height);
        
        if (Vector2.Distance(_oldPosition, targetPosition) > maxSpeed)
            targetPosition = _oldPosition + (targetPosition - _oldPosition).normalized * maxSpeed;


        _rectTransform.anchoredPosition = targetPosition;

        if (_oldPosition != null)
            OnMoveEvent?.Invoke(this, _rectTransform.anchoredPosition - _oldPosition);
    }

    public void ReturnToOrigin(bool tweenCardReturn, float duration = .5f, bool isDraw = false)
    {
        if (_rectTransform.anchoredPosition == Vector2.zero)
            return;

        if (!tweenCardReturn)
        {
            _rectTransform.anchoredPosition = Vector2.zero;
            return;
        }
        
        Tween.UIAnchoredPosition(_rectTransform, Vector2.zero, duration);
    }

    public void SetCardData(CardData cardData)
    {
        if (cardVisual == null)
        {
            cardVisual = GetComponentInChildren<CardVisual>();
            if (!cardVisual)
            {
                Debug.LogError("No Card Visual for Card");
                return;
            }
        }

        if (cardEffectHandler == null)
        {
            cardEffectHandler = GetComponent<CardEffectHandler>();
            if (!cardEffectHandler)
            {
                Debug.LogError("No Card Effect Handler for Card");
                return;
            }
        }

        _cardData = cardData;
        cardVisual.Initialize(this, cardData);
        cardEffectHandler.Initialize(this, cardData);
    }

    public void SetSelected(bool isSelected)
    {
        if (isSelected == _isSelected) return;

        _isSelected = isSelected;
        SelectEvent?.Invoke(this, isSelected);
    }

    public void SetCardIndex(int index)
    {
        this._cardIndex = index;
    }

    /**
     * Call this when the parent of the card body changes.
     * Resolves an offset issue when the parent changes on ScreenSpace Overlay mode.
     */
    public void OnChangeParent(Transform oldParent, Transform newParent)
    {
        if (!_isDragging) return;
        if (_canvas.renderMode != RenderMode.ScreenSpaceOverlay) return;

        Vector2 oldParentPosition = oldParent.position;
        Vector2 newParentPosition = newParent.position;
        offset -= (newParentPosition - oldParentPosition);
    }

    #region UI Events Implementation

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        BeginDragEvent?.Invoke(this);

        var mousePos = _canvas.renderMode is RenderMode.WorldSpace or RenderMode.ScreenSpaceCamera
            ? UIHelpers.GetLocalCoordsFromMouseScreenPosition(_rectTransform, _mouseScreenPosition, _canvas)
            : _mouseScreenPosition;
        offset = _rectTransform.anchoredPosition - mousePos;
        _isDragging = true;

        _canvas.GetComponent<GraphicRaycaster>().enabled = false;
        _image.raycastTarget = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EndDragEvent?.Invoke(this, _rectTransform.anchoredPosition.y >= yDeltaBeforePlayableScreenSpace);
        _isDragging = false;

        _canvas.GetComponent<GraphicRaycaster>().enabled = true;
        _image.raycastTarget = true;
    }
    
    public void ReturnToDeck()
    {
        Destroy(transform.parent.gameObject);
    }

    public void GoToTrash()
    {
        var parentCardSlot = transform.parent.GetComponent<CardSlot>();
        if (!parentCardSlot)
        {
            Debug.LogError("No Card Slot on Card Body");
            return;
        }
        parentCardSlot.GoToTrash();
        
        _deckSystem ??= Registry<DeckSystem>.GetFirst();
        _deckSystem.ReturnCard(_cardData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEnterEvent?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PointerExitEvent?.Invoke(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PointerUpEvent?.Invoke(this, eventData.button == PointerEventData.InputButton.Left);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PointerDownEvent?.Invoke(this);
    }

    #endregion
}