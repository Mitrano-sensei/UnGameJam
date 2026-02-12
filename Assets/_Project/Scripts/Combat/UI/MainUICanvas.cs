using System.Linq;
using UnityEngine;
using Utilities;

public class MainUICanvas : MonoBehaviour, ILoadable
{
    public void LoadWithScene()
    {
        Registry<MainUICanvas>.RegisterSingletonOrLogError(this);
    }

    public void UnLoadWithScene()
    {
        Registry<MainUICanvas>.TryRemove(this);
    }
}
