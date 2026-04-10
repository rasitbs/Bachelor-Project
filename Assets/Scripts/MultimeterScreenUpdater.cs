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
        if (redProbe == null)
            redProbe = GameObject.Find("RedWirePlug").GetComponent<MultimeterProbe>();
        
        if (blackProbe == null)
            blackProbe = GameObject.Find("BlackWirePlug").GetComponent<MultimeterProbe>();
            
        if (voltageText == null)    
            voltageText = GameObject.Find("MultimeterScreen").GetComponent<TextMeshProUGUI>();
            
        if (breakerSwitch == null)
            breakerSwitch = GameObject.Find("Breaker Switch").GetComponent<BreakerSwitchFlipper>();
    }

    void Update()
    {
        // 1. Check if both probes are in the "To" side (Standard Connection)
        bool isCorrectTo = (redProbe.currentSocketTag == "Hot Point Red To" && 
                            blackProbe.currentSocketTag == "Hot Point Black To");

        // 2. Check if both probes are in the "From" side AND the breaker is on
        bool isCorrectFrom = (breakerSwitch.isFlipped && 
                              redProbe.currentSocketTag == "Hot Point Red From" && 
                              blackProbe.currentSocketTag == "Hot Point Black From");

        // 3. Update the display
        if (isCorrectTo || isCorrectFrom)
        {
            voltageText.text = "230V";
        }
        else
        {
            voltageText.text = "---V";
        }
    }
}
