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
    private Vector2 offset;
    private Vector2 _mouseScreenPosition;
    private Vector2 _oldPosition;

    private bool _isDragging = false;

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
        inputReader.Interact += () => { };
    }

    private void Update()
    {
        if (_isDragging) HandleDrag();
    }

    private void HandleDrag()
    {
        var localPoint = GetLocalCoordsFromMouseScreenPosition();
        _oldPosition = _rectTransform.anchoredPosition;
        _rectTransform.anchoredPosition = localPoint + offset;

        if (_oldPosition != null)
            cardVisual.SetVelocity(_rectTransform.anchoredPosition - _oldPosition);
    }

    private Vector2 GetLocalCoordsFromMouseScreenPosition()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rectTransform.parent as RectTransform,
            _mouseScreenPosition,
            canvas.worldCamera,
            out var localPoint
        );
        return localPoint;
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

    #region UI Events Implementation

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        BeginDragEvent?.Invoke(this);

        var mousePos = GetLocalCoordsFromMouseScreenPosition();
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