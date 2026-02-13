using System.Collections;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class RepairPreview : APreview
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private SimpleInteractionHandler simpleInteractionHandlerObject;
    [SerializeField] private Image image;
    [SerializeField] private DescriptionHolder descriptionHolder;

    [Header("Settings")]
    [SerializeField] private TweenSettings spawnTweenSettings;
    [SerializeField] private TweenSettings destroyTweenSettings;

    [Header("Hover Scale")]
    [SerializeField] private Transform transformToScale;
    [SerializeField] private float scaleFactor;
    [SerializeField] private TweenSettings hoverTweenSettings;
    [SerializeField] private TweenSettings hoverDescriptionSettings;

    public void SetPrice(int price) => _priceText.text = $"{price}g";
    
    public void SpawnAnimation()
    {
        transform.localScale = Vector3.zero;
        Tween.Scale(transform, new TweenSettings<float>(1f, spawnTweenSettings));
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

    public void Initialize(RepairItem repairItem)
    {
        image.sprite = repairItem.Image;
        descriptionHolder.SetDescription(repairItem.Description);
        SetPrice(Registry<ShopSystem>.GetFirst().RepairPrice);
        
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
}