using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PreviewInteractionHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Header("Events")]
    [HideInInspector] public UnityEvent OnClick = new();
    [HideInInspector] public UnityEvent OnHoverEnter = new();
    [HideInInspector] public UnityEvent OnHoverExit = new();

    
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHoverEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnHoverExit?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnClick?.Invoke();
    }
}