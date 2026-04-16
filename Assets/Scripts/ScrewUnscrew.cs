using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ScrewUnscrew : MonoBehaviour
{
    private bool isScrewed = true;

    private GameObject lightcover = null;
    private GameObject old_armature = null;
    private GameObject new_armature = null;

    private GameObject[] screws = new GameObject[4]; // Assuming screws are child objects of the light cover

    private void Awake()
    {
        lightcover = this.gameObject; // Assuming the script is attached to the light cover object
        old_armature = GameObject.Find("Light Armature Old"); // Find the armature in the scene
        new_armature = GameObject.Find("Light Armature New"); // Find the new armature in the scene

        for (int i = 0; i < 4; i++)
        {
            screws[i] = lightcover.transform.GetChild(i+1).gameObject; // Assuming screws are the 2nd, 3rd, 4th and 5th children of the light cover
        }

        Debug.Log("The object is initially screwed.");
    }

    public void ToggleScrew()
    {
        isScrewed = !isScrewed;
        if (isScrewed)
        {
            lightcover.GetComponent<XRGrabInteractable>().enabled = false; // Disable interaction when screwed
            old_armature.GetComponent<XRGrabInteractable>().enabled = false; // Disable interaction for the old armature when screwed
            new_armature.GetComponent<XRGrabInteractable>().enabled = false; // Disable interaction for the new armature when screwed
            Debug.Log("The object is now screwed.");

            for (int i = 0; i < screws.Length; i++)
            {
                screws[i].transform.localPosition = new Vector3(screws[i].transform.localPosition.x, screws[i].transform.localPosition.y + 0.2f, screws[i].transform.localPosition.z); 
            }
        }
        else
        {
            lightcover.GetComponent<XRGrabInteractable>().enabled = true; // Enable interaction when unscrewed
            old_armature.GetComponent<XRGrabInteractable>().enabled = true; // Enable interaction for the old armature when unscrewed
            new_armature.GetComponent<XRGrabInteractable>().enabled = true; // Enable interaction for the new armature when unscrewed

            Debug.Log("The object is now unscrewed.");

            for (int i = 0; i < screws.Length; i++)
            {
                screws[i].transform.localPosition = new Vector3(screws[i].transform.localPosition.x, screws[i].transform.localPosition.y - 0.2f, screws[i].transform.localPosition.z);
            }
        }
    }

}

