using UnityEngine;

public class LightStateObserver : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Light targetLight;
    [SerializeField] private BreakerSwitchFlipper breakerSwitch;
    [SerializeField] private ArmatureSocketObserver armatureSocketObserver;

    private void OnEnable()
    {
        if (breakerSwitch != null)
            breakerSwitch.OnStateChanged += UpdateLightState;

        if (armatureSocketObserver != null)
            armatureSocketObserver.OnStateChanged += UpdateLightState;

        
        UpdateLightState();
    }

    private void OnDisable()
    {
        if (breakerSwitch != null)
            breakerSwitch.OnStateChanged -= UpdateLightState;

        if (armatureSocketObserver != null)
            armatureSocketObserver.OnStateChanged -= UpdateLightState;
    }

    public void UpdateLightState()
    {
        if (targetLight == null || breakerSwitch == null || armatureSocketObserver == null)
            return;

        bool shouldLightBeOn = breakerSwitch.isFlipped && armatureSocketObserver.isLightArmatureNew;

        if (targetLight.enabled != shouldLightBeOn)
        {
            targetLight.enabled = shouldLightBeOn;
        }
    }
}