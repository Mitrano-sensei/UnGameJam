using System.Linq;
using UnityEngine;
using Utilities;

public class MainUICanvas : MonoBehaviour, ILoadable
{
    public void LoadWithScene()
    {
        if (Registry<MainUICanvas>.All.Any())
        {
            Debug.LogError("There is already a deck handler in the scene, only one is allowed at a time");
            return;
        }
        
        Registry<MainUICanvas>.TryAdd(this);
    }

    public void UnLoadWithScene()
    {
        Registry<MainUICanvas>.TryRemove(this);
    }
}
