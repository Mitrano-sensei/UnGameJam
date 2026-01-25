using UnityEngine;

[CreateAssetMenu(menuName = "Cards/CardData")]
public class CardData : ScriptableObject
{
    public string CardName;
    public Sprite CardImage;
    
    [TextArea] public string CardDescription;
}
