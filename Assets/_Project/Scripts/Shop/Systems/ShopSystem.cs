using System.Collections.Generic;
using System.Linq;
using EditorAttributes;
using UnityEngine;
using Utilities;

public class ShopSystem: MonoBehaviour, ILoadable
{
    [Header("References")]
    private MoneySystem _moneySystem;
    private RelicSystem _relicSystem;
    private DeckSystem _deckSystem;

    private List<RelicData> _availableRelics;
    private List<CardBundle> _availableCards;
    
    [Header("Shop Items")]
    [SerializeField] private List<RelicData> _possibleRelics;
    [SerializeField] private List<CardBundle> _possibleCards;
    
    [SerializeField] private List<Transform> _cardBundleSlots;
    [SerializeField] private List<Transform> _relicSlots;
    
    [Header("Prices")]
    [SerializeField] private int _relicPrices = 2;
    [SerializeField] private int _cardsPrice = 2;
    [SerializeField] private int _repairPrice = 1;
    [SerializeField] private int _removeCardPrice = 1;
    
    public int RelicPrices => _relicPrices;
    public int CardsPrice => _cardsPrice;
    public int RepairPrice => _repairPrice;
    public int RemoveCardPrice => _removeCardPrice;
    
    public void LoadWithScene()
    {
        Registry<ShopSystem>.RegisterSingletonOrLogError(this);
        
        _moneySystem = Registry<MoneySystem>.GetFirst();
        _deckSystem = Registry<DeckSystem>.GetFirst();
        _relicSystem = Registry<RelicSystem>.GetFirst();
        
        GenerateShopItems();
    }

    public void UnLoadWithScene()
    {
        Registry<ShopSystem>.TryRemove(this);
    }

    [Button]
    public void GenerateShopItems()
    {
        _availableCards = _possibleCards.Shuffle().GetRange(0, _cardBundleSlots.Count);
        _availableRelics = _possibleRelics.Shuffle().GetRange(0, _relicSlots.Count);

        for (var index = 0; index < _availableRelics.Count; index++)
        {
            var relic = _availableRelics[index];
            InitPreview(relic, index, true);
        }    
        

        for (var index = 0; index < _availableCards.Count; index++)
        {
            var card = _availableCards[index];
            InitPreview(card, index, false);
        }
    }

    private void InitPreview(BuyableItem buyableItem, int index, bool isRelic)
    {
        switch (isRelic)
        {
            case true when index >= _relicSlots.Count:
                Debug.LogError("Index out of bounds, skipping");
                return;
            case false when index >= _cardBundleSlots.Count:
                Debug.LogError("Index out of bounds, skipping");
                return;
        }
        
        var parentSlot = isRelic ? _relicSlots[index] : _cardBundleSlots[index];
        var preview = buyableItem.GeneratePreview();
        preview.transform.SetParent(parentSlot);
        preview.transform.localPosition = Vector3.zero;
    }

    public void BuyRelic(RelicData relic)
    {
        if (_moneySystem.Money < _relicPrices) return;
        if (!_availableRelics.Contains(relic))
        {
            Debug.LogError("Attempting to buy something that is not available");
            return;
        }
        
        _moneySystem.Money -= _relicPrices;
        _relicSystem.AddRelic(relic);
        
        _availableRelics.Remove(relic);
    }

    public void BuyCardBundle(CardBundle cardBundle)
    {
        if (_moneySystem.Money < _cardsPrice) return;
        if (!_availableCards.Contains(cardBundle))
        {
            Debug.LogError("Attempting to buy something that is not available");
            return;
        }
        
        _moneySystem.Money -= _cardsPrice;
        _deckSystem.AddCardBundle(cardBundle);

        _availableCards.Remove(cardBundle);
    }
    

    public bool CanBuyRelic()
    {
        return _moneySystem.Money >= _relicPrices;
    }
    
    public bool CanBuyCardBundle()
    {
        return _moneySystem.Money >= _cardsPrice;
    }
}