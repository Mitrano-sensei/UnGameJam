using System;
using PrimeTween;
using SerializeReferenceEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Utilities;

public abstract class RelicData : BuyableItem
{
    [Header("References")]
    [SerializeField] private RelicPreview previewPrefab;

    [Header("Data")]
    [SerializeField] private string relicName;
    [SerializeField] private string description;
    [SerializeField] private Sprite relicImage;

    [Header("Events")]
    [SerializeField] private UnityEvent onUseRelic;

    public UnityEvent OnUseRelic => onUseRelic;

    public string RelicName => relicName;
    public string Description => description;


    public Sprite RelicImage => relicImage;

    public abstract void Apply();
    public abstract void Remove();

    public override APreview GeneratePreview(bool forShop = true, bool spawnAnimation = true)
    {
        var preview = Instantiate(previewPrefab);
        preview.SetRelicData(this);
        preview.SetPriceTag(forShop);
        if (spawnAnimation) preview.SpawnAnimation();
        if (forShop) preview.AddClickEvent(() => BuyItem(preview));
        onUseRelic.AddListener(() =>
        {
            if (preview) preview.PlayUseAnimation();
        });

        return preview;
    }

    public void RemoveFromPreview(RelicPreview preview)
    {
        preview.DestroySelf();
    }

    private void BuyItem(RelicPreview preview)
    {
        var shopSystem = Registry<ShopSystem>.GetFirst();

        if (!shopSystem) return;
        if (!shopSystem.CanBuyRelic()) return;

        shopSystem.BuyRelic(this);
        RemoveFromPreview(preview);
    }
}