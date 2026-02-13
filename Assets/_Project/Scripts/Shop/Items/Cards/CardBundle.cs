using System.Collections;
using System.Collections.Generic;
using EditorAttributes;
using PrimeTween;
using UnityEngine;
using Utilities;

[CreateAssetMenu(menuName = "Cards/CardBundle", fileName = "CardBundle", order = 0)]
public class CardBundle : BuyableItem
{
    [Header("Settings")]
    [SerializeField, Required] private CardBundlePreview previewPrefab;
    
    [Header("Content")]
    [SerializeField] private List<CardData> _cards;
    

    public List<CardData> Content => _cards;

    public override APreview GeneratePreview()
    {
        var preview = Instantiate(previewPrefab);
        preview.SetCardBundle(this);
        preview.SpawnAnimation();
        preview.AddClickEvent(() => BuyItem(preview));
        
        return preview;
    }

    private void RemoveFromPreview(CardBundlePreview preview)
    {
        preview.DestroySelf();
    }

    private void BuyItem(CardBundlePreview preview)
    {
        var shopSystem = Registry<ShopSystem>.GetFirst();

        if (!shopSystem.CanBuyCardBundle()) return;

        shopSystem.BuyCardBundle(this);
        RemoveFromPreview(preview);
    }
}