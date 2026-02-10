using UnityEngine;

public class CoreSystem : MonoBehaviour
{
    [SerializeField] private CoreSceneManager coreSceneManager;
    
    void Start()
    {
        // Initialization
        // TODO
        
        coreSceneManager.LoadMenuScene();
    }
}
