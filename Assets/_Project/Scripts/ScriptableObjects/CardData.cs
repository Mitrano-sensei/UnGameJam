using System;
using System.Collections.Generic;
using SerializeReferenceEditor;
using UnityEngine;
using Utilities;

[CreateAssetMenu(menuName = "Cards/CardData")]
[Serializable]
public class CardData : BuyableItem
{
    [SerializeField] private CardPreview cardPreviewPrefab;
    
    [SerializeField] private string cardName;
    [SerializeField] private Sprite cardImage;
    
    public string CardName => cardName;
    public Sprite CardImage => cardImage;
    
    [TextArea] public string CardDescription;
    
    [SerializeField, SerializeReference]
    [SR]
    public List<GameAction> Effects;
    
    public override APreview GeneratePreview()
    {
        var preview = Instantiate(cardPreviewPrefab);
        preview.SetCardData(this);
        preview.SpawnAnimation();
        preview.AddClickEvent(() => BuyItem(preview));
        
        return preview;
    }
    
    private void BuyItem(APreview preview)
    {
        if (!preview.CanBuy) return;
        
        var shopSystem = Registry<ShopSystem>.GetFirst();
        shopSystem.BuyCard(preview);
    }
}