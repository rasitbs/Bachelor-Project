using UnityEngine;

public class Yell : MonoBehaviour
{
    private BreakerSwitchFlipper breakerSwitch = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (breakerSwitch == null)
        {
            breakerSwitch = GameObject.Find("Breaker Switch").GetComponent<BreakerSwitchFlipper>();
        }
    }

    public void Shout()
    {
        breakerSwitch.FlipBreakerSwitch();
    }
}
