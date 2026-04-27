using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Central state machine for the Traftec HSE training simulator.
///
/// Responsibilities:
///   - Owns the authoritative GameState for the running session.
///   - Fires the appropriate MQTT telemetry events via EventService on every
///     state transition.
///   - Loads the corresponding Unity scene when entering a new state.
///   - Enforces the Scene 2 dual-completion gate: BOTH the SJA (hazard marking)
///     AND the risk-assessment quiz must finish before advancing to Scene 3.
///
/// Usage (from any scene script):
///   GameStateManager.Instance.ChangeState(GameState.Scene2_SJA);
///   GameStateManager.Instance.NotifySJACompleted();
///   GameStateManager.Instance.NotifyQuizCompleted();
///
/// This MonoBehaviour lives on a DontDestroyOnLoad GameObject and persists
/// for the entire application lifetime.
/// </summary>
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    /// <summary>Read-only view of the current simulation state.</summary>
    public GameState CurrentState { get; private set; } = GameState.Idle;

    // ── Scene name configuration ──────────────────────────────────────────────
    // These must match the scene names registered in File → Build Settings.
    // They are exposed in the Inspector so the names can be updated without
    // touching code if scenes are renamed.
    [Header("Scene Names (must match Build Settings exactly)")]
    [SerializeField] private string sceneName_PPE      = "Scene1_PPE";
    [SerializeField] private string sceneName_SJA      = "Scene2_SJA";
    [SerializeField] private string sceneName_PreLift  = "Scene3_PreLift";
    [SerializeField] private string sceneName_Lift     = "Scene3_1_Lift";
    [SerializeField] private string sceneName_Final    = "Final";

    // ── Scene 2 dual-completion gate ──────────────────────────────────────────
    // Both flags must be true before TryAdvanceFromScene2 fires ChangeState.
    // They are reset every time Scene2_SJA is entered (supports replay / retry).
    private bool _sjaCompleted;
    private bool _quizCompleted;

    // ── Scene 3 prerequisites ─────────────────────────────────────────────────
    // The player must equip gloves AND confirm 230 V at the fuse box before
    // they are allowed to board the aerial lift. Unlike Scene 2, the state does
    // NOT advance automatically when both flags are set — the player still has to
    // physically board the lift (NotifyLiftBoarded triggers the transition).
    private bool _glovesEquipped;
    private bool _voltageVerified;

    /// <summary>
    /// True when both Scene 3 prerequisites are satisfied.
    /// The lift-boarding script should check this before allowing the player to attach.
    /// </summary>
    public bool IsScene3PrerequisitesMet => _glovesEquipped && _voltageVerified;

    // ── Unity lifecycle ───────────────────────────────────────────────────────

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

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Transitions the simulation to <paramref name="newState"/>.
    /// Fires exit events for the current state, fires enter events for the new
    /// state, and loads the corresponding Unity scene.
    /// </summary>
    public void ChangeState(GameState newState)
    {
        if (newState == CurrentState)
        {
            Debug.LogWarning($"[GameStateManager] Already in state {newState} — transition ignored.");
            return;
        }

        Debug.Log($"[GameStateManager] Transition: {CurrentState} → {newState}");

        OnExitState(CurrentState);
        CurrentState = newState;
        OnEnterState(CurrentState);
    }

    /// <summary>
    /// Call this from the SJA (hazard-marking) system when the player has
    /// finished marking all hazards in Scene 2.
    /// Automatically advances to Scene 3 if the quiz is also complete.
    /// </summary>
    public void NotifySJACompleted()
    {
        if (CurrentState != GameState.Scene2_SJA)
        {
            Debug.LogWarning("[GameStateManager] NotifySJACompleted called outside Scene2_SJA — ignored.");
            return;
        }

        _sjaCompleted = true;
        Debug.Log("[GameStateManager] SJA hazard marking completed.");
        TryAdvanceFromScene2();
    }

    /// <summary>
    /// Call this from the risk-assessment quiz system when the player submits
    /// their answers in Scene 2.
    /// Automatically advances to Scene 3 if the SJA is also complete.
    /// </summary>
    public void NotifyQuizCompleted()
    {
        if (CurrentState != GameState.Scene2_SJA)
        {
            Debug.LogWarning("[GameStateManager] NotifyQuizCompleted called outside Scene2_SJA — ignored.");
            return;
        }

        _quizCompleted = true;
        Debug.Log("[GameStateManager] Risk-assessment quiz completed.");
        TryAdvanceFromScene2();
    }

    /// <summary>
    /// Call this from the gloves script when the player equips gloves before
    /// working at the fuse box in Scene 3.
    /// </summary>
    public void NotifyGlovesEquipped()
    {
        if (CurrentState != GameState.Scene3_PreLift)
        {
            Debug.LogWarning("[GameStateManager] NotifyGlovesEquipped called outside Scene3_PreLift — ignored.");
            return;
        }

        _glovesEquipped = true;
        Debug.Log($"[GameStateManager] Scene 3 progress — Gloves: {_glovesEquipped}, Voltage: {_voltageVerified}");
    }

    /// <summary>
    /// Call this from MultimeterScreenUpdater the first time the display reads 230 V,
    /// confirming the fuse box is live and the voltage check step is complete.
    /// </summary>
    public void NotifyVoltageVerified()
    {
        if (CurrentState != GameState.Scene3_PreLift)
        {
            Debug.LogWarning("[GameStateManager] NotifyVoltageVerified called outside Scene3_PreLift — ignored.");
            return;
        }

        _voltageVerified = true;
        Debug.Log($"[GameStateManager] Scene 3 progress — Gloves: {_glovesEquipped}, Voltage: {_voltageVerified}");
    }

    /// <summary>
    /// Call this from the lift-boarding trigger when the player attaches the
    /// harness and boards the aerial lift at the end of Scene 3.
    /// Transitions to Scene3_1_Lift only if both prerequisites are satisfied.
    /// If they are not, the call is ignored with a warning — use
    /// IsScene3PrerequisitesMet to disable the boarding interaction in the UI.
    /// </summary>
    public void NotifyLiftBoarded()
    {
        if (CurrentState != GameState.Scene3_PreLift)
        {
            Debug.LogWarning("[GameStateManager] NotifyLiftBoarded called outside Scene3_PreLift — ignored.");
            return;
        }

        if (!_glovesEquipped || !_voltageVerified)
        {
            Debug.LogWarning($"[GameStateManager] Lift boarding blocked — prerequisites not met. " +
                             $"Gloves: {_glovesEquipped}, Voltage: {_voltageVerified}");
            return;
        }

        Debug.Log("[GameStateManager] Scene 3 complete — boarding the aerial lift.");
        ChangeState(GameState.Scene3_1_Lift);
    }

    // ── Private state logic ───────────────────────────────────────────────────

    /// <summary>
    /// Advances past Scene 2 only when both the SJA and the quiz are done.
    /// </summary>
    private void TryAdvanceFromScene2()
    {
        if (_sjaCompleted && _quizCompleted)
        {
            Debug.Log("[GameStateManager] Both Scene 2 conditions met — advancing to Scene 3.");
            ChangeState(GameState.Scene3_PreLift);
        }
        else
        {
            Debug.Log($"[GameStateManager] Scene 2 progress — SJA: {_sjaCompleted}, Quiz: {_quizCompleted}");
        }
    }

    /// <summary>
    /// Called immediately after CurrentState is updated.
    /// Fires the MQTT enter-event and loads the scene for the new state.
    /// </summary>
    private void OnEnterState(GameState state)
    {
        switch (state)
        {
            case GameState.Idle:
                // Main menu / lobby — no session is active, no MQTT event needed.
                break;

            case GameState.Scene1_PPE:
                // A new training session starts when the player enters the PPE van.
                EventService.Instance?.PublishSessionStarted("PPE_SELECTION");
                LoadScene(sceneName_PPE);
                break;

            case GameState.Scene2_SJA:
                // Reset both completion flags so a replayed scene starts clean.
                _sjaCompleted  = false;
                _quizCompleted = false;
                EventService.Instance?.PublishSceneEntered(2, "SJA_RISK_ASSESSMENT");
                LoadScene(sceneName_SJA);
                break;

            case GameState.Scene3_PreLift:
                // Reset prerequisites so a replayed scene always starts from scratch.
                _glovesEquipped = false;
                _voltageVerified = false;
                EventService.Instance?.PublishSceneEntered(3, "PRE_LIFT_PREPARATIONS");
                LoadScene(sceneName_PreLift);
                break;

            case GameState.Scene3_1_Lift:
                EventService.Instance?.PublishSceneEntered(4, "AERIAL_LIFT_WORK");
                LoadScene(sceneName_Lift);
                break;

            case GameState.Completed:
                // Ask the backend to compute and return the final score, then
                // publish SESSION_END to close the telemetry record.
                EventService.Instance?.RequestFinalScore();
                EventService.Instance?.PublishSessionEnded();
                LoadScene(sceneName_Final);
                break;
        }
    }

    /// <summary>
    /// Called immediately before CurrentState is updated.
    /// Most per-scene events (kit picks, hazard marks, quiz answers) are
    /// published directly by their own scene scripts — they are listed here
    /// as comments so the flow is easy to follow during a thesis review.
    /// </summary>
    private void OnExitState(GameState state)
    {
        switch (state)
        {
            case GameState.Idle:
                break;

            case GameState.Scene1_PPE:
                // KIT_SELECTION events are published per-item by PPESceneController,
                // not as a single bulk event here, so individual choices are recorded.
                break;

            case GameState.Scene2_SJA:
                // HAZARD_MARKED events are published by SJAManager.
                // RISK_ASSESSMENT_COMPLETED is published by RiskAssessmentManager.
                break;

            case GameState.Scene3_PreLift:
                // ACTION_INTERACT events for glove equip and voltage check are
                // published by the gloves script and MultimeterScreenUpdater respectively.
                break;

            case GameState.Scene3_1_Lift:
                // ACTION_INTERACT events for each tool use are published by the
                // lift-work scene scripts.
                break;

            case GameState.Completed:
                break;
        }
    }

    // ── Scene name → state mapping ───────────────────────────────────────────

    /// <summary>
    /// Tries to map <paramref name="sceneName"/> to a GameState and calls
    /// ChangeState if a match is found. Returns true when handled.
    /// Used by SceneLoader so existing Inspector button wiring does not need
    /// to be replaced — calls that pass through SceneLoader are automatically
    /// intercepted and routed through the state machine.
    /// </summary>
    public bool TryTransitionToScene(string sceneName)
    {
        if (sceneName == sceneName_PPE)      { ChangeState(GameState.Scene1_PPE);      return true; }
        if (sceneName == sceneName_SJA)      { ChangeState(GameState.Scene2_SJA);      return true; }
        if (sceneName == sceneName_PreLift)  { ChangeState(GameState.Scene3_PreLift);  return true; }
        if (sceneName == sceneName_Lift)     { ChangeState(GameState.Scene3_1_Lift);   return true; }
        if (sceneName == sceneName_Final)    { ChangeState(GameState.Completed);       return true; }
        return false;
    }

    // ── Scene loading ─────────────────────────────────────────────────────────

    /// <summary>
    /// Loads a Unity scene by name.
    /// Skips the load if the scene is already active to avoid a redundant
    /// reload (e.g. when ChangeState is called from a scene's own Awake).
    /// </summary>
    private void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("[GameStateManager] Scene name is empty — load skipped.");
            return;
        }

        if (SceneManager.GetActiveScene().name == sceneName)
        {
            Debug.Log($"[GameStateManager] Scene '{sceneName}' is already active — reload skipped.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }
}
