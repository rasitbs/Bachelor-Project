using Oculus.Interaction;
using UnityEngine;

public class MultimeterProbe : MonoBehaviour
{
    public string currentSocketName = "None";
    public GameObject currentOwner = null; // Store the actual armature object

    public SnapInteractor snapInteractor;

    public void OnSocketConnect()
    {
        if (snapInteractor != null && snapInteractor.SelectedInteractable != null)
        {
            Transform socketTransform = snapInteractor.SelectedInteractable.transform;

            // Store the "Root" or "Parent" to identify the specific armature
            currentOwner = socketTransform.root.gameObject;
#if UNITY_EDITOR
            Debug.Log($"MultimeterProbe: Connected to {currentOwner.name}");
#endif

            if (socketTransform.parent != null)
                currentSocketName = socketTransform.parent.name;
            else
                currentSocketName = socketTransform.name;
        }
    }

    public void OnSocketDisconnect()
    {
        currentSocketName = "None";
        currentOwner = null;
    }
}