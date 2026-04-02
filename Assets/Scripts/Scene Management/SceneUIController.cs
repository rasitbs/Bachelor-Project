using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionUIController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string lobbySceneName = "Main"; // Change to your actual Lobby scene name
    
    private float sessionStartTime;

    private void Start()
    {
        // Track when this scene started to calculate duration
        sessionStartTime = Time.time;
    }

    /// <summary>
    /// This method will be called by the Button's OnClick event
    /// </summary>
    public void EndSessionAndGoToLobby()
    {
        float duration = Time.time - sessionStartTime;

        if (EventService.Instance != null)
        {
            Debug.Log($"[SessionUI] Ending session. Duration: {duration:F1}s");
            
            // 1. Call your existing event service logic
            EventService.Instance.PublishSessionEnded(duration);
            
            // 2. (Optional) Request the final score immediately so it's ready when we get to the lobby
            EventService.Instance.RequestFinalScore();
        }
        else
        {
            Debug.LogWarning("EventService Instance not found. Make sure you started from the Main scene.");
        }

        // 3. Load the Lobby/Main scene
        SceneManager.LoadNewScene(lobbySceneName);
    }
}
