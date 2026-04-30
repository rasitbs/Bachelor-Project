using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Oculus.Interaction;

/*
    Should only display voltages of 230V or "---V" based on probe placement and breaker state.
    Scene 3-1 special case: If the armature is not installed, always show "---V" regardless of probe placement, to encourage the player to install the armature to check voltage.
*/

public class MultimeterScreenUpdater : MonoBehaviour
{
    [Header("Hardware References")]
    [SerializeField] private MultimeterProbe redProbe;
    [SerializeField] private MultimeterProbe blackProbe;
    [SerializeField] private BreakerSwitchFlipper breakerSwitch;

    
    [SerializeField] private ArmatureSocketObserver armatureSocketObserver;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI voltageText;

    private bool isScene31;
    private bool isScene3;

    public bool hasCheckedTo;
    public bool hasCheckedFrom;
    private bool _toNotifiedThisScene;
    private bool _fromNotifiedThisScene;

    void Start()
    {
        isScene31 = SceneManager.GetActiveScene().name == "Scene 3-1";
        isScene3 = SceneManager.GetActiveScene().name == "Scene 3";

#if UNITY_EDITOR
        Debug.Log($"MultimeterScreenUpdater: Detected Scene 3-1: {isScene31}");
        Debug.Log($"MultimeterScreenUpdater: Detected Scene 3: {isScene3}");
#endif

        // References
        if (redProbe == null) redProbe = GameObject.Find("RedWirePlug")?.GetComponent<MultimeterProbe>();
        if (blackProbe == null) blackProbe = GameObject.Find("BlackWirePlug")?.GetComponent<MultimeterProbe>();
        if (voltageText == null) voltageText = GameObject.Find("Screen")?.GetComponent<TextMeshProUGUI>();
        if (breakerSwitch == null) breakerSwitch = GameObject.Find("Breaker Switch")?.GetComponent<BreakerSwitchFlipper>();

        // Find the ArmatureSocketObserver if we are in Scene 3-1 and it's not assigned
        if (isScene31 && armatureSocketObserver == null)
        {
            GameObject socketObj = GameObject.Find("Armature Socket");
            if (socketObj != null)
            {
                armatureSocketObserver = socketObj.GetComponent<ArmatureSocketObserver>();
            }
        }

        hasCheckedFrom        = false;
        hasCheckedTo          = false;
        _toNotifiedThisScene   = false;
        _fromNotifiedThisScene = false;
    }

    void Update()
    {
        // SCENE 3-1 EXCLUSIVE LOGIC
        if (isScene31)
        {
            // Rely completely on the ArmatureSocketObserver's exposed string state
            if (armatureSocketObserver == null || armatureSocketObserver.armatureState == "Empty Socket")
            {
                voltageText.text = "---V";
                return; // Exit early: No armature, no voltage reading
            }
        }

        // STANDARD VOLTAGE LOGIC (Runs for Scene 3 OR if Scene 3-1 has an armature installed)
        bool isBreakerOn = breakerSwitch != null && breakerSwitch.isFlipped;
        bool blockToPower = isScene31 && !isBreakerOn;

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
            if (isCorrectTo) hasCheckedTo = true;
            if (isCorrectFrom) hasCheckedFrom = true;

            voltageText.text = "230V";

            // In Scene 3 only: notify the state machine the first time 230V is confirmed.
            // Scene 3-1 reuses this multimeter for the bulb swap, so we skip that scene.
            if (isScene3 && !_voltageNotifiedThisScene)
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
