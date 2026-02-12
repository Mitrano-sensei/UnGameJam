using System.Linq;
using EditorAttributes;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

public class MoneySystem : MonoBehaviour, ILoadable
{
    [SerializeField] private int baseMoney = 4;
    
    private int _money;
    public int Money
    {
        get => _money;
        set => SetMoney(value);
    }

    [Header("Misc")]
    [SerializeField] private bool isDebug;
    
    public UnityEvent<MoneyChangedEventData> OnMoneyChanged = new(); 

    public void LoadWithScene()
    {
        Registry<MoneySystem>.RegisterSingletonOrLogError(this);
        
        _money = baseMoney;
        OnMoneyChanged.AddListener(e => { if (isDebug) Debug.Log($"Money changed from {e.OldMoney} to {e.NewMoney}"); });
    }

    public void UnLoadWithScene()
    {
        Registry<MoneySystem>.TryRemove(this);        
    }
    
    public void SetMoney(int value)
    {
        var oldValue = _money;
        _money = value;
        OnMoneyChanged.Invoke(new MoneyChangedEventData(oldValue, _money));
    }
    
    public void AddMoney(int value) => SetMoney(Money + value);
    
    [Button]
    public void AddOneMoney() => AddMoney(1);
    
    [Button]
    public void RemoveOneMoney() => AddMoney(-1);
    
    public class MoneyChangedEventData
    {
        public int OldMoney { get; set; }
        public int NewMoney { get; set; }
        public int Change => NewMoney - OldMoney;
        
        public MoneyChangedEventData(int oldMoney, int newMoney)
        {
            OldMoney = oldMoney;
            NewMoney = newMoney;
        }
    }
}
