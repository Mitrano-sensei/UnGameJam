using System.Collections;
using EditorAttributes;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class CardBundlePreview : APreview
{
    [Header("References")]
    [SerializeField] private Image previewImageCard1;
    [SerializeField] private Image previewImageCard2;
    [SerializeField] private PreviewInteractionHandler previewInteractionHandlerObject;
    [SerializeField] private TextMeshProUGUI priceText;
    
    [Header("Hover Scale")]
    [SerializeField] private Transform transformToScale;
    [SerializeField] private float scaleFactor;
    [SerializeField] private TweenSettings hoverTweenSettings;
    
    [Header("Settings")]
    [SerializeField] private TweenSettings spawnTweenSettings;
    [SerializeField] private TweenSettings destroyTweenSettings;
    
    public void SetCardBundle(CardBundle cardBundle)
    {
        // _cardBundle = cardBundle;

        previewImageCard1.sprite = cardBundle.Content[0].CardImage;
        previewImageCard2.sprite = cardBundle.Content[0].CardImage;
        
        priceText.text = $"{Registry<ShopSystem>.GetFirst().CardsPrice.ToString()}g";
        
        InitializeInteractions();
    }

    private void InitializeInteractions()
    {
        previewInteractionHandlerObject.OnHoverEnter.AddListener(OnHoverEnterAction);
        previewInteractionHandlerObject.OnHoverExit.AddListener(OnHoverExitAction);
        previewInteractionHandlerObject.OnClick.AddListener(OnClickAction);
    }
    
    private void OnClickAction()
    {
        onClick?.Invoke();
    }
    
    private void OnHoverEnterAction()
    {
        Tween.Scale(transformToScale, new TweenSettings<float>(scaleFactor, hoverTweenSettings));
    }

    private void OnHoverExitAction()
    {
        Tween.Scale(transformToScale, new TweenSettings<float>(1f, hoverTweenSettings));
    }

    public override void DestroySelf()
    {
        StartCoroutine(ScaleThenDestroy());
    }

    private IEnumerator ScaleThenDestroy()
    {
        yield return Tween.Scale(transform, new TweenSettings<float>(0f, destroyTweenSettings)).ToYieldInstruction();
        yield return new WaitForSeconds(0.1f);
        Tween.StopAll(transform);
        Destroy(gameObject);
    }

    public void SpawnAnimation()
    {
        transform.localScale = Vector3.zero;
        Tween.Scale(transform, new TweenSettings<float>(1f, spawnTweenSettings));
    }
}