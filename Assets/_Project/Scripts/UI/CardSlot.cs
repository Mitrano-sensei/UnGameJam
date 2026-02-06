using UnityEngine;
using Utilities;

public class CardSlot : MonoBehaviour
{
    private CardBody cardBody;
    public CardBody CardBody => cardBody;

    private void Start()
    {
        cardBody = GetComponentInChildren<CardBody>();
    }

    public void GoToTrash()
    {
        var handManager = Registry<HandManager>.GetFirst();
        transform.SetParent(handManager.TrashTransform);
    }
}
