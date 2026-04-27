using UnityEngine;
using Oculus.Interaction;
using System.Collections.Generic;

public class LightCoverObserver : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SnapInteractable _snapZone;

    [SerializeField] private List<Grabbable> _armatureGrabbables;

    private void OnEnable()
    {
        if (_snapZone == null) return;

        _snapZone.WhenSelectingInteractorViewAdded += HandleLightCoverAdded;
        _snapZone.WhenSelectingInteractorViewRemoved += HandleLightCoverRemoved;
    }

    private void OnDisable()
    {
        if (_snapZone == null) return;

        _snapZone.WhenSelectingInteractorViewAdded -= HandleLightCoverAdded;
        _snapZone.WhenSelectingInteractorViewRemoved -= HandleLightCoverRemoved;
    }

    private void HandleLightCoverAdded(IInteractorView interactor)
    {
        SetArmaturesActive(false);
        Debug.Log($"[LightCoverObserver] {interactor.Identifier} Snapped: Grabbables Disabled");
    }

    private void HandleLightCoverRemoved(IInteractorView interactor)
    {
        SetArmaturesActive(true);
        Debug.Log("[LightCoverObserver] Cover Removed: Grabbables Enabled");
    }

    private void SetArmaturesActive(bool canMove)
    {
        foreach (var grabbable in _armatureGrabbables)
        {
            if (grabbable != null)
            {
                grabbable.enabled = canMove;
            }
        }
    }
}