using UnityEngine;
using TMPro;

public class MultimeterScreenUpdater : MonoBehaviour
{
    [SerializeField]
    private GameObject multimeterScreen;
    [SerializeField]
    private GameObject redplug;
    [SerializeField]
    private GameObject blackplug;

    public TextMeshProUGUI voltageText;
    public bool isMeasuringVoltage = false;

    void Start()
    {
        multimeterScreen = this.gameObject;

        // Find the red and black plugs in the scene
        if (redplug == null)    
            redplug = GameObject.Find("RedWirePlug");
        if (blackplug == null)
            blackplug = GameObject.Find("BlackWirePlug");

        if (redplug == null || blackplug == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Red or Black plug not found in the scene. Please ensure they are named 'RedWirePlug' and 'BlackWirePlug'.");
#endif
        }

    }

    void updateScreen()
    {
        if (isMeasuringVoltage)
        {
            voltageText.text = "230V"; // Placeholder 
        }
    }
}
