using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventBuilder
{
    private EventHeader _header;
    private EventPayload _payload;
    private TelemetryData _telemetry;
    private ISessionManager _sessionManager;
    private int _sceneId;

    public EventBuilder(ISessionManager sessionManager)
    {
        _sessionManager = sessionManager;
        _sceneId = SceneManager.GetActiveScene().buildIndex;
    }

    public EventBuilder WithEventType(string eventType)
    {
        if (_header == null)
            _header = new EventHeader();

        _header.eventType = eventType;
        _header.sessionId = _sessionManager?.GetSessionId() ?? "unknown";
        _header.timestamp = DateTime.UtcNow.ToString("o");
        _header.sceneId = _sceneId;
        return this;
    }

    public EventBuilder WithPayload(EventPayload payload)
    {
        _payload = payload ?? new EventPayload();
        return this;
    }

    public EventBuilder WithTelemetry(TelemetryData telemetry)
    {
        _telemetry = telemetry;
        return this;
    }

    public EventBuilder WithPlayerPosition(Vector3 position)
    {
        if (_telemetry == null)
            _telemetry = new TelemetryData();

        _telemetry.playerPosition = new Vector3Data(position.x, position.y, position.z);
        return this;
    }

    public MyEvent Build()
    {
        if (_header == null)
            throw new InvalidOperationException("EventBuilder: event type must be set before building");

        if (_payload == null)
            _payload = new EventPayload();

        return new MyEvent(_header, _payload, _telemetry);
    }
}