using UnityEngine;
using UnityEngine.Events;

public abstract class BuyableItem : ScriptableObject
{
    public abstract APreview GeneratePreview(bool forShop = true, bool spawnAnimation = true);
}

public abstract class APreview : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] protected UnityEvent onClick = new();
    
    private bool _canBuy = true;
    public bool CanBuy => _canBuy;
    
    public void SetCanBuy(bool canBuy) => _canBuy = canBuy;
    
    public void AddClickEvent(UnityAction unityAction) => onClick.AddListener(unityAction);
    public abstract void DestroySelf();
}