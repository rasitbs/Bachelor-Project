using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadNewScene(string sceneName)
    {
        // If the state machine recognises this scene name it handles the load,
        // fires MQTT events, and enforces any prerequisites.
        if (GameStateManager.Instance != null &&
            GameStateManager.Instance.TryTransitionToScene(sceneName))
        {
            return;
        }

        // Fallback for scenes not managed by the state machine (e.g. Tutorial).
        Debug.Log("Loading Scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
}
