using System;
using UnityEngine.SceneManagement;

public class SessionContext
{
    public string SessionId { get; private set; }
    public string Level { get; private set; }
    public int SceneId  { get; private set; }

    public void Start(string level)
    {
        SessionId    = Guid.NewGuid().ToString("N");
        Level        = level;
        SceneId      = SceneManager.GetActiveScene().buildIndex;
    }

    public void Reset()
    {
        SessionId    = null;
        Level        = null;
        SceneId      = 0;
    }
}
