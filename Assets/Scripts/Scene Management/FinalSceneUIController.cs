using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 

public class FinalSceneUIController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string lobbySceneName = "Main"; 
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI poengscoreText;

    private void OnEnable()
    {
        ScoreResponseHandler.OnScoreReceived += HandleScoreReceived;
    }

    private void OnDisable()
    { 
        ScoreResponseHandler.OnScoreReceived -= HandleScoreReceived;
    }

    private void Start()
    {
        if (poengscoreText != null)
        {
            poengscoreText.text = "Laster Score...";
        }

        if (EventService.Instance != null)
        {
            Debug.Log($"[SessionUI] Ending session.");
            
            // Publish session end event 
            EventService.Instance.PublishSessionEnded();
            
            // Request the final score from the backend
            EventService.Instance.RequestFinalScore();
        }
        else
        {
            Debug.LogWarning("[SessionUI] EventService Instance not found. Make sure you started from the Main scene.");
            if (poengscoreText != null) poengscoreText.text = "Error: No Event Service";
        }
    }

    private void HandleScoreReceived(string sessionId, int finalScore)
    {
        Debug.Log($"[SessionUI] Score received for session {sessionId}: {finalScore}");
        
        if (poengscoreText != null)
        {
            poengscoreText.text = $"Poengscore: {finalScore}";
        }
    }

    public void EndSessionAndGoToLobby()
    {
        // Load the Lobby/Main scene
        SceneManager.LoadScene(lobbySceneName);
    }
}
