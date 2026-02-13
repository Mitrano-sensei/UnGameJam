using EditorAttributes;
using UnityEngine;
using Utilities;

[CreateAssetMenu(menuName = "Create RepairItem", fileName = "RepairItem", order = 0)]
public class RepairItem : BuyableItem
{
    [Header("Settings")]
    [SerializeField, Required] private RepairPreview previewPrefab;
    [SerializeField, Required] private Sprite image;
    [SerializeField] private string description = "An item to restore all your health!";
    
    public Sprite Image => image;
    public string Description => description;

    public override APreview GeneratePreview()
    {
        var preview = Instantiate(previewPrefab);
        preview.Initialize(this);
        preview.SpawnAnimation();
        preview.AddClickEvent(() => BuyItem(preview));
        
        return preview;
    }

    private void RemoveFromPreview(RepairPreview preview)
    {
        preview.DestroySelf();
    }
    
    private void BuyItem(RepairPreview preview)
    {
        var shopSystem = Registry<ShopSystem>.GetFirst();

        if (!shopSystem.CanBuyRepair()) return;

        shopSystem.BuyRepair(preview);
        RemoveFromPreview(preview);
    }
    
}