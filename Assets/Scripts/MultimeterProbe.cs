using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MultimeterProbe : MonoBehaviour
{
    public string currentSocketName = "None";

    public void OnSocketConnect(SelectEnterEventArgs args)
    {
        // We grab the .name of the GameObject the Socket is attached to
        currentSocketName = args.interactorObject.transform.gameObject.name;

#if UNITY_EDITOR
        Debug.Log("Connected to: " + currentSocketName);
#endif
    }

    public void OnSocketDisconnect(SelectExitEventArgs args)
    {
        currentSocketName = "None";
    }
}