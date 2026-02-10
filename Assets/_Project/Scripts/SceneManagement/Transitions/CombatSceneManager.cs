using UnityEngine;

public class CombatSceneManager : MonoBehaviour
{
    public void SwitchToShop()
    {
        SceneController.Instance
            .NewTransition()
            .Load(SceneDatabase.Slots.SessionContent, SceneDatabase.Scenes.Shop, setActive:true)
            .WithOverlay()
            .Perform();
    }

    public void EndSession()
    {
        SceneController.Instance
            .NewTransition()
            .Load(SceneDatabase.Slots.Menu, SceneDatabase.Scenes.Menu, true)
            .Unload(SceneDatabase.Slots.Session)
            .Unload(SceneDatabase.Slots.SessionContent)
            .WithOverlay()
            .WithClearUnusedAssets()
            .Perform();
    }
}