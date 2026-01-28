using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/CardData")]
[Serializable]
public class CardData : ScriptableObject
{
    public string CardName;
    public Sprite CardImage;
    
    [TextArea] public string CardDescription;
}
