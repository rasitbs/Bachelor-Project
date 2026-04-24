using UnityEngine;
using Oculus.Interaction;
using System.Collections.Generic;

public class LightCoverObserver : MonoBehaviour
{
    [SerializeField] private SnapInteractable _snapZone;
    [SerializeField] private List<Grabbable> _armatureGrabbables;

    private void OnEnable()
    {
        // Subscribe to the event using the event syntax, not property assignment
        _snapZone.WhenSelectingInteractorRemoved.Action += HandleLightCoverRemoved;
    }

    private void OnDisable()
    {
        _snapZone.WhenSelectingInteractorRemoved.Action -= HandleLightCoverRemoved;
    }

    private void HandleLightCoverRemoved(SnapInteractor interactor)
    {
        // Enable all grabbable scripts in the list
        foreach (var grabbable in _armatureGrabbables)
        {
            if (grabbable != null)
            {
                grabbable.enabled = true;
            }
        }

        Debug.Log("Light Cover removed: Armatures are now grabbable.");
    }
}