using System.Collections.Generic;
using UnityEngine;

public class CardEffectHandler : MonoBehaviour
{
    [Header("References")]
    private CardBody _cardBody;
    private List<GameAction> _effects;


    public void Initialize(CardBody cardBody, CardData cardData)
    {
        this._cardBody = cardBody;
        this._effects = cardData.Effects;
    }
    
    
    public void PerformEffects()
    {
        var actionSystem = ActionSystem.Instance;
        if (actionSystem.IsPerforming) return;

        actionSystem.Perform(_effects, () =>
        {
            Debug.Log("Done");
        });
        
        // TODO: Animations & Destroy gameobject
    }
}