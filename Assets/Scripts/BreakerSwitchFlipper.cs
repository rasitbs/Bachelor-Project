using UnityEngine;

public class BreakerSwitchFlipper : MonoBehaviour
{
    [SerializeField] private GameObject breakerSwitch;
    public bool isFlipped = true;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip flipSound;

    void Start()
    {
        if (breakerSwitch == null)
            breakerSwitch = this.gameObject;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        ApplyTransform();
    }

    public void FlipBreakerSwitch()
    {
        isFlipped = !isFlipped;
        ApplyTransform();

        if (audioSource != null && flipSound != null)
        {
            audioSource.PlayOneShot(flipSound);
        }

        if(EventService.Instance != null)
        {
            EventService.Instance.PublishActionInteract(breakerSwitch.name.ToString(), "Flip breaker switch", "Finger", true);
        }
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