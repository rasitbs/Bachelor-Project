using System;
using UnityEngine;

public class SessionManager : MonoBehaviour, ISessionManager
{
    [Header("Scenario Configuration")]
    [SerializeField] private string Level = "HMS - Skifte av lysarmatur";

    private SessionContext _session;

    public static SessionManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _session = new SessionContext();
    }

    private void Start()
    {
        _session.Start(Level);
        Debug.Log($"[SessionManager] Session initialized: {_session.SessionId}");
    }

    public string GetSessionId() => _session?.SessionId ?? "unknown";
    public string GetLevel() => Level;

    public void ResetSession()
    {
        _session?.Reset();
        _session.Start(Level);
        Debug.Log($"[SessionManager] Session reset: {_session.SessionId}");
    }

    private void OnDestroy()
    {
        _session?.Reset();
    }
}
