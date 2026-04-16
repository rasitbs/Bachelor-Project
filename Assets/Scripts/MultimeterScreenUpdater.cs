using UnityEngine;
using TMPro;

public class MultimeterScreenUpdater : MonoBehaviour
{
    [Header("Hardware References")]
    [SerializeField] private MultimeterProbe redProbe;
    [SerializeField] private MultimeterProbe blackProbe;
    [SerializeField] private BreakerSwitchFlipper breakerSwitch;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI voltageText;

    void Start()
    {
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

        bool isCorrectTo = (redProbe.currentSocketName == "Hot Point Red To" &&
                            blackProbe.currentSocketName == "Hot Point Black To");

        bool isCorrectFrom = (isBreakerOn &&
                              redProbe.currentSocketName == "Hot Point Red From" &&
                              blackProbe.currentSocketName == "Hot Point Black From");

        bool isCorrectCross = (isBreakerOn &&
                              redProbe.currentSocketName == "Hot Point Red From" &&
                              blackProbe.currentSocketName == "Hot Point Black To") ||
                             (isBreakerOn &&
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