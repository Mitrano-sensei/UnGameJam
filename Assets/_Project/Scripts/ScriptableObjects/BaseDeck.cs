using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/BaseDeck")]
[Serializable]
public class BaseDeck : ScriptableObject
{
    public string Name;
    [SerializeField] public int baseHandSize = 5;
    public List<CardData> Cards;
}