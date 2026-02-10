using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

/**
 * Inspired by https://www.youtube.com/watch?v=oejg12YyYyI
 */
public class SceneController : Singleton<SceneController>
{
    [SerializeField] private LoadingOverlay loadingOverlay;

    private Dictionary<string, string> loadedSceneBySlot = new();
    private bool _isBusy = false;

    public Coroutine ExecutePlan(SceneTransitionPlan plan)
    {
        if (_isBusy)
        {
            Debug.LogWarning("Scene change already in progress :/");
            return null;
        }
        
        _isBusy = true;
        return StartCoroutine(ChangeSceneCoroutine(plan));
    }

    public SceneTransitionPlan NewTransition()
    {
        return new SceneTransitionPlan();
    }

    private IEnumerator ChangeSceneCoroutine(SceneTransitionPlan plan)
    {
        if (plan.UseOverlay)
        {
            yield return loadingOverlay.FadeIn();
            yield return new WaitForSeconds(.5f);
        }

        foreach (var slotToUnload in plan.SlotsToUnload)
        {
            yield return UnloadSceneCoroutine(slotToUnload);
        }

        if (plan.ClearUnusedAsset) yield return CleanupUnusedAssetCoroutine();

        foreach (var sceneToLoadKvp in plan.ScenesToLoad)
        {
            if (loadedSceneBySlot.ContainsKey(sceneToLoadKvp.Key))
                yield return UnloadSceneCoroutine(sceneToLoadKvp.Key);
            
            yield return LoadAdditiveSceneCoroutine(sceneToLoadKvp.Key, sceneToLoadKvp.Value, plan.ActiveSceneName == sceneToLoadKvp.Value);
        }

        if (plan.UseOverlay)
            yield return loadingOverlay.FadeOut();

        _isBusy = false;
    }

    private IEnumerator LoadAdditiveSceneCoroutine(string slotKey, string sceneName, bool isActiveScene)
    {
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        if (loadOp == null) yield break;
        loadOp.allowSceneActivation = false;
        
        while (loadOp.progress < .9f) yield return null;
        loadOp.allowSceneActivation = true;

        while (!loadOp.isDone) yield return null;

        if (isActiveScene)
        {
            Scene newScene = SceneManager.GetSceneByName(sceneName);
            if (newScene.IsValid() && newScene.isLoaded) 
                SceneManager.SetActiveScene(newScene);
        }
        loadedSceneBySlot[slotKey] = sceneName;
    }
    
    /**
     * Not strictly necessary, but it's a good idea to clean up the unused assets when changing from big scenes.
     */
    private IEnumerator CleanupUnusedAssetCoroutine()
    {
        AsyncOperation cleanupOp = Resources.UnloadUnusedAssets();
        while (!cleanupOp.isDone) yield return null;
    }

    private IEnumerator UnloadSceneCoroutine(string slotKey)
    {
        if (!loadedSceneBySlot.TryGetValue(slotKey, out string sceneName)) yield break;
        if (string.IsNullOrEmpty(sceneName)) yield break;

        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(sceneName);
        if (unloadOp != null)
        {
            while (!unloadOp.isDone) yield return null;
        }

        loadedSceneBySlot.Remove(slotKey);
    }

    #region SceneTransitionPlan Builder
    public class SceneTransitionPlan
    {
        public Dictionary<string, string> ScenesToLoad { get; }= new();
        public List<string> SlotsToUnload { get; }= new();
        public string ActiveSceneName { get; private set; } = "";
        public bool ClearUnusedAsset { get; private set; } = false;
        public bool UseOverlay { get; private set; } = false;
        
        public SceneTransitionPlan Load(string slotKey, string sceneName, bool setActive = false)
        {
            ScenesToLoad[slotKey] = sceneName;
            if (setActive) ActiveSceneName = sceneName;
            return this;
        }

        public SceneTransitionPlan Unload(string slotKey)
        {
            SlotsToUnload.Add(slotKey);
            return this;
        }

        public SceneTransitionPlan WithOverlay()
        {
            UseOverlay = true;
            return this;
        }
        
        public SceneTransitionPlan WithClearUnusedAssets()
        {
            ClearUnusedAsset = true;
            return this;
        }

        public Coroutine Perform()
        {
            return SceneController.Instance.ExecutePlan(this);
        }
    }
    #endregion
}