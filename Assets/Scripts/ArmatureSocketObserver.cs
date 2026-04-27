using UnityEngine;
using Oculus.Interaction;

/// <summary>
/// Watches the armature snap socket in Scene 3-1.
/// When the player installs the replacement armature (snaps it into the socket),
/// this triggers the final state transition to Completed after a short delay.
///
/// Setup: assign the SnapInteractable on the armature socket to _armatureSocket
/// in the Inspector.
/// </summary>
public class ArmatureSocketObserver : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SnapInteractable _armatureSocket;

    [Header("Transition")]
    [Tooltip("Seconds to wait after armature is installed before loading the Final scene.")]
    [SerializeField] private float completionDelay = 2f;

    private bool _armatureInstalled = false;

    private void Awake()
    {
        if (_armatureSocket != null) return;

        // Mirror the auto-find pattern used by MultimeterScreenUpdater so the
        // Inspector field is optional — no manual assignment needed.
        GameObject socketObj = GameObject.Find("Armature Socket");
        if (socketObj != null)
            _armatureSocket = socketObj.GetComponentInChildren<SnapInteractable>();

        if (_armatureSocket == null)
            Debug.LogWarning("[ArmatureSocketObserver] Could not find 'Armature Socket' — assign it in the Inspector.");
    }

    private void OnEnable()
    {
        if (_armatureSocket == null) return;
        _armatureSocket.WhenSelectingInteractorViewAdded += HandleArmatureInstalled;
    }

    private void OnDisable()
    {
        if (_armatureSocket == null) return;
        _armatureSocket.WhenSelectingInteractorViewAdded -= HandleArmatureInstalled;
    }

    private void HandleArmatureInstalled(IInteractorView interactor)
    {
        if (_armatureInstalled) return;
        _armatureInstalled = true;

        Debug.Log($"[ArmatureSocketObserver] Armature installed by {interactor.Identifier} — " +
                  $"completing scene in {completionDelay}s.");

        Invoke(nameof(CompleteScene), completionDelay);
    }

    private void CompleteScene()
    {
        GameStateManager.Instance?.ChangeState(GameState.Completed);
    }
}
