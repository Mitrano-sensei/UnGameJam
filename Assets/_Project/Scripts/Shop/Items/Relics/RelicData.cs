using System;
using PrimeTween;
using SerializeReferenceEditor;
using UnityEngine;
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
    
    public string RelicName => relicName;
    public string Description => description;
    

    public Sprite RelicImage => relicImage;
    
    public abstract void Apply();
    public abstract void Remove();

    public override APreview GeneratePreview()
    {
        var preview = Instantiate(previewPrefab);
        preview.SetRelicData(this);
        preview.SpawnAnimation();
        preview.AddClickEvent(() => BuyItem(preview));
        
        return preview;
    }

    public void RemoveFromPreview(RelicPreview preview)
    {
        preview.DestroySelf();
    }
    
    private void BuyItem(RelicPreview preview)
    {
        var shopSystem = Registry<ShopSystem>.GetFirst();

        if (!shopSystem.CanBuyRelic()) return;
        
        shopSystem.BuyRelic(this);
        RemoveFromPreview(preview);
    }
}