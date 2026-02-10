using UnityEngine;

public class DemoMainMenuManager : MonoBehaviour
{
    public void StartSession()
    {
        SceneController.Instance
            .NewTransition()
            .Load(DemoSceneDatabase.Slots.Session, DemoSceneDatabase.Scenes.Session)
            .Load(DemoSceneDatabase.Slots.SessionContent, DemoSceneDatabase.Scenes.Shop, setActive: true)
            .Unload(DemoSceneDatabase.Slots.Menu)
            .WithOverlay()
            .WithClearUnusedAssets()
            .Perform();
    }
}