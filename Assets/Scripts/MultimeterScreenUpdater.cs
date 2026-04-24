using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Oculus.Interaction; 

public class MultimeterScreenUpdater : MonoBehaviour
{
    [Header("Hardware References")]
    [SerializeField] private MultimeterProbe redProbe;
    [SerializeField] private MultimeterProbe blackProbe;
    [SerializeField] private BreakerSwitchFlipper breakerSwitch;

    [SerializeField] private SnapInteractable armatureAttachPoint;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI voltageText;

    private bool isScene31;

    void Start()
    {
        isScene31 = SceneManager.GetActiveScene().name == "3-1";

        // References
        if (redProbe == null) redProbe = GameObject.Find("RedWirePlug").GetComponent<MultimeterProbe>();
        if (blackProbe == null) blackProbe = GameObject.Find("BlackWirePlug").GetComponent<MultimeterProbe>();
        if (voltageText == null) voltageText = GameObject.Find("MultimeterScreen").GetComponent<TextMeshProUGUI>();
        if (breakerSwitch == null) breakerSwitch = GameObject.Find("Breaker Switch")?.GetComponent<BreakerSwitchFlipper>();

        // Find the Attach Point child if not assigned
        if (armatureAttachPoint == null)
        {
            GameObject socketObj = GameObject.Find("Armature Socket"); // Adjust name to parent object that holds the SnapInteractable
            if (socketObj != null)
            {
                armatureAttachPoint = socketObj.GetComponentInChildren<SnapInteractable>();
            }
        }
    }

    void Update()
    {
        // SCENE 3-1 EXCLUSIVE LOGIC
        if (isScene31)
        {
            // If the socket (Attach Point) has nothing snapped into it
            if (armatureAttachPoint == null || armatureAttachPoint.SelectingInteractors == null || armatureAttachPoint.SelectingInteractors.Count == 0)
            {
                voltageText.text = "---V";
                return; // Exit early: No armature, no voltage reading
            }
        }

        // STANDARD VOLTAGE LOGIC (Runs for Scene 3 OR if Scene 3-1 has an armature)
        bool isBreakerOn = breakerSwitch != null && breakerSwitch.isFlipped;
        bool blockToPower = isScene31 && isBreakerOn;

        bool isCorrectTo = !blockToPower &&
                           (redProbe.currentSocketName == "Hot Point Red To" &&
                            blackProbe.currentSocketName == "Hot Point Black To");

        bool isCorrectFrom = (isBreakerOn &&
                              redProbe.currentSocketName == "Hot Point Red From" &&
                              blackProbe.currentSocketName == "Hot Point Black From");

        bool isCorrectCross = (isBreakerOn && !blockToPower &&
                              redProbe.currentSocketName == "Hot Point Red From" &&
                              blackProbe.currentSocketName == "Hot Point Black To") ||
                             (isBreakerOn && !blockToPower &&
                              redProbe.currentSocketName == "Hot Point Red To" &&
                              blackProbe.currentSocketName == "Hot Point Black From");

        if (isCorrectTo || isCorrectFrom || isCorrectCross)
        {
            voltageText.text = "230V";
        }
        else
        {
            voltageText.text = "---V";
        }
    }
}