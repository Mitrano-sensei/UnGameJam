using UnityEngine;
using UnityEngine.Events;

public abstract class BuyableItem : ScriptableObject
{
    public abstract APreview GeneratePreview();
}

public abstract class APreview : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] protected UnityEvent onClick = new();
    
    public void AddClickEvent(UnityAction unityAction) => onClick.AddListener(unityAction);
    public abstract void DestroySelf();
}