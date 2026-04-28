using UnityEngine;

/// <summary>
/// Thin proxy that lives in Scene 3-1 so its methods can be wired to
/// Inspector OnClick events. Forwards calls to GameStateManager.
/// </summary>
public class Scene3_1Controller : MonoBehaviour
{
    /// <summary>
    /// Wire this to the proceed button's OnClick in Scene 3-1.
    /// Advances to the Final scene only if the armature has been installed —
    /// otherwise the call is ignored with a warning.
    /// </summary>
    public void ProceedToFinal()
    {
        GameStateManager.Instance?.NotifyScene3_1ProceedPressed();
    }
}
