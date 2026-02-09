using UnityEngine;

public class CoreSceneManager : MonoBehaviour
{
    public void LoadMenuScene()
    {
        SceneController.Instance
            .NewTransition()
            .Load(SceneDatabase.Slots.Menu, SceneDatabase.Scenes.Menu)
            .Perform();
    }
}