using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ScrewUnscrew : MonoBehaviour
{
    private bool isScrewed = true;

    private GameObject lightcover = null; 

    private GameObject[] screws = new GameObject[4]; // Assuming screws are child objects of the light cover

    private void Awake()
    {
        lightcover = this.gameObject; // Assuming the script is attached to the light cover object

        for (int i = 0; i < 4; i++)
        {
            screws[i] = lightcover.transform.GetChild(i+1).gameObject; // Assuming screws are the 2nd, 3rd, 4th and 5th children of the light cover
        }

        // Initialize the object in the screwed state
        isScrewed = true;
        Debug.Log("The object is initially screwed.");
    }

    public void ToggleScrew()
    {
        isScrewed = !isScrewed;
        if (isScrewed)
        {
            lightcover.GetComponent<XRGrabInteractable>().enabled = false; // Disable interaction when screwed
            Debug.Log("The object is now screwed.");

            for (int i = 0; i < screws.Length; i++)
            {
                screws[i].transform.localPosition = new Vector3(screws[i].transform.localPosition.x, screws[i].transform.localPosition.y + 2.0f, screws[i].transform.localPosition.z); 
            }
        }
        else
        {
            lightcover.GetComponent<XRGrabInteractable>().enabled = true; // Enable interaction when unscrewed

            Debug.Log("The object is now unscrewed.");

            for (int i = 0; i < screws.Length; i++)
            {
                screws[i].transform.localPosition = new Vector3(screws[i].transform.localPosition.x, screws[i].transform.localPosition.y - 2.0f, screws[i].transform.localPosition.z);
            }
        }
    }

}

