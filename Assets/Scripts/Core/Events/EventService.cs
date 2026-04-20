using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventService : MonoBehaviour
{
    public static EventService Instance { get; private set; }

    private ISessionManager _sessionManager;
    private IMqttEventPublisher _mqttPublisher;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        foreach (var manager in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
        {
            if (_sessionManager == null && manager is ISessionManager sm)
                _sessionManager = sm;
            if (_mqttPublisher == null && manager is IMqttEventPublisher mp)
                _mqttPublisher = mp;
        }

        if (_sessionManager == null)
            Debug.LogWarning("[EventService] ISessionManager not found in scene");
        if (_mqttPublisher == null)
            Debug.LogWarning("[EventService] IMqttEventPublisher not found in scene");
    }

    public void PublishSessionStarted(string level)
    {
        var evt = new EventBuilder(_sessionManager)
            .WithEventType("SESSION_START")
            .WithPayload(new EventPayload { level = level })
            .Build();
        _mqttPublisher?.PublishEvent(evt);

#if UNITY_EDITOR
        Debug.Log($"[EventService] Published SESSION_START: {level}");
#endif
    }

    public void PublishSessionEnded()
    {
        var evt = new EventBuilder(_sessionManager)
            .WithEventType("SESSION_END")
            .WithPayload(new EventPayload())
            .Build();
        _mqttPublisher?.PublishEvent(evt);

#if UNITY_EDITOR
        Debug.Log($"[EventService] Published SESSION_END");
#endif
    }

    public void PublishHazardMarked(string hazardId, bool correct, int points)
    {
        var evt = new EventBuilder(_sessionManager)
            .WithEventType("HAZARD_MARKED")
            .WithPayload(new EventPayload
            {
                hazardId = hazardId,
                correct = correct,
                points = points
            })
            .Build();
        _mqttPublisher?.PublishEvent(evt);

#if UNITY_EDITOR
        Debug.Log($"[EventService] Published HAZARD_MARKED: {hazardId} ({(correct ? "correct" : "incorrect")})");
#endif
    }

    public void PublishHseAlert(string triggerId, string description, int penalty)
    {
        var evt = new EventBuilder(_sessionManager)
            .WithEventType("HSE_ALERT_EVENT")
            .WithPayload(new EventPayload
            {
                triggerId = triggerId,
                description = description,
                penalty = penalty
            })
            .Build();
        _mqttPublisher?.PublishEvent(evt);

#if UNITY_EDITOR
        Debug.Log($"[EventService] Published HSE_ALERT: {triggerId}");
#endif
    }

    public void PublishActionInteract(string targetObjectId, string action, string toolUsed, bool isSuccess,
                                       string reason = "", string consequence = "")
    {
        var evt = new EventBuilder(_sessionManager)
            .WithEventType("ACTION_INTERACT")
            .WithPayload(new EventPayload
            {
                targetObjectId = targetObjectId,
                action = action,
                toolUsed = toolUsed,
                isSuccess = isSuccess
            })
            .Build();
        _mqttPublisher?.PublishEvent(evt);

#if UNITY_EDITOR
        Debug.Log($"[EventService] Published ACTION_INTERACT: {action} on {targetObjectId}");
#endif
    }

    public void RequestFinalScore()
    {
        var request = new ScoreRequestMessage
        {
            sessionId = _sessionManager?.GetSessionId() ?? "unknown",
            type = "SCORE_REQUEST"
        };

        _mqttPublisher?.PublishScoreRequest(request);

#if UNITY_EDITOR
        Debug.Log($"[EventService] Requested final score for session: {request.sessionId}");
#endif
    }
}

public interface ISessionManager
{
    string GetSessionId();
    string GetLevel();
}

public interface IMqttEventPublisher
{
    void PublishEvent(MyEvent evt); 
    void PublishScoreRequest(ScoreRequestMessage request);
}
