using UnityEngine;

/// <summary>
/// Debug utility for testing GameStateManager transitions without
/// needing to complete full VR interactions.
///
/// Attach to the same GameObject as GameStateManager in the Main scene.
///
/// Two ways to trigger:
///   1. Right-click the component header in the Inspector (ContextMenu)
///   2. Keyboard shortcuts while in Play mode (see below)
///
/// Keyboard shortcuts (Play mode):
///   1  →  Force Scene1_PPE
///   2  →  Force Scene2_SJA
///   3  →  Force Scene3_PreLift
///   4  →  Force Scene3_1_Lift
///   5  →  Force Completed
///   S  →  Notify: SJA Completed
///   Q  →  Notify: Quiz Completed
///   G  →  Notify: Gloves Equipped
///   T  →  Notify: Voltage Checked To
///   R  →  Notify: Voltage Checked From
///   L  →  Notify: Lift Boarded
///   P  →  Notify: Scene 2 Proceed
///   F  →  Notify: Scene 3-1 Proceed
///
/// Remove this component before building for production.
/// </summary>
public class GameStateDebugger : MonoBehaviour
{
    [Header("Live state — updates every frame in Play mode")]
    [SerializeField] private string _currentState      = "not running";
    [SerializeField] private bool   _glovesEquipped;
    [SerializeField] private bool   _checkedVoltageTo;
    [SerializeField] private bool   _checkedVoltageFrom;
    [SerializeField] private bool   _fuseVerified;
    [SerializeField] private bool   _scene3AllMet;

    [Header("Settings")]
    [SerializeField] private bool enableKeyboardShortcuts = true;

    // ── Unity lifecycle ───────────────────────────────────────────────────────

    private void Update()
    {
        RefreshDisplay();

        if (!enableKeyboardShortcuts) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) ForceScene1_PPE();
        if (Input.GetKeyDown(KeyCode.Alpha2)) ForceScene2_SJA();
        if (Input.GetKeyDown(KeyCode.Alpha3)) ForceScene3_PreLift();
        if (Input.GetKeyDown(KeyCode.Alpha4)) ForceScene3_1_Lift();
        if (Input.GetKeyDown(KeyCode.Alpha5)) ForceCompleted();

        if (Input.GetKeyDown(KeyCode.S)) NotifySJA();
        if (Input.GetKeyDown(KeyCode.Q)) NotifyQuiz();
        if (Input.GetKeyDown(KeyCode.G)) NotifyGloves();
        if (Input.GetKeyDown(KeyCode.T)) NotifyVoltageTo();
        if (Input.GetKeyDown(KeyCode.R)) NotifyVoltageFrom();
        if (Input.GetKeyDown(KeyCode.L)) NotifyLift();
        if (Input.GetKeyDown(KeyCode.P)) NotifyScene2Proceed();
        if (Input.GetKeyDown(KeyCode.F)) NotifyScene3_1Proceed();
    }

    // ── Force state transitions ───────────────────────────────────────────────

    [ContextMenu("Force → Idle")]
    public void ForceIdle() => ForceState(GameState.Idle);

    [ContextMenu("Force → Scene1_PPE")]
    public void ForceScene1_PPE() => ForceState(GameState.Scene1_PPE);

    [ContextMenu("Force → Scene2_SJA")]
    public void ForceScene2_SJA() => ForceState(GameState.Scene2_SJA);

    [ContextMenu("Force → Scene3_PreLift")]
    public void ForceScene3_PreLift() => ForceState(GameState.Scene3_PreLift);

    [ContextMenu("Force → Scene3_1_Lift")]
    public void ForceScene3_1_Lift() => ForceState(GameState.Scene3_1_Lift);

    [ContextMenu("Force → Completed")]
    public void ForceCompleted() => ForceState(GameState.Completed);

    // ── Notify gate signals ───────────────────────────────────────────────────

    [ContextMenu("Notify: SJA Completed  [S]")]
    public void NotifySJA()
    {
        Debug.Log("[GameStateDebugger] Notifying SJA completed.");
        GameStateManager.Instance?.NotifySJACompleted();
    }

    [ContextMenu("Notify: Quiz Completed  [Q]")]
    public void NotifyQuiz()
    {
        Debug.Log("[GameStateDebugger] Notifying quiz completed.");
        GameStateManager.Instance?.NotifyQuizCompleted();
    }

    [ContextMenu("Notify: Gloves Equipped  [G]")]
    public void NotifyGloves()
    {
        Debug.Log("[GameStateDebugger] Notifying gloves equipped.");
        GameStateManager.Instance?.NotifyGlovesEquipped();
    }

    [ContextMenu("Notify: Voltage Checked To  [T]")]
    public void NotifyVoltageTo()
    {
        Debug.Log("[GameStateDebugger] Notifying voltage checked To.");
        GameStateManager.Instance?.NotifyVoltageCheckedTo();
    }

    [ContextMenu("Notify: Voltage Checked From  [R]")]
    public void NotifyVoltageFrom()
    {
        Debug.Log("[GameStateDebugger] Notifying voltage checked From.");
        GameStateManager.Instance?.NotifyVoltageCheckedFrom();
    }

    [ContextMenu("Notify: Lift Boarded  [L]")]
    public void NotifyLift()
    {
        Debug.Log("[GameStateDebugger] Notifying lift boarded.");
        GameStateManager.Instance?.NotifyLiftBoarded();
    }

    [ContextMenu("Notify: Scene 2 Proceed Pressed  [P]")]
    public void NotifyScene2Proceed()
    {
        Debug.Log("[GameStateDebugger] Notifying Scene 2 proceed pressed.");
        GameStateManager.Instance?.NotifyScene2ProceedPressed();
    }

    [ContextMenu("Notify: Armature Installed")]
    public void NotifyArmatureInstalled()
    {
        Debug.Log("[GameStateDebugger] Notifying armature installed.");
        GameStateManager.Instance?.NotifyArmatureInstalled();
    }

    [ContextMenu("Notify: Scene 3-1 Proceed Pressed  [F]")]
    public void NotifyScene3_1Proceed()
    {
        Debug.Log("[GameStateDebugger] Notifying Scene 3-1 proceed pressed.");
        GameStateManager.Instance?.NotifyScene3_1ProceedPressed();
    }

    // ── Bypass combos ─────────────────────────────────────────────────────────

    [ContextMenu("Bypass: Complete Scene 2 (SJA + Quiz + Proceed)")]
    public void BypassScene2()
    {
        Debug.Log("[GameStateDebugger] Bypassing Scene 2 gates.");
        GameStateManager.Instance?.NotifySJACompleted();
        GameStateManager.Instance?.NotifyQuizCompleted();
        GameStateManager.Instance?.NotifyScene2ProceedPressed();
    }

    [ContextMenu("Bypass: Complete Scene 3 checklist (Gloves + To + From)")]
    public void BypassScene3Prereqs()
    {
        Debug.Log("[GameStateDebugger] Bypassing Scene 3 checklist.");
        GameStateManager.Instance?.NotifyGlovesEquipped();
        GameStateManager.Instance?.NotifyVoltageCheckedTo();
        GameStateManager.Instance?.NotifyVoltageCheckedFrom();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void ForceState(GameState state)
    {
        if (GameStateManager.Instance == null)
        {
            Debug.LogWarning("[GameStateDebugger] GameStateManager.Instance is null.");
            return;
        }
        Debug.Log($"[GameStateDebugger] Forcing state → {state}");
        GameStateManager.Instance.ChangeState(state);
    }

    private void RefreshDisplay()
    {
        if (GameStateManager.Instance == null)
        {
            _currentState       = "GameStateManager not found";
            _glovesEquipped     = false;
            _checkedVoltageTo   = false;
            _checkedVoltageFrom = false;
            _fuseVerified       = false;
            _scene3AllMet       = false;
            return;
        }

        _currentState       = GameStateManager.Instance.CurrentState.ToString();
        _glovesEquipped     = GameStateManager.Instance.IsGlovesEquipped;
        _checkedVoltageTo   = GameStateManager.Instance.IsCheckedVoltageTo;
        _checkedVoltageFrom = GameStateManager.Instance.IsCheckedVoltageFrom;
        _fuseVerified       = GameStateManager.Instance.IsFuseVerified;
        _scene3AllMet       = GameStateManager.Instance.IsScene3PrerequisitesMet;
    }
}
