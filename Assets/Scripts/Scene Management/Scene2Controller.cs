using UnityEngine;

/// <summary>
/// Thin proxy that lives in Scene 2 so its methods can be wired to
/// Inspector OnClick events. Forwards calls to GameStateManager.
/// </summary>
public class Scene2Controller : MonoBehaviour
{
    /// <summary>
    /// Wire this to the "proceed" button's OnClick event in Scene 2.
    /// Advances to Scene 3 only if both SJA and quiz are complete —
    /// otherwise the call is ignored with a warning.
    /// </summary>
    public void ProceedToScene3()
    {
        GameStateManager.Instance?.NotifyScene2ProceedPressed();
    }
}
