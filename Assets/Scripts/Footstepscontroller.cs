using UnityEngine;
using UnityEngine.InputSystem;

public class FootstepController : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip footstepClip;
    public float volume = 0.5f;

    [Header("Settings")]
    public float stepInterval = 0.5f;       // Seconds between each step
    public float moveThreshold = 0.1f;      // Minimum joystick input to trigger steps

    [Header("Input")]
    public InputActionReference moveAction;  // XRI Left Locomotion/Move

    private AudioSource _audioSource;
    private float _stepTimer = 0f;

    void Awake()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.clip = footstepClip;
        _audioSource.volume = volume;
        _audioSource.spatialBlend = 0f; // 2D sound
        _audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (moveAction == null || footstepClip == null) return;

        Vector2 input = moveAction.action.ReadValue<Vector2>();
        bool isMoving = input.magnitude > moveThreshold;

        if (isMoving)
        {
            _stepTimer -= Time.deltaTime;
            if (_stepTimer <= 0f)
            {
                _audioSource.PlayOneShot(footstepClip, volume);
                _stepTimer = stepInterval;
            }
        }
        else
        {
            _stepTimer = 0f;
        }
    }
}