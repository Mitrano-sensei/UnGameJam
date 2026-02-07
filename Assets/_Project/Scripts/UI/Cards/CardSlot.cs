using UnityEngine;

public class CardSlot : MonoBehaviour
{
    private CardBody cardBody;
    public CardBody CardBody => cardBody;
    public HandManager _handManager;

    private void Start()
    {
        cardBody = GetComponentInChildren<CardBody>();
    }

    public void Initialize(HandManager manager)
    {
        _handManager = manager;
    }
    
    public void GoToTrash()
    {
        transform.SetParent(_handManager.TrashTransform);
    }
}
