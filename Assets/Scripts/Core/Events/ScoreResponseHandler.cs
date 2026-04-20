using System;
using System.Text;
using UnityEngine;

public class ScoreResponseHandler : MonoBehaviour
{
    [Header("Topic Configuration")]
    [SerializeField] private string topicScoreResponse = "response/#";

    public static event Action<string, int> OnScoreReceived;

    private void OnEnable()
    {
        MqttApplicationManager.OnMessageReceived += HandleMessageReceived;
    }

    private void OnDisable()
    {
        MqttApplicationManager.OnMessageReceived -= HandleMessageReceived;
    }

    private void HandleMessageReceived(string topic, string payload)
    {
        if (topic != topicScoreResponse)
            return;

        try
        {
            var response = JsonUtility.FromJson<ScoreResponseMessage>(payload);
            OnScoreReceived?.Invoke(response.sessionId, response.finalScore);

            #if UNITY_EDITOR
            Debug.Log($"[ScoreResponseHandler] Received score: session={response.sessionId}, score={response.finalScore}");
            #endif
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[ScoreResponseHandler] Failed to parse score response: {ex.Message}");
        }
    }
}
