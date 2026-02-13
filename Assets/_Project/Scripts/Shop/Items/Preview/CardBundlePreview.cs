using System.Collections;
using EditorAttributes;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utilities;

public class CardBundlePreview : APreview
{
    [Header("References")]
    [SerializeField] private Image previewImageCard1;
    [SerializeField] private Image previewImageCard2;
    [SerializeField] private DescriptionHolder descriptionHolder1;
    [SerializeField] private DescriptionHolder descriptionHolder2;
    [SerializeField] private SimpleInteractionHandler simpleInteractionHandlerObject;
    [SerializeField] private TextMeshProUGUI priceText;

    [Header("Hover Scale")]
    [SerializeField] private Transform transformToScale;
    [SerializeField] private float scaleFactor;
    [SerializeField] private TweenSettings hoverTweenSettings;
    [SerializeField] private TweenSettings hoverDescriptionSettings;

    [Header("Settings")]
    [SerializeField] private TweenSettings spawnTweenSettings;
    [SerializeField] private TweenSettings destroyTweenSettings;

    public void SetCardBundle(CardBundle cardBundle)
    {
        // _cardBundle = cardBundle;

        previewImageCard1.sprite = cardBundle.Content[0].CardImage;
        previewImageCard2.sprite = cardBundle.Content[0].CardImage;

        priceText.text = $"{Registry<ShopSystem>.GetFirst().CardsPrice.ToString()}g";
        descriptionHolder1.SetDescription(cardBundle.Content[0].CardDescription);
        descriptionHolder2.SetDescription(cardBundle.Content[1].CardDescription);

        descriptionHolder1.gameObject.SetActive(false);
        descriptionHolder2.gameObject.SetActive(false);

        InitializeInteractions();
    }

    private void InitializeInteractions()
    {
        simpleInteractionHandlerObject.OnHoverEnter.AddListener(OnHoverEnterAction);
        simpleInteractionHandlerObject.OnHoverExit.AddListener(OnHoverExitAction);
        simpleInteractionHandlerObject.OnClick.AddListener(OnClickAction);
    }

    private void OnClickAction()
    {
        onClick?.Invoke();
    }

    private void OnHoverEnterAction()
    {
        // Card
        Tween.Scale(transformToScale, new TweenSettings<float>(scaleFactor, hoverTweenSettings));

        // Description
        descriptionHolder1.gameObject.SetActive(true);
        descriptionHolder2.gameObject.SetActive(true);
        Sequence.Create()
            .Chain(Tween.Scale(descriptionHolder1.transform, new TweenSettings<float>(startValue: 0f, endValue: 1f, hoverDescriptionSettings)))
            .Group(Tween.Scale(descriptionHolder2.transform, new TweenSettings<float>(startValue: 0f, endValue: 1f, hoverDescriptionSettings)));
    }

    private void OnHoverExitAction()
    {
        // Card
        Tween.Scale(transformToScale, new TweenSettings<float>(1f, hoverTweenSettings));

        // Description
        Sequence.Create()
            .Chain(Tween.Scale(descriptionHolder1.transform, new TweenSettings<float>(startValue: 1f, endValue: 0f, hoverDescriptionSettings)))
            .Group(Tween.Scale(descriptionHolder2.transform, new TweenSettings<float>(startValue: 1f, endValue: 0f, hoverDescriptionSettings)))
            .OnComplete(() =>
            {
                descriptionHolder1.gameObject.SetActive(false);
                descriptionHolder2.gameObject.SetActive(false);
            });
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