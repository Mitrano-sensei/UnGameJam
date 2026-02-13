using System.Collections.Generic;
using System.Linq;
using EditorAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Utilities;

public class DeckSystem : MonoBehaviour, ILoadable
{
    [Header("Reference")]
    [SerializeField, Required] private InputReader inputReader;

    private StatSystem _statSystem;

    [Header("Base Deck")]
    [SerializeField] private BaseDeck baseDeck;

    [Header("Deck")]
    [SerializeField] private int baseHandSize;
    [SerializeField, ReadOnly] private List<CardData> _boughtCards = new();

    [SerializeField, ReadOnly] private List<CardData> _currentHand = new();
    [SerializeField, ReadOnly] private List<CardData> _currentDeck = new();

    private int _currentHandSize;

    [Header("Events")]
    [HideInInspector] private readonly UnityEvent<int, int> onHandSizeChanged = new(); // oldValue, newValue -> onHandSizeChanged

    [Header("Misc")]
    [SerializeField] private bool initOnStart = true;
    [SerializeField] private bool drawOnInit = true;

    [EnableField(nameof(drawOnInit))]
    [SerializeField, Range(1, 100)] private int initialDrawPercent;

    private static readonly System.Random _rng = new System.Random();

    private bool _isInitialized;

    private void Start()
    {
        if (initOnStart) Initialize();
    }

    public void LoadWithScene()
    {
        Registry<DeckSystem>.RegisterSingletonOrLogError(this);

        _statSystem = Registry<StatSystem>.GetFirst();
        _statSystem.AddStatListener(OnMaxHandStatChanged);
    }

    public void UnLoadWithScene()
    {
        Registry<DeckSystem>.TryRemove(this);
        _statSystem.RemoveStatListener(OnMaxHandStatChanged);
    }

    public void Initialize()
    {
        _currentDeck = GetFullDeck();
        _currentHandSize = baseHandSize + _statSystem.GetStatModifierValue(StatSystem.StatType.HandSize);

        _currentHand.Clear();
        ShuffleDeck();

        _isInitialized = true;
        
        if (!drawOnInit) return;
        Draw(Mathf.FloorToInt(_currentHandSize * initialDrawPercent * .01f));
    }

    public List<CardData> GetFullDeck()
    {
        var deck = baseDeck.Cards.ToList();
        deck.AddRange(_boughtCards);
        return deck;
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
            return;
        }

        for (int i = 0; i < amount; i++)
        {
            if (_currentHand.Count >= _currentHandSize) return;
            if (_currentDeck.Count == 0) return;

            var drawnCard = _currentDeck[0];
            _currentHand.Add(drawnCard);
            _currentDeck.Remove(drawnCard);

            handManager.AddCardToHand(drawnCard, true);
        }
    }

    public bool CanDraw()
    {
        return _isInitialized && _currentHand.Count < _currentHandSize;
    }

    public void ReturnCard(CardData cardData)
    {
        _currentHand.Remove(cardData);
        _currentDeck.Add(cardData);
    }

    private void OnMaxHandStatChanged(StatSystem.StatType type, int oldValue, int newValue)
    {
        if (type != StatSystem.StatType.HandSize) return;

        _currentHandSize = baseHandSize + newValue;
        onHandSizeChanged.Invoke(oldValue, _currentHandSize);
    }

    public void OnEndCombatPhase()
    {
        _isInitialized = false;
    }

    #region Shop

    public void AddCardBundle(CardBundle cardBundle)
    {
        _boughtCards.AddRange(cardBundle.Content);
    }
    
    public void AddCard(CardData cardData)
    {
        _boughtCards.Add(cardData);
    }

    #endregion

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
        Draw(baseHandSize);
    }

    #endregion
}