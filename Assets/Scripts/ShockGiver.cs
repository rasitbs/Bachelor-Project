using UnityEngine;

public class ShockGiver : MonoBehaviour
{

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip AudioClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GiveShock()
    {
        // Camera Shake
        Camera cam = Camera.main;
        for (int i = 0; i < 10; i++)    
            cam.transform.localPosition += Random.insideUnitSphere * 0.1f;

        // Audio cue
        audioSource.PlayOneShot(AudioClip);

        // Sparks


        // Point Deduction
        EventService.Instance.PublishHseAlert("Shock event", "Player tried to flip breaker without gloves", 5);
#if UNITY_EDITOR
        Debug.Log("Shock given!");
#endif

    }
}
