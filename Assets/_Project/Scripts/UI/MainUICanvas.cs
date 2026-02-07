using UnityEngine;
using Utilities;

public class MainUICanvas : MonoBehaviour
{
    void Start()
    {
        Registry<MainUICanvas>.TryAdd(this);
    }
    
    private void OnDisable()
    {
        Registry<MainUICanvas>.TryRemove(this);
    }
}
