using UnityEngine;

public class ShopSceneManager : MonoBehaviour
{
    public void SwitchToCombat()
    {
        SceneController.Instance
            .NewTransition()
            .Load(SceneDatabase.Slots.SessionContent, SceneDatabase.Scenes.Combat, setActive:true)
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