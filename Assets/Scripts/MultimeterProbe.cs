using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MultimeterProbe : MonoBehaviour
{
    public string currentSocketTag = "None";

    public void OnSocketConnect(SelectEnterEventArgs args)
    {
        currentSocketTag = args.interactorObject.transform.gameObject.tag;
    }

    public void OnSocketDisconnect(SelectExitEventArgs args)
    {
        currentSocketTag = "None";
    }
}
