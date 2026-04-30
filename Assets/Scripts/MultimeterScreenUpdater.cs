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
    [SerializeField] private ArmatureSocketObserver armatureSocketObserver;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI voltageText;

    private bool isScene31;
    private bool isScene3;

    public bool hasCheckedTo;
    public bool hasCheckedFrom;
    private bool _voltageNotifiedThisScene;
    private bool _toNotifiedThisScene;
    private bool _fromNotifiedThisScene;

    void Start()
    {
        isScene31 = SceneManager.GetActiveScene().name == "Scene 3-1";
        isScene3 = SceneManager.GetActiveScene().name == "Scene 3";

        _toNotifiedThisScene   = false;
        _fromNotifiedThisScene = false;
        _voltageNotifiedThisScene = false;

        if (redProbe == null) redProbe = GameObject.Find("RedWirePlug")?.GetComponent<MultimeterProbe>();
        if (blackProbe == null) blackProbe = GameObject.Find("BlackWirePlug")?.GetComponent<MultimeterProbe>();
        if (voltageText == null) voltageText = GameObject.Find("Screen")?.GetComponent<TextMeshProUGUI>();
        if (breakerSwitch == null) breakerSwitch = GameObject.Find("Breaker Switch")?.GetComponent<BreakerSwitchFlipper>();

        if (isScene31 && armatureSocketObserver == null)
        {
            GameObject socketObj = GameObject.Find("Armature Socket");
            if (socketObj != null) armatureSocketObserver = socketObj.GetComponent<ArmatureSocketObserver>();
        }
    }

    void Update()
    {
        
        if (isScene31)
        {
            if (armatureSocketObserver == null || armatureSocketObserver.currentSocketedObject == null)
            {
                voltageText.text = "---V";
                return;
            }

           
            bool probesAreOnSocketedArmature =
                redProbe.currentOwner == armatureSocketObserver.currentSocketedObject &&
                blackProbe.currentOwner == armatureSocketObserver.currentSocketedObject;

            if (!probesAreOnSocketedArmature)
            {
                voltageText.text = "---V";
                return; // Probes are touching a different armature instance
            }
        }

       
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
            if (isCorrectTo)
            {
                hasCheckedTo = true;
                if (isScene3 && !_toNotifiedThisScene)
                {
                    _toNotifiedThisScene = true;
                    GameStateManager.Instance?.NotifyVoltageCheckedTo();
                }
            }

            if (isCorrectFrom)
            {
                hasCheckedFrom = true;
                if (isScene3 && !_fromNotifiedThisScene)
                {
                    _fromNotifiedThisScene = true;
                    GameStateManager.Instance?.NotifyVoltageCheckedFrom();
                }
            }

            voltageText.text = "230V";

            // _voltageNotifiedThisScene kept for compatibility — individual
            // To/From checks above now handle state machine notification.
            if (isScene3 && !_voltageNotifiedThisScene)
                _voltageNotifiedThisScene = true;
        }
        else
        {
            voltageText.text = "---V";
        }
    }
}
