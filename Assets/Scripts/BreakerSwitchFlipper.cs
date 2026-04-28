using UnityEngine;
using System.Collections;

public class BreakerSwitchFlipper : MonoBehaviour
{
    [SerializeField] private GameObject breakerSwitch;
    public bool isFlipped = true;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip flipSound;

    [SerializeField] private ConsequenceFlags consequenceFlags;
    [SerializeField] private ShockGiver shockgiver;
    [SerializeField] private SetGreenActivateNext setGreenActivateNext;

    void Start()
    {
        if (breakerSwitch == null)
            breakerSwitch = this.gameObject;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        
        if (shockgiver == null)
            shockgiver = GetComponent<ShockGiver>();

        if (consequenceFlags == null)
            consequenceFlags = GetComponent<ConsequenceFlags>();

        if (setGreenActivateNext == null)
            setGreenActivateNext = FindObjectOfType<SetGreenActivateNext>();

        ApplyTransform();
    }

    private bool isProcessing = false;
    public void FlipBreakerSwitch()
    {
        if (isProcessing)
            return;

        isProcessing = true;

        if (!consequenceFlags.isWearingGloves)
        { 
            shockgiver.GiveShock();
            StartCoroutine(ResetInteraction());
            return;
        }

        isFlipped = !isFlipped;

        if(setGreenActivateNext.currentTaskName == "Oppgave 4")
        {             
            setGreenActivateNext.SetGreen();
        }

        ApplyTransform();

        if (audioSource != null && flipSound != null)
        {
            audioSource.PlayOneShot(flipSound);
        }

        if(EventService.Instance != null)
        {
            EventService.Instance.PublishActionInteract(breakerSwitch.name.ToString(), "Flip breaker switch", "Finger", true);
        }

        StartCoroutine(ResetInteraction());
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

    private System.Collections.IEnumerator ResetInteraction()
    {
        yield return new WaitForSeconds(0.5f); // Wait half a second before allowing another flip
        isProcessing = false;
    }
}