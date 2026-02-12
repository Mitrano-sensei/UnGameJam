using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;

public class RelicSystem : MonoBehaviour, ILoadable
{
    private List<RelicData> _activeRelics = new List<RelicData>();
    
    public List<String> ActiveRelics => _activeRelics.Select(relic => relic.RelicName).ToList();
    
    public void LoadWithScene()
    {
        Registry<RelicSystem>.RegisterSingletonOrLogError(this);
    }

    public void UnLoadWithScene()
    {
        Registry<RelicSystem>.TryRemove(this);
    }
    
    public void AddRelic(RelicData relic) => _activeRelics.Add(relic);
    public void RemoveRelic(RelicData relic) => _activeRelics.Remove(relic);
    
    public void ApplyRelics() => _activeRelics.ForEach(relic => relic.Apply());
}