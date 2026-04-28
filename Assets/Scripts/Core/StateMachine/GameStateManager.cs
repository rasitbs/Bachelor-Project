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
    [SerializeField] private string sceneName_PreLift  = "3";
    [SerializeField] private string sceneName_Lift     = "3-1";
    [SerializeField] private string sceneName_Final    = "Final";

    // ── Scene 2 dual-completion gate ──────────────────────────────────────────
    // Both flags must be true before the player is allowed to proceed.
    // The state does NOT advance automatically — the player must press the
    // "next" button which calls NotifyScene2ProceedPressed().
    // They are reset every time Scene2_SJA is entered (supports replay / retry).
    private bool _sjaCompleted;
    private bool _quizCompleted;

    /// <summary>True when both SJA and quiz are done. Use this to enable the
    /// "proceed" button in Scene 2.</summary>
    public bool IsScene2Complete => _sjaCompleted && _quizCompleted;

    // ── Scene 3-1 completion gate ─────────────────────────────────────────────
    // The armature must be installed before the player is allowed to proceed.
    // Does NOT advance automatically — player must press the proceed button.
    private bool _armatureInstalled;

    /// <summary>True when the replacement armature has been installed.</summary>
    public bool IsScene3_1Complete => _armatureInstalled;

    /// <summary>
    /// Call this from ArmatureSocketObserver when the armature snaps into the socket.
    /// </summary>
    public void NotifyArmatureInstalled()
    {
        if (CurrentState != GameState.Scene3_1_Lift)
        {
            Debug.LogWarning("[GameStateManager] NotifyArmatureInstalled called outside Scene3_1_Lift — ignored.");
            return;
        }

        _armatureInstalled = true;
        Debug.Log("[GameStateManager] Armature installed.");
    }

    /// <summary>
    /// Call this from the Scene 3-1 proceed button after the armature is installed.
    /// Advances to Completed only if the armature has been installed.
    /// </summary>
    public void NotifyScene3_1ProceedPressed()
    {
        if (CurrentState != GameState.Scene3_1_Lift)
        {
            Debug.LogWarning("[GameStateManager] NotifyScene3_1ProceedPressed called outside Scene3_1_Lift — ignored.");
            return;
        }

        if (!_armatureInstalled)
        {
            Debug.LogWarning("[GameStateManager] Proceed blocked — armature not yet installed.");
            return;
        }

        Debug.Log("[GameStateManager] Scene 3-1 complete — player proceeding to Final.");
        ChangeState(GameState.Completed);
    }

    // ── Scene 3 prerequisites ─────────────────────────────────────────────────
    // All four checklist items must be completed before NotifyLiftBoarded()
    // accepts the carabiner attach. Does NOT auto-advance — the carabiner
    // interaction itself triggers the transition.
    //
    //   1. _glovesEquipped      — player puts on safety gloves
    //   2. _checkedVoltageTo    — multimeter placed on "To" side of fuse box (230 V)
    //   3. _checkedVoltageFrom  — multimeter placed on "From" side of fuse box (230 V)
    //   4. _fuseVerified        — auto-set when both voltage checks are done,
    //                             confirming the fuse conducts correctly
    private bool _glovesEquipped;
    private bool _checkedVoltageTo;
    private bool _checkedVoltageFrom;
    private bool _fuseVerified;

    public bool IsScene3PrerequisitesMet => _glovesEquipped && _checkedVoltageTo && _checkedVoltageFrom && _fuseVerified;
    public bool IsGlovesEquipped     => _glovesEquipped;
    public bool IsCheckedVoltageTo   => _checkedVoltageTo;
    public bool IsCheckedVoltageFrom => _checkedVoltageFrom;
    public bool IsFuseVerified       => _fuseVerified;

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
    /// </summary>
    public void NotifySJACompleted()
    {
        if (CurrentState != GameState.Scene2_SJA)
        {
            Debug.LogWarning("[GameStateManager] NotifySJACompleted called outside Scene2_SJA — ignored.");
            return;
        }

        _sjaCompleted = true;
        Debug.Log($"[GameStateManager] Scene 2 progress — SJA: {_sjaCompleted}, Quiz: {_quizCompleted}");
    }

    /// <summary>
    /// Call this from the risk-assessment quiz system when the player passes.
    /// </summary>
    public void NotifyQuizCompleted()
    {
        if (CurrentState != GameState.Scene2_SJA)
        {
            Debug.LogWarning("[GameStateManager] NotifyQuizCompleted called outside Scene2_SJA — ignored.");
            return;
        }

        _quizCompleted = true;
        Debug.Log($"[GameStateManager] Scene 2 progress — SJA: {_sjaCompleted}, Quiz: {_quizCompleted}");
    }

    /// <summary>
    /// Call this from the Scene 2 "next" button after the player has completed
    /// both the SJA and the quiz and manually chooses to proceed.
    /// Advances to Scene 3 only if both conditions are satisfied.
    /// </summary>
    public void NotifyScene2ProceedPressed()
    {
        if (CurrentState != GameState.Scene2_SJA)
        {
            Debug.LogWarning("[GameStateManager] NotifyScene2ProceedPressed called outside Scene2_SJA — ignored.");
            return;
        }

        if (!_sjaCompleted || !_quizCompleted)
        {
            Debug.LogWarning($"[GameStateManager] Proceed blocked — not all Scene 2 tasks done. " +
                             $"SJA: {_sjaCompleted}, Quiz: {_quizCompleted}");
            return;
        }

        Debug.Log("[GameStateManager] Scene 2 complete — player proceeding to Scene 3.");
        ChangeState(GameState.Scene3_PreLift);
    }

    /// <summary>
    /// Call this from the gloves canvas button when the player puts on safety gloves.
    /// </summary>
    public void NotifyGlovesEquipped()
    {
        if (CurrentState != GameState.Scene3_PreLift)
        {
            Debug.LogWarning("[GameStateManager] NotifyGlovesEquipped called outside Scene3_PreLift — ignored.");
            return;
        }
        _glovesEquipped = true;
        LogScene3Progress();
    }

    /// <summary>
    /// Call this from MultimeterScreenUpdater the first time probes are correctly
    /// placed on the TO side of the fuse box and read 230 V.
    /// </summary>
    public void NotifyVoltageCheckedTo()
    {
        if (CurrentState != GameState.Scene3_PreLift)
        {
            Debug.LogWarning("[GameStateManager] NotifyVoltageCheckedTo called outside Scene3_PreLift — ignored.");
            return;
        }
        _checkedVoltageTo = true;
        TryAutoVerifyFuse();
        LogScene3Progress();
    }

    /// <summary>
    /// Call this from MultimeterScreenUpdater the first time probes are correctly
    /// placed on the FROM side of the fuse box and read 230 V.
    /// </summary>
    public void NotifyVoltageCheckedFrom()
    {
        if (CurrentState != GameState.Scene3_PreLift)
        {
            Debug.LogWarning("[GameStateManager] NotifyVoltageCheckedFrom called outside Scene3_PreLift — ignored.");
            return;
        }
        _checkedVoltageFrom = true;
        TryAutoVerifyFuse();
        LogScene3Progress();
    }

    /// <summary>
    /// Call this from the lift carabiner attach trigger.
    /// Transitions to Scene3_1_Lift only if all four checklist items are done.
    /// Use IsScene3PrerequisitesMet to disable the carabiner interaction until ready.
    /// </summary>
    public void NotifyLiftBoarded()
    {
        if (CurrentState != GameState.Scene3_PreLift)
        {
            Debug.LogWarning("[GameStateManager] NotifyLiftBoarded called outside Scene3_PreLift — ignored.");
            return;
        }

        if (!IsScene3PrerequisitesMet)
        {
            Debug.LogWarning($"[GameStateManager] Carabiner blocked — checklist incomplete. " +
                             $"Gloves:{_glovesEquipped} To:{_checkedVoltageTo} " +
                             $"From:{_checkedVoltageFrom} Fuse:{_fuseVerified}");
            return;
        }

        Debug.Log("[GameStateManager] Scene 3 checklist complete — boarding the aerial lift.");
        ChangeState(GameState.Scene3_1_Lift);
    }

    // ── Private state logic ───────────────────────────────────────────────────

    // When both voltage checks are done the fuse is confirmed working — no
    // separate player interaction needed.
    private void TryAutoVerifyFuse()
    {
        if (_checkedVoltageTo && _checkedVoltageFrom)
        {
            _fuseVerified = true;
            Debug.Log("[GameStateManager] Fuse verified — both To and From measure 230 V.");
        }
    }

    private void LogScene3Progress()
    {
        Debug.Log($"[GameStateManager] Scene 3 checklist — " +
                  $"Gloves:{_glovesEquipped} To:{_checkedVoltageTo} " +
                  $"From:{_checkedVoltageFrom} Fuse:{_fuseVerified}");
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
                // Reset all four checklist flags so a replayed scene starts clean.
                _glovesEquipped     = false;
                _checkedVoltageTo   = false;
                _checkedVoltageFrom = false;
                _fuseVerified       = false;
                EventService.Instance?.PublishSceneEntered(3, "PRE_LIFT_PREPARATIONS");
                LoadScene(sceneName_PreLift);
                break;

            case GameState.Scene3_1_Lift:
                _armatureInstalled = false;
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
