using UnityEngine;
using UnityEngine.Events;

public class HazardManager : MonoBehaviour
{
    [Header("References")]
    public HazardChecklistUI checklistUI;

    [Header("Audio")]
    public AudioClip correctSound;
    public AudioClip incorrectSound;
    private AudioSource _audioSource;

    [Header("Events")]
    public UnityEvent onAllHazardsFound;
    public UnityEvent<HazardMarker> onCorrectHazardFound;
    public UnityEvent<HazardMarker> onIncorrectHazardFound;

    private int _totalCorrect = 0;
    private int _foundCorrect = 0;

    void Awake()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.playOnAwake = false;
    }

    void Start()
    {
        HazardMarker[] allMarkers = FindObjectsByType<HazardMarker>(FindObjectsSortMode.None);
        foreach (var marker in allMarkers)
        {
            if (marker.isCorrectHazard)
                _totalCorrect++;
        }

        checklistUI?.SetTotalCount(_totalCorrect);
    }

    public void OnCorrectHazardFound(HazardMarker marker)
    {
        _foundCorrect++;
        onCorrectHazardFound?.Invoke(marker);
        checklistUI?.AddCorrectEntry(marker.hazardDescription);

        if (correctSound != null)
            _audioSource.PlayOneShot(correctSound);

        if (_foundCorrect >= _totalCorrect)
        {
            onAllHazardsFound?.Invoke();
            checklistUI?.ShowAllFoundMessage();
            GameStateManager.Instance?.NotifySJACompleted();
        }
    }

    public void OnIncorrectHazardFound(HazardMarker marker)
    {
        onIncorrectHazardFound?.Invoke(marker);

        if (incorrectSound != null)
            _audioSource.PlayOneShot(incorrectSound);

        Debug.Log($"[HazardManager] Wrong hazard: {marker.hazardDescription}");
    }

    public int FoundCount => _foundCorrect;
    public int TotalCount => _totalCorrect;
}