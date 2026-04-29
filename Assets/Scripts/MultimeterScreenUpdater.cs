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
    private bool isScene3;

    public bool hasCheckedTo;
    public bool hasCheckedFrom;
    // Ensures NotifyVoltageVerified is only called once per Scene 3 session.
    private bool _voltageNotifiedThisScene;

    void Start()
    {
        isScene31 = SceneManager.GetActiveScene().name == "Scene 3-1";
#if UNITY_EDITOR
        Debug.Log($"MultimeterScreenUpdater: Detected Scene 3-1: {isScene31}");
#endif

        isScene3 = SceneManager.GetActiveScene().name == "Scene 3";
#if UNITY_EDITOR
        Debug.Log($"MultimeterScreenUpdater: Detected Scene 3: {isScene3}");
#endif

        // References
        if (redProbe == null) redProbe = GameObject.Find("RedWirePlug").GetComponent<MultimeterProbe>();
        if (blackProbe == null) blackProbe = GameObject.Find("BlackWirePlug").GetComponent<MultimeterProbe>();
        if (voltageText == null) voltageText = GameObject.Find("Screen").GetComponent<TextMeshProUGUI>();
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

        hasCheckedFrom = false;
        hasCheckedTo = false;
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
            if (isCorrectTo)
            {
                // Has checked to is set to correct, point manager checks list and ensures Oppgave 2 is current task and sets green
                hasCheckedTo = true;
            }

            if (isCorrectFrom)
            {
                // Has checked from is set to correct, point manager checks list and ensures Oppgave 3 is current task and sets green
                hasCheckedFrom = true;
            }
            
            voltageText.text = "230V";

            // In Scene 3 only: notify the state machine the first time 230V is confirmed.
            // Scene 3-1 reuses this multimeter for the bulb swap, so we skip that scene.
            if (!isScene31 && !_voltageNotifiedThisScene)
            {
                _voltageNotifiedThisScene = true;
                GameStateManager.Instance?.NotifyVoltageVerified();
            }
        }
        else
        {
            voltageText.text = "---V";
        }
    }
}