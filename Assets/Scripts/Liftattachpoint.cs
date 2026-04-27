using UnityEngine;
using UnityEngine.SceneManagement;


public class LiftAttachPoint : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource attachSound;

    [Header("Settings")]
    public float attachRadius = 0.3f;           // How close hook must be
    public string nextSceneName = "Scene 3-1";  // Must match scene name exactly

    [Header("Visual Feedback")]
    public Renderer indicatorRenderer;          // Optional glowing ring/indicator
    public Color idleColor = new Color(0f, 0.5f, 1f, 0.5f);    // Blue idle
    public Color readyColor = new Color(0f, 1f, 0.3f, 0.8f);   // Green when hook nearby

    [Header("Transition")]
    public float transitionDelay = 1.5f;        // Seconds before scene loads

    private bool _hookAttached = false;
    private HookItem _hookInRange = null;

    void Start()
    {
        SetIndicatorColor(idleColor);
    }

    void Update()
    {
        if (_hookAttached) return;

        // Find hook in scene
        HookItem[] hooks = FindObjectsOfType<HookItem>();
        HookItem nearest = null;
        float nearestDist = float.MaxValue;

        foreach (var hook in hooks)
        {
            if (!hook.IsGrabbed) continue; // Only detect when player is holding it

            float dist = Vector3.Distance(transform.position, hook.transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = hook;
            }
        }

        // Update visual feedback
        if (nearest != null && nearestDist <= attachRadius)
        {
            SetIndicatorColor(readyColor);
            _hookInRange = nearest;
            AttachHook(nearest);
        }
        else
        {
            SetIndicatorColor(idleColor);
            _hookInRange = null;
        }
    }

    private void AttachHook(HookItem hook)
    {
        _hookAttached = true;
        hook.AttachToPoint(transform);
        if (attachSound != null)
            attachSound.Play();

        SetIndicatorColor(readyColor);

        Debug.Log($"[LiftAttachPoint] Hook attached! Loading {nextSceneName} in {transitionDelay}s");
        Invoke(nameof(LoadNextScene), transitionDelay);
    }

    private void LoadNextScene()
    {
        // Route through the state machine so prerequisites are enforced,
        // the GameState is updated, and the MQTT SCENE_ENTER event is fired.
        // GameStateManager.NotifyLiftBoarded() calls ChangeState(Scene3_1_Lift)
        // which loads the scene via its own LoadScene — nextSceneName is no longer used.
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.NotifyLiftBoarded();
        }
        else
        {
            // Fallback for scenes where GameStateManager is not present (e.g. editor testing).
            Debug.LogWarning("[LiftAttachPoint] GameStateManager not found — loading scene directly.");
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void SetIndicatorColor(Color color)
    {
        if (indicatorRenderer != null)
            indicatorRenderer.material.color = color;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attachRadius);
    }
}