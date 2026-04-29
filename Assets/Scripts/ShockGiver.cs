using UnityEngine;
using System.Collections; 

public class ShockGiver : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shockClip;
    [SerializeField] private ParticleSystem sparks;
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeMagnitude = 0.1f;

    public void GiveShock()
    {
        // Audio cue
        if (audioSource != null && shockClip != null)
            audioSource.PlayOneShot(shockClip);

        // Visual Sparks
        if (sparks != null)
            sparks.Play();

        // Camera Shake?

        // Point Deduction
        if (EventService.Instance != null)
        {
            EventService.Instance.PublishHseAlert("Shock event", "Player tried to flip breaker without gloves", 5);
        }
            

#if UNITY_EDITOR
        Debug.Log("Shock given!");
#endif
    }

    private IEnumerator ShakeCamera()
    {
        Transform camTransform = Camera.main.transform;
        Vector3 originalPos = camTransform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            camTransform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            elapsed += Time.deltaTime;

            yield return null; // Wait until the next frame
        }

        camTransform.localPosition = originalPos;
    }
}