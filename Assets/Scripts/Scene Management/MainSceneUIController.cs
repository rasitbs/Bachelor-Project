using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneUIController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string sceneName = "Scene 1"; 
    [SerializeField] private string scenarioName = "HMS Grunnleggende";

    public void StartSessionAndGoToScene()
    {
        // Route through the state machine — it publishes SESSION_START and loads the scene.
        if (GameStateManager.Instance != null)
        {
            Debug.Log($"[SessionUI] Starting session '{scenarioName}' via GameStateManager.");
            GameStateManager.Instance.ChangeState(GameState.Scene1_PPE);
        }
        else
        {
            Debug.LogWarning("[SessionUI] GameStateManager not found — cannot start session.");
        }
    }
}

