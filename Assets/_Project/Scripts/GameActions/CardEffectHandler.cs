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

        cardBody.EndDragEvent?.AddListener(OnEndDragEvent);
    }

    private void OnEndDragEvent(CardBody cardBody, bool isPlayed)
    {
        if (isPlayed) PerformEffects();
    }


    public void PerformEffects()
    {
        var actionSystem = ActionSystem.Instance;
        if (actionSystem.IsPerforming)
        {
            Debug.Log("Was Already performing");
            // TODO : Add queue
            return;
        }

        actionSystem.Perform(_effects, () => { Debug.Log("Done"); });
    }
}