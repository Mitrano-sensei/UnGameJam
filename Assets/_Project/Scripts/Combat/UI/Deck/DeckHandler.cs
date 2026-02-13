using EditorAttributes;
using PrimeTween;
using UnityEngine;
using Utilities;

public class DeckHandler : MonoBehaviour, ILoadable
{
    [Header("References")]
    [SerializeField] private SimpleInteractionHandler interactionHandler;
    [SerializeField] private Transform _returnPosition;
    [SerializeField] private DPSMeter dpsMeter;
    [SerializeField, Required] private DeckView deckView;

    [Header("Setting")]
    [SerializeField] private TweenSettings hoverTweenSettings;

    public DPSMeter DPSMeter => dpsMeter;

    public Transform ReturnPosition => _returnPosition;

    public void LoadWithScene()
    {
        Registry<DeckHandler>.RegisterSingletonOrLogError(this);

        interactionHandler.OnHoverEnter.AddListener(OnHoverEnterAction);
        interactionHandler.OnHoverExit.AddListener(OnHoverExitAction);
        interactionHandler.OnClick.AddListener(OnClickAction);

        if (dpsMeter != null) dpsMeter.Initialize(Registry<DeckSystem>.GetFirst());
    }

    public void UnLoadWithScene()
    {
        Registry<DeckHandler>.TryRemove(this);

        interactionHandler.OnHoverEnter.RemoveListener(OnHoverEnterAction);
        interactionHandler.OnHoverExit.RemoveListener(OnHoverExitAction);
        interactionHandler.OnClick.RemoveListener(OnClickAction);
    }

    private void OnHoverEnterAction()
    {
        Tween.Scale(transform, new TweenSettings<float>(startValue: 1f, endValue: 1.1f, hoverTweenSettings));
    }

    private void OnHoverExitAction()
    {
        Tween.Scale(transform, new TweenSettings<float>(startValue: 1.1f, endValue: 1f, hoverTweenSettings));
    }

    private void OnClickAction()
    {
        deckView.FadeIn();
    }
}