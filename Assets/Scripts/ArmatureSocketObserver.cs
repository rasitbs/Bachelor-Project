using UnityEngine;
using Oculus.Interaction;

public class ArmatureSocketObserver : MonoBehaviour
{
    public System.Action OnStateChanged;

    [Header("References")]
    [SerializeField] private SnapInteractable _armatureSocket;

    private bool _armatureInstalled = false;
    public bool isLightArmatureNew = false;

    private void Awake()
    {
        if (_armatureSocket != null) return;

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
        _armatureSocket.WhenSelectingInteractorViewRemoved += HandleArmatureRemoved;
    }

    private void OnDisable()
    {
        if (_armatureSocket == null) return;
        _armatureSocket.WhenSelectingInteractorViewAdded -= HandleArmatureInstalled;
        _armatureSocket.WhenSelectingInteractorViewRemoved -= HandleArmatureRemoved;
    }

    private void HandleArmatureInstalled(IInteractorView interactor)
    {
        _armatureInstalled = true;

        MonoBehaviour interactorComponent = interactor as MonoBehaviour;

        if (interactorComponent != null)
        {
            GameObject snappedObject = interactorComponent.gameObject;

            if (snappedObject.name.Contains("Light Armature New") ||
               (snappedObject.transform.parent != null && snappedObject.transform.parent.name.Contains("Light Armature New")) ||
               (snappedObject.transform.root.name.Contains("Light Armature New")))
            {
                isLightArmatureNew = true;
            }
        }
        else
        {
            Debug.LogWarning("[ArmatureSocketObserver] Could not cast Interactor to MonoBehaviour.");
        }

        Debug.Log($"[ArmatureSocketObserver] Armature installed. Is New: {isLightArmatureNew}");

        OnStateChanged?.Invoke();
        GameStateManager.Instance?.NotifyArmatureInstalled();
    }

    private void HandleArmatureRemoved(IInteractorView interactor)
    {
        _armatureInstalled = false;
        isLightArmatureNew = false;

        Debug.Log("[ArmatureSocketObserver] Armature removed.");

        OnStateChanged?.Invoke();
    }
}