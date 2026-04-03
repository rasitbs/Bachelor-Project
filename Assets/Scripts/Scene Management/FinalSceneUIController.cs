using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Required for TextMeshPro

public class FinalSceneUIController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string lobbySceneName = "Main"; 
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI poengscoreText; // Assign "Poengscore" TMP object in the inspector

    private void OnEnable()
    {
        // Subscribe to the MQTT score response event
        ScoreResponseHandler.OnScoreReceived += HandleScoreReceived;
    }

    private void OnDisable()
    {
        // Always unsubscribe to prevent memory leaks
        ScoreResponseHandler.OnScoreReceived -= HandleScoreReceived;
    }

    private void Start()
    {
        // Show a loading text while waiting for the MQTT broker
        if (poengscoreText != null)
        {
            poengscoreText.text = "Laster Score...";
        }

        if (EventService.Instance != null)
        {
            // 1. Calculate total duration from the MainSceneUIController's static variable
            float duration = Time.time - MainSceneUIController.GlobalSessionStartTime;
            Debug.Log($"[SessionUI] Ending session. Total Duration: {duration:F1}s");
            
            // 2. Publish session end
            EventService.Instance.PublishSessionEnded(duration);
            
            // 3. Request the final score from the backend
            EventService.Instance.RequestFinalScore();
        }
        else
        {
            Debug.LogWarning("EventService Instance not found. Make sure you started from the Main scene.");
            if (poengscoreText != null) poengscoreText.text = "Error: No Event Service";
        }
    }

    /// <summary>
    /// Called automatically when the backend responds with the score.
    /// </summary>
    private void HandleScoreReceived(string sessionId, int finalScore)
    {
        Debug.Log($"[SessionUI] Score received for session {sessionId}: {finalScore}");
        
        if (poengscoreText != null)
        {
            poengscoreText.text = $"Poengscore: {finalScore}";
        }
    }

    /// <summary>
    /// This method will be called by the "Lobby" Button's OnClick event
    /// </summary>
    public void EndSessionAndGoToLobby()
    {
        // Load the Lobby/Main scene
        SceneManager.LoadScene(lobbySceneName);
    }
}
