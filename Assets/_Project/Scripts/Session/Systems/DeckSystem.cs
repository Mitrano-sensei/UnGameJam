using System.Collections.Generic;
using System.Linq;
using EditorAttributes;
using UnityEngine;
using Utilities;

public class DeckSystem : MonoBehaviour, ILoadable
{
    [Header("Reference")]
    [SerializeField, Required] private InputReader inputReader;

    [Header("Base Deck")]
    [SerializeField] private BaseDeck baseDeck;

    [Header("Deck")]
    [SerializeField] private int handSize;
    
    [SerializeField, ReadOnly] private List<CardData> _currentHand = new();
    [SerializeField, ReadOnly] private List<CardData> _currentDeck = new();

    [Header("Misc")]
    [SerializeField] private bool initOnStart = true;
    [SerializeField] private bool drawOnInit = true;
    
    [EnableField(nameof(drawOnInit))]
    [SerializeField, Range(1, 100)] private int initialDrawPercent;
    
    private static readonly System.Random _rng = new System.Random();

    private void Start()
    {
        if (initOnStart) Initialize();
    }
    
    public void LoadWithScene()
    {
        if (Registry<DeckSystem>.All.Any())
        {
            Debug.LogError("There is already a deck system in the scene, only one is allowed at a time");
            return;
        }
        
        Registry<DeckSystem>.TryAdd(this);
    }

    public void UnLoadWithScene()
    {
        Registry<DeckSystem>.TryRemove(this);
    }

    public void Initialize()
    {
        _currentDeck = baseDeck.Cards.ToList();
        // TODO : Add purchased cards
        
        _currentHand.Clear();
        ShuffleDeck();

        if (!drawOnInit) return;
        Draw(Mathf.FloorToInt(handSize * initialDrawPercent * .01f));
    }

    private void ShuffleDeck()
    {
        _currentDeck = _currentDeck.OrderBy((c) => _rng.Next()).ToList();
    }


    public void Draw(int amount = 1)
    {
        var handManager = Registry<HandManager>.GetFirst();

        if (!handManager)
        {
            Debug.LogWarning("Trying to draw cards without a hand manager");
        }
        
        for (int i = 0; i < amount; i++)
        {
            if (_currentHand.Count >= handSize) return;
            if (_currentDeck.Count == 0)        return;

            var drawnCard = _currentDeck[0];
            _currentHand.Add(drawnCard);
            _currentDeck.Remove(drawnCard);
            
            handManager.AddCardToHand(drawnCard, true);
        }
    }

    public bool CanDraw()
    {
        return _currentHand.Count < handSize;
    }

    public void ReturnCard(CardData cardData)
    {
        _currentHand.Remove(cardData);
        _currentDeck.Add(cardData);
        
    }

    #region Debug

    [Button]
    private void InitDebug()
    {
        if (!Application.IsPlaying(this)) return;
        
        Initialize();
    }

    [Button]
    private void DrawDebug()
    {
        if (!Application.IsPlaying(this)) return;
        Draw();
    }

    [Button]
    private void ShuffleDebug()
    {
        if (!Application.IsPlaying(this)) return;
        ShuffleDeck();
    }

    [Button]
    private void FillHandDebug()
    {
        if (!Application.IsPlaying(this)) return;
        Draw(handSize);
    }

    #endregion
}