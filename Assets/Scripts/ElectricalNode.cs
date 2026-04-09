using UnityEngine;

public class ElectricalNode : MonoBehaviour
{
    public enum NodeType { LineIn, LineOut, Neutral }

    [Tooltip("LineIn = Always 230V. Neutral = Always 0V. LineOut = 230V only if breaker is ON.")]
    public NodeType nodeType;

    [Tooltip("Only required if this node is a LineOut")]
    public BreakerSwitchFlipper breakerSwitch;

    // This determines the actual electricity at this specific socket
    public float GetVoltage()
    {
        switch (nodeType)
        {
            case NodeType.LineIn:
                return 230f; // Hot From (Always live)
            case NodeType.Neutral:
                return 0f;   // Black From / Black To (Always 0V reference)
            case NodeType.LineOut:
                return (breakerSwitch != null && breakerSwitch.IsOn) ? 230f : 0f; // Hot To (Dependent on switch)
            default:
                return 0f;
        }
    }
}
