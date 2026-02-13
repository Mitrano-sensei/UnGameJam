using System.Collections;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utilities;

public class RelicPreview : APreview
{
    [Header("References")]
    [SerializeField] private Image relicPreviewImage;
    [SerializeField] private SimpleInteractionHandler simpleInteractionHandlerObject;
    [SerializeField] private TextMeshProUGUI relicPrice;
    [SerializeField] private DescriptionHolder descriptionHolder;

    [Header("Hover Scale")]
    [SerializeField] private Transform transformToScale;
    [SerializeField] private float scaleFactor;
    [SerializeField] private TweenSettings hoverTweenSettings;
    [SerializeField] private TweenSettings hoverDescriptionSettings;

    [Header("Settings")]
    [SerializeField] private TweenSettings spawnTweenSettings;
    [SerializeField] private TweenSettings destroyTweenSettings;

    public void SetRelicData(RelicData relicData)
    {
        // _cardBundle = cardBundle;
        relicPreviewImage.sprite = relicData.RelicImage;

        relicPrice.text = $"{Registry<ShopSystem>.GetFirst().RelicPrices.ToString()}g";
        descriptionHolder.SetDescription(relicData.Description);

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
        Tween.Scale(transformToScale, new TweenSettings<float>(scaleFactor, hoverTweenSettings));

        // Description
        descriptionHolder.gameObject.SetActive(true);
        Tween.Scale(descriptionHolder.transform, new TweenSettings<float>(startValue: 0f, endValue: 1f, hoverDescriptionSettings));
    }

    private void OnHoverExitAction()
    {
        Tween.Scale(transformToScale, new TweenSettings<float>(1f, hoverTweenSettings));

        // Description
        Tween.Scale(descriptionHolder.transform, new TweenSettings<float>(startValue: 1f, endValue: 0f, hoverDescriptionSettings))
            .OnComplete(() => descriptionHolder.gameObject.SetActive(false));
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