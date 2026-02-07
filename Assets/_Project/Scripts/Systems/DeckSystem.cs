using System.Collections.Generic;
using System.Linq;
using EditorAttributes;
using UnityEngine;
using Utilities;

public class DeckSystem : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField, Required] private InputReader inputReader;
    private HandManager _handManager;
    
    private HandManager MyHandManager
    {
        get
        {
            _handManager ??= Registry<HandManager>.GetFirst();
            if (_handManager == null)
            {
                Debug.LogError("No Hand Manager found in the scene, please add one and try again");
                return null;
            }

            return _handManager;
        }
    }

    [Header("Base Deck")]
    [SerializeField] private List<CardData> baseDeck;

    [Header("Deck")]
    [SerializeField] private int handSize;
    
    [SerializeField, ReadOnly] private List<CardData> _currentHand = new();
    [SerializeField, ReadOnly] private List<CardData> _currentDeck = new();

    [Header("Misc")]
    [SerializeField] private bool _initOnStart = true;
    
    private static readonly System.Random _rng = new System.Random();

    private void Awake()
    {
        if (Registry<DeckSystem>.All.Any())
        {
            Debug.LogError("There is already a deck system in the scene, only one is allowed at a time");
            return;
        }
        
        Registry<DeckSystem>.TryAdd(this);
    }

    private void Start()
    {
        if (_initOnStart) Initialize();
    }

    private void Initialize()
    {
        _currentDeck = baseDeck;
        ShuffleDeck();
        Draw(handSize);
    }

    private void ShuffleDeck()
    {
        _currentDeck = _currentDeck.OrderBy((c) => _rng.Next()).ToList();

        // TODO: Animation?
    }


    public void Draw(int amount = 1)
    {
        for (int i = 0; i < amount; i++)
        {
            if (_currentHand.Count >= handSize) return;
            if (_currentDeck.Count == 0)        return;

            var drawnCard = _currentDeck[0];
            _currentHand.Add(drawnCard);
            _currentDeck.Remove(drawnCard);
            
            MyHandManager.AddCardToHand(drawnCard, true);
        }
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

    public void ReturnCard(CardData cardData)
    {
        _currentHand.Remove(cardData);
        _currentDeck.Add(cardData);
        
    }
}