using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;

public class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler,
    IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    [Header("References")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private InputReader inputReader;
    // private Camera _mainCamera;
    private Image _image;
    private RectTransform _rectTransform;
    // private RectTransform _canvasRect;
    // private RectTransform _slotRect;

    public RectTransform RectTransform => _rectTransform;

    [Header("Dragging")]
    [SerializeField] private float moveSpeedLimit = 50f;
    private Vector2 offset;
    private Vector2 _mouseScreenPosition;

    private bool _isDragging = false;
    
    [Header("Events")]
    [HideInInspector] public UnityEvent<Card> PointerEnterEvent;
    [HideInInspector] public UnityEvent<Card> PointerExitEvent;
    [HideInInspector] public UnityEvent<Card, bool> PointerUpEvent;
    [HideInInspector] public UnityEvent<Card> PointerDownEvent;
    [HideInInspector] public UnityEvent<Card> BeginDragEvent;
    [HideInInspector] public UnityEvent<Card> EndDragEvent;
    [HideInInspector] public UnityEvent<Card, bool> SelectEvent;


    private void Start()
    {
        canvas = this.GetComponentInParentIfNull(canvas);
        _image = this.GetComponentOrException<Image>();
        // _mainCamera = Camera.main;

        _rectTransform = GetComponent<RectTransform>();
        // _canvasRect = canvas.GetComponent<RectTransform>();
        // _slotRect = transform.parent.GetComponent<RectTransform>();

        inputReader.Point += (p, isMouse) => _mouseScreenPosition = isMouse ? p : Vector2.zero;
        inputReader.Interact += () => {};
    }

    private void Update()
    {
        if (_isDragging)
        {
            var localPoint = GetLocalCoordsFromMouseScreenPosition();
            _rectTransform.anchoredPosition = localPoint + offset;
        }
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

    // private Vector3 GetWorldPositionOnPlane(Camera myCamera, Vector3 screenPosition, float z)
    // {
    //     Ray ray = myCamera.ScreenPointToRay(screenPosition);
    //     Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, z));
    //     xy.Raycast(ray, out var distance);
    //     return ray.GetPoint(distance);
    // }

    #region UI Events Implementation

    public void OnDrag(PointerEventData eventData) {}

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