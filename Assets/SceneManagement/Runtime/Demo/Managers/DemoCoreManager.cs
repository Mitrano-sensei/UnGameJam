using UnityEngine;

public class DemoCoreManager : MonoBehaviour
{
    void Start()
    {
        // Core Setup
        // Load everything that is ALWAYS persistent, like Audio, Input, UI Events, Save System etc
        SceneController.Instance
            .NewTransition()
            .Load(DemoSceneDatabase.Slots.Menu, DemoSceneDatabase.Scenes.Menu)
            .WithOverlay()
            .Perform();
    }
}