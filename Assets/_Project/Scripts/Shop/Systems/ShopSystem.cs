using System;
using System.Collections.Generic;
using System.Linq;
using EditorAttributes;
using UnityEngine;
using Utilities;

public class ShopSystem : MonoBehaviour, ILoadable
{
    [Header("References")]
    [SerializeField] private RepairItem repairItem;
    private MoneySystem _moneySystem;
    private RelicSystem _relicSystem;
    private DeckSystem _deckSystem;

    private List<RelicData> _availableRelics;
    private List<CardBundle> _availableCards;

    [Header("Shop Items")]
    [SerializeField] private List<RelicData> _possibleRelics;
    [SerializeField] private List<CardBundle> _possibleCards;

    [SerializeField] private List<ShopSlot> _shopSlots;

    private readonly List<Transform> _cardBundleSlots = new();
    private readonly List<Transform> _relicSlots = new();
    private readonly List<Transform> _repairSlots = new();

    private List<APreview> _cardPreviewReferences = new();
    private List<APreview> _relicPreviewReferences = new();
    private List<APreview> _repairReferences = new();

    [Header("Prices")]
    [SerializeField] private int _relicPrices = 2;
    [SerializeField] private int _cardsPrice = 2;
    [SerializeField] private int _repairPrice = 1;
    [SerializeField] private int _removeCardPrice = 1;
    [SerializeField] private int _singleCardPrice = 1;

    private List<RelicData> _boughtRelics = new();
    
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
        DestroyBuyableItems();

        InitializeShopSlots();

        _availableCards = _possibleCards.Shuffle().GetRange(0, _cardBundleSlots.Count);
        _availableRelics = _possibleRelics.FindAll(r => !_boughtRelics.Contains(r)).Shuffle().GetRange(0, _relicSlots.Count);

        // Relics
        for (var index = 0; index < _availableRelics.Count; index++)
        {
            var relic = _availableRelics[index];
            var preview = InitPreview(relic, index, ShopSlot.ShopSlotType.Relic);

            if (preview == null) continue;
            _relicPreviewReferences.Add(preview);
        }

        // Card Bundles
        for (var index = 0; index < _availableCards.Count; index++)
        {
            var card = _availableCards[index];
            var preview = InitPreview(card, index, ShopSlot.ShopSlotType.CardBundle);

            if (preview == null) continue;
            _cardPreviewReferences.Add(preview);
        }

        // Repair
        for (var index = 0; index < _repairSlots.Count; index++)
        {
            APreview preview = InitPreview(repairItem, index, ShopSlot.ShopSlotType.Repair);
            preview.transform.position = _repairSlots[index].position;

            _repairReferences.Add(preview);
        }
    }

    private void InitializeShopSlots()
    {
        foreach (var slot in _shopSlots)
        {
            var list = slot.Type switch
            {
                ShopSlot.ShopSlotType.CardBundle => _cardBundleSlots,
                ShopSlot.ShopSlotType.Relic => _relicSlots,
                ShopSlot.ShopSlotType.Repair => _repairSlots,
                _ => throw new ArgumentOutOfRangeException()
            };

            list.Add(slot.SlotTransform);
        }
    }

    private void DestroyBuyableItems()
    {
        _cardPreviewReferences.ForEach(p => p.DestroySelf());
        _relicPreviewReferences.ForEach(p => p.DestroySelf());
        _repairReferences.ForEach(r => r.DestroySelf());

        _cardPreviewReferences.Clear();
        _relicPreviewReferences.Clear();
        _repairReferences.Clear();
    }

    private APreview InitPreview(BuyableItem buyableItem, int index, ShopSlot.ShopSlotType type)
    {
        switch (type)
        {
            case ShopSlot.ShopSlotType.Relic when index >= _relicSlots.Count:
                Debug.LogError("Index out of bounds, skipping");
                return null;
            case ShopSlot.ShopSlotType.CardBundle when index >= _cardBundleSlots.Count:
                Debug.LogError("Index out of bounds, skipping");
                return null;
            case ShopSlot.ShopSlotType.Repair when index >= _repairSlots.Count:
                Debug.LogError("Index out of bounds, skipping");
                return null;
        }

        var parentSlot = type switch
        {
            ShopSlot.ShopSlotType.CardBundle => _cardBundleSlots[index],
            ShopSlot.ShopSlotType.Relic => _relicSlots[index],
            ShopSlot.ShopSlotType.Repair => _repairSlots[index],
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
        
        var preview = buyableItem.GeneratePreview();
        preview.transform.SetParent(parentSlot);
        preview.transform.localPosition = Vector3.zero;

        return preview;
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
        _boughtRelics.Add(relic);

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

    public void BuyRepair(APreview repairPreview)
    {
        if (_moneySystem.Money < _repairPrice) return;
        _moneySystem.Money -= _repairPrice;
        
        var healthSystem = Registry<HealthSystem>.GetFirst();
        healthSystem.FullHeal();
        _repairReferences.Remove(repairPreview);
    }

    public void BuyCard(APreview cardPreview)
    {
        if (_moneySystem.Money < _singleCardPrice) return;
        _moneySystem.Money -= _singleCardPrice;

        // TODO: Not fully implemented
        // _deckSystem.AddCard(preview.CardData);
        Debug.LogError("Not fully implemented");
        cardPreview.DestroySelf();
    }

    public bool CanBuyRelic() => _moneySystem.Money >= _relicPrices;
    public bool CanBuyCardBundle() => _moneySystem.Money >= _cardsPrice;
    public bool CanBuyRepair() => _moneySystem.Money >= _repairPrice;


    [Serializable]
    public class ShopSlot
    {
        public Transform SlotTransform;
        public ShopSlotType Type;

        public enum ShopSlotType
        {
            CardBundle,
            Relic,
            Repair
        }
    }
}