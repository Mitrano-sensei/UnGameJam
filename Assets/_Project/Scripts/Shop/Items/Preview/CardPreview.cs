using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class CardPreview : APreview
{
    [Header("References")]
    [SerializeField] private Image cardPreviewImage;
    [SerializeField] private SimpleInteractionHandler simpleInteractionHandlerObject;
    [SerializeField] private DescriptionHolder descriptionHolder;
    
    [Header("Hover Scale")]
    [SerializeField] private Transform transformToScale;
    [SerializeField] private float scaleFactor;
    [SerializeField] private TweenSettings hoverTweenSettings;
    [SerializeField] private TweenSettings hoverDescriptionSettings;
    
    [Header("Settings")]
    [SerializeField] private TweenSettings spawnTweenSettings;
    [SerializeField] private TweenSettings destroyTweenSettings;
    
    public void SetCardData(CardData cardData)
    {
        cardPreviewImage.sprite = cardData.CardImage;
        descriptionHolder.SetDescription(cardData.CardDescription);
        
        InitializeInteractions();
    }
    
    private void InitializeInteractions()
    {
        simpleInteractionHandlerObject.OnHoverEnter.AddListener(OnHoverEnterAction);
        simpleInteractionHandlerObject.OnHoverExit.AddListener(OnHoverExitAction);
    }
    
    private void OnHoverEnterAction()
    {
        descriptionHolder.gameObject.SetActive(true);
        
        Tween.Scale(transformToScale, new TweenSettings<float>(scaleFactor, hoverTweenSettings));
        Tween.Scale(descriptionHolder.transform, new TweenSettings<float>(startValue: 0f, endValue: 1f, hoverDescriptionSettings));
    }
    
    private void OnClickAction()
    {
        onClick?.Invoke();
    }
    
    private void OnHoverExitAction()
    {
        descriptionHolder.gameObject.SetActive(false);
        
        Tween.Scale(transformToScale, new TweenSettings<float>(1f, hoverTweenSettings));
        Tween.Scale(descriptionHolder.transform, new TweenSettings<float>(startValue: 1f, endValue: 0f, hoverDescriptionSettings));
    }
    
    public override void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void SpawnAnimation()
    {
        transform.localScale = Vector3.zero;
        Tween.Scale(transform, new TweenSettings<float>(1f, spawnTweenSettings));
    }
}