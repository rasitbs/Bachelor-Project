using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class MultimeterScreenUpdater : MonoBehaviour
{
    private MultimeterProbe redprobe;
    private MultimeterProbe blackprobe;
    private BreakerSwitchFlipper breakerswitch;
    private TMPro.TextMeshProUGUI voltageText;
    
    void Start()
    {
        redprobe = GameObject.Find("RedWirePlug").GetComponent<MultimeterProbe>();
        blackprobe = GameObject.Find("BlackWirePlug").GetComponent<MultimeterProbe>();
        voltageText = GameObject.Find("MultimeterScreen").GetComponent<TMPro.TextMeshProUGUI>();
        breakerswitch = GameObject.Find("Breaker Switch").GetComponent<BreakerSwitchFlipper>();
    }

    void Update()
    {
        if((redprobe.isRedTo && blackprobe.isBlackTo) || (breakerswitch.isFlipped && redprobe.isRedFrom && blackprobe.isBlackFrom))
        {
            voltageText.text = "230V";
        }
        else
        {
            voltageText.text = "---V";
        }
    }
}