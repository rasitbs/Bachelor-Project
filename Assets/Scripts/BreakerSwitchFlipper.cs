using UnityEngine;

public class BreakerSwitchFlipper : MonoBehaviour
{
    [SerializeField] private GameObject breakerSwitch;
    public bool isFlipped = true;

    void Start()
    {
        if (breakerSwitch == null)
            breakerSwitch = this.gameObject;

        ApplyTransform();
    }

    public void FlipBreakerSwitch()
    {
        isFlipped = !isFlipped;
        ApplyTransform();
    }

    private void ApplyTransform()
    {
        if (isFlipped)
        {
            breakerSwitch.transform.localPosition = new Vector3(0, 2.16f, 4.3f);
            breakerSwitch.transform.localRotation = Quaternion.Euler(120, 0, 0);
        }
        else
        {
            breakerSwitch.transform.localPosition = new Vector3(0, -2.61f, 3.98f);
            breakerSwitch.transform.localRotation = Quaternion.Euler(-120, 0, 0);
        }
    }
}