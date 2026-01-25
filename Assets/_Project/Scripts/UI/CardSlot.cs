using UnityEngine;

public class CardSlot : MonoBehaviour
{
    private CardBody cardBody;
    public CardBody CardBody => cardBody;

    private void Start()
    {
        cardBody = GetComponentInChildren<CardBody>();
    }
}
