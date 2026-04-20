using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MyEvent
{
    public EventHeader header;
    public EventPayload payload;
    public TelemetryData telemetry;

    public MyEvent(EventHeader header, EventPayload payload, TelemetryData telemetry = null)
    {
        this.header = header;
        this.payload = payload;
        this.telemetry = telemetry;
    }
}

[Serializable]
public class EventHeader
{
    public string sessionId;
    public string timestamp;
    public int    sceneId;
    public string eventType;
}

[Serializable]
public class EventMetadata
{
    public string reason;
    public string consequenceTriggered;
}

[Serializable]
public class EventPayload
{
    public string level;

    public string targetObjectId;
    public string action;
    public string toolUsed;
    public bool   isSuccess;
    
    [NonSerialized]
    public EventMetadata metadata;

    public string hazardId;
    public bool   correct;
    public int    points;
    public string triggerId;
    public string description;
    public int    penalty;
}

[Serializable]
public class TelemetryData
{
    public Vector3Data playerPosition;
}

[Serializable]
public class Vector3Data
{
    public float x;
    public float y;
    public float z;

    public Vector3Data() { }
    
    public Vector3Data(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}


[Serializable]
public class ScoreRequestMessage
{
    public string sessionId;
    public string type = "SCORE_REQUEST";
}

[Serializable]
public class ScoreResponseMessage
{
    public string sessionId;
    public int    finalScore;
}
