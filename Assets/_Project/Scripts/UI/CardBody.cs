using PrimeTween;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;

public class CardBody : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler,
    IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    [Header("References")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private InputReader inputReader;
    [SerializeField] private CardVisual cardVisual;
    private Image _image;
    private RectTransform _rectTransform;

    public RectTransform RectTransform => _rectTransform;

    [Header("Dragging")]
    [SerializeField] private float maxSpeed = 10f;
    private Vector2 offset;
    private Vector2 _mouseScreenPosition;
    private Vector2 _oldPosition;

    private bool _isDragging;

    [Header("Selection")]
    private bool _isSelected;

    [Header("Events")]
    [HideInInspector] public UnityEvent<CardBody> PointerEnterEvent;
    [HideInInspector] public UnityEvent<CardBody> PointerExitEvent;
    [HideInInspector] public UnityEvent<CardBody, bool> PointerUpEvent;
    [HideInInspector] public UnityEvent<CardBody> PointerDownEvent;
    [HideInInspector] public UnityEvent<CardBody> BeginDragEvent;
    [HideInInspector] public UnityEvent<CardBody> EndDragEvent;
    [HideInInspector] public UnityEvent<CardBody, bool> SelectEvent;

    public int ParentIndex => transform.parent.GetSiblingIndex();

    private void Start()
    {
        if (canvas == null)
            canvas = Registry<MainUICanvas>.GetFirst().GetComponent<Canvas>();
        _image = this.GetComponentOrException<Image>();
        _rectTransform = GetComponent<RectTransform>();

        inputReader.Point += (p, isMouse) => _mouseScreenPosition = isMouse ? p : Vector2.zero;
    }

    private void Update()
    {
        if (_isDragging) HandleDrag();
    }

    private void HandleDrag()
    {
        var localPoint = UIHelpers.GetLocalCoordsFromMouseScreenPosition(_rectTransform, _mouseScreenPosition, canvas);
        _oldPosition = _rectTransform.anchoredPosition;
        var targetPosition = localPoint + offset;
        
        if (Vector2.Distance(_oldPosition, targetPosition) > maxSpeed)
            targetPosition = _oldPosition + (targetPosition - _oldPosition).normalized * maxSpeed;
        
        _rectTransform.anchoredPosition = targetPosition;

        if (_oldPosition != null)
            cardVisual.AnimateVelocity(_rectTransform.anchoredPosition - _oldPosition);
    }

    public void ReturnToOrigin(bool tweenCardReturn, float duration = .5f)
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

    public void SetCardData(CardData exampleCardData)
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

        cardVisual.Initialize(this, exampleCardData);
    }

    public void SetSelected(bool isSelected)
    {
        if (isSelected == _isSelected) return;
        
        _isSelected = isSelected;
        SelectEvent?.Invoke(this, isSelected);
    }

    #region UI Events Implementation

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        BeginDragEvent?.Invoke(this);

        var mousePos = UIHelpers.GetLocalCoordsFromMouseScreenPosition(_rectTransform, _mouseScreenPosition, canvas);
        offset = _rectTransform.anchoredPosition - mousePos;
        _isDragging = true;

        canvas.GetComponent<GraphicRaycaster>().enabled = false;
        _image.raycastTarget = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EndDragEvent?.Invoke(this);
        _isDragging = false;

        canvas.GetComponent<GraphicRaycaster>().enabled = true;
        _image.raycastTarget = true;
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