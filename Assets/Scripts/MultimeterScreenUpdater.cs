using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MultimeterScreenUpdater : MonoBehaviour
{
    [Header("Hardware References")]
    [SerializeField] private MultimeterProbe redProbe;
    [SerializeField] private MultimeterProbe blackProbe;
    [SerializeField] private BreakerSwitchFlipper breakerSwitch;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI voltageText;

    private bool isScene3_1;

    void Start()
    {
        isScene3_1 = SceneManager.GetActiveScene().name == "3-1";
        // Null checks for setup
        if (redProbe == null)
            redProbe = GameObject.Find("RedWirePlug").GetComponent<MultimeterProbe>();

        if (blackProbe == null)
            blackProbe = GameObject.Find("BlackWirePlug").GetComponent<MultimeterProbe>();

        if (voltageText == null)
            voltageText = GameObject.Find("MultimeterScreen").GetComponent<TextMeshProUGUI>();

        if (breakerSwitch == null)
            breakerSwitch = GameObject.Find("Breaker Switch")?.GetComponent<BreakerSwitchFlipper>();
    }

    void Update()
    {
        bool isBreakerOn = breakerSwitch != null && breakerSwitch.isFlipped;

        bool blockToPower = isScene3_1 && isBreakerOn;

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