using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private string prefix = "Din poengsum: ";

    private void Awake()
    {
        // If not assigned in inspector, try to find it on this object
        if (scoreText == null)
            scoreText = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        // Subscribe to the static event in ScoreResponseHandler
        ScoreResponseHandler.OnScoreReceived += UpdateScoreUI;
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks/errors
        ScoreResponseHandler.OnScoreReceived -= UpdateScoreUI;
    }

    private void Start()
    {
        // Trigger the fetch request as soon as the scene starts
        FetchPoints();
    }

    public void FetchPoints()
    {
        Debug.Log("[ScoreDisplay] Requesting points...");
        if (EventService.Instance != null)
        {
            EventService.Instance.RequestFinalScore();
        }
        else
        {
            Debug.LogError("EventService Instance not found! Is the Main scene loaded first?");
        }
    }

    private void UpdateScoreUI(string sessionId, int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"{prefix}{score}";
        }
    }
}
