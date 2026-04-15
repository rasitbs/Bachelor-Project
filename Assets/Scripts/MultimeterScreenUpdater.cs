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
    
    private bool breakerSwitchIsFlipped = true;

    void Start()
    {
        if (redProbe == null)
            redProbe = GameObject.Find("RedWirePlug").GetComponent<MultimeterProbe>();
        
        if (blackProbe == null)
            blackProbe = GameObject.Find("BlackWirePlug").GetComponent<MultimeterProbe>();
            
        if (voltageText == null)    
            voltageText = GameObject.Find("MultimeterScreen").GetComponent<TextMeshProUGUI>();
            
        if (breakerSwitch == null)
        {
            breakerSwitch = GameObject.Find("Breaker Switch")?.GetComponent<BreakerSwitchFlipper>();
        }
        
        breakerSwitchIsFlipped = breakerSwitch != null ? breakerSwitch.isFlipped : true;
    }

    void Update()
    {
        bool isCorrectTo = (redProbe.currentSocketName == "Hot Point Red To" &&
                            blackProbe.currentSocketName == "Hot Point Black To");

        bool isCorrectFrom = (breakerSwitchIsFlipped &&
                              redProbe.currentSocketName == "Hot Point Red From" &&
                              blackProbe.currentSocketName == "Hot Point Black From");

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
