using UnityEditor.Build.Content;
using UnityEngine;

public class DemoShopSceneManager : MonoBehaviour
{
    public void SwitchToCombat()
    {
        SceneController.Instance
            .NewTransition()
            .Load(DemoSceneDatabase.Slots.SessionContent, DemoSceneDatabase.Scenes.Combat, setActive:true)
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