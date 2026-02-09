using UnityEngine;

public class DemoCombatSceneManager : MonoBehaviour
{
    public void SwitchToShop()
    {
        SceneController.Instance
            .NewTransition()
            .Load(DemoSceneDatabase.Slots.SessionContent, DemoSceneDatabase.Scenes.Shop, setActive:true)
            .WithOverlay()
            .Perform();
    }

    public void EndSession()
    {
        SceneController.Instance
            .NewTransition()
            .Load(DemoSceneDatabase.Slots.Menu, DemoSceneDatabase.Scenes.Menu, true)
            .Unload(DemoSceneDatabase.Slots.Session)
            .Unload(DemoSceneDatabase.Slots.SessionContent)
            .WithOverlay()
            .WithClearUnusedAssets()
            .Perform();
    }
}