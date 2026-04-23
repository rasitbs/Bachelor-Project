using UnityEngine;
using Oculus.Interaction;

public class MultimeterProbe : MonoBehaviour
{
    public string currentSocketName = "None";

    // Drag your SnapInteractor (from the Probe) into this slot in the Inspector
    public SnapInteractor snapInteractor;

    public void OnSocketConnect()
    {
        // We check what the Interactor is currently selecting
        if (snapInteractor != null && snapInteractor.SelectedInteractable != null)
        {
            // This is the SelectInteractable object
            Transform socketTransform = snapInteractor.SelectedInteractable.transform;

            // Get the parent: 'Hot Point Black From'
            if (socketTransform.parent != null)
            {
                currentSocketName = socketTransform.parent.name;
            }
            else
            {
                currentSocketName = socketTransform.name;
            }
        }

#if UNITY_EDITOR
        Debug.Log("Probe successfully fetched name: " + currentSocketName);
#endif
    }

    public void OnSocketDisconnect()
    {
        currentSocketName = "None";
    }
}