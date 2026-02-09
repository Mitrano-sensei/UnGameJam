using System;
using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;

public class BootstrapLoader : MonoBehaviour
{
    [Header("To Load")]
    [HelpBox("Will Load from bottom to top")]
    [SerializeField] private List<GameObject> toLoad = new List<GameObject>();
    [HelpBox("Will UnLoad from bottom to top")]
    [SerializeField] private List<GameObject> toUnLoad = new List<GameObject>();
    
    private void OnEnable()
    {
        foreach (GameObject toLoadGo in toLoad)
        {
            Debug.Log("Loading " + toLoadGo.name);
            if (!toLoadGo.TryGetComponent<ILoadable>(out var loadable))
            {
                Debug.LogError($"Gameobject {toLoadGo.name} does not implement ILoadable");
            }
            loadable.LoadWithScene();
        }
    }

    private void OnDisable()
    {
        foreach (var toUnLoadGo in toUnLoad)
        {
            if (!toUnLoadGo.TryGetComponent<ILoadable>(out var loadable))
            {
                Debug.LogError($"Gameobject {toUnLoadGo.name} does not implement ILoadable");
            }
            if (!toLoad.Contains(toUnLoadGo))
            {
                Debug.LogError($"Gameobject {toUnLoadGo.name} is not in toLoad list.");
            }
            
            loadable.UnLoadWithScene();
        }
    }
}

public interface ILoadable
{
    void LoadWithScene();
    void UnLoadWithScene();
}
