using UnityEngine;
using TMPro;

public class MultimeterScreenUpdater : MonoBehaviour
{
    [SerializeField] private MultimeterProbe redProbe;
    [SerializeField] private MultimeterProbe blackProbe;

    public TextMeshProUGUI voltageText;
    public bool isMeasuringVoltage = true;

    void Update()
    {
        if (!isMeasuringVoltage) return;

        // Both probes must be plugged in to complete a circuit
        if (redProbe.IsConnected && blackProbe.IsConnected)
        {
            // Calculate voltage difference: Red Probe Voltage - Black Probe Voltage
            float measuredVoltage = redProbe.GetPotential() - blackProbe.GetPotential();

            // Format to show a whole number (e.g., 230V or -230V)
            voltageText.text = Mathf.RoundToInt(measuredVoltage).ToString() + "V";
        }
        else
        {
            // If one or both are unplugged, show 0V or a disconnected state
            voltageText.text = "0V";
        }
    }
}