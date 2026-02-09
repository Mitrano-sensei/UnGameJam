using System;
using System.Collections.Generic;
using SerializeReferenceEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/CardData")]
[Serializable]
public class CardData : ScriptableObject
{
    public string CardName;
    public Sprite CardImage;
    
    [TextArea] public string CardDescription;
    
    [SerializeField, SerializeReference]
    [SR]
    public List<GameAction> Effects;
}