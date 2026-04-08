using UnityEngine;
using UnityEngine.Events;

public class HazardManager : MonoBehaviour
{
    [Header("References")]
    public HazardChecklistUI checklistUI;

    [Header("Events")]
    public UnityEvent onAllHazardsFound;        // Fires when all correct hazards are found
    public UnityEvent<HazardMarker> onCorrectHazardFound;
    public UnityEvent<HazardMarker> onIncorrectHazardFound;

    // Counts
    private int _totalCorrect = 0;
    private int _foundCorrect = 0;

    void Start()
    {
        // Count all correct hazards in scene
        HazardMarker[] allMarkers = FindObjectsOfType<HazardMarker>();
        foreach (var marker in allMarkers)
        {
            if (marker.isCorrectHazard)
                _totalCorrect++;
        }

        checklistUI?.SetTotalCount(_totalCorrect);
        Debug.Log($"[HazardManager] Total correct hazards: {_totalCorrect}");
    }

    public void OnCorrectHazardFound(HazardMarker marker)
    {
        _foundCorrect++;
        onCorrectHazardFound?.Invoke(marker);
        checklistUI?.AddCorrectEntry(marker.hazardDescription);

        if (_foundCorrect >= _totalCorrect)
        {
            Debug.Log("[HazardManager] All hazards found!");
            onAllHazardsFound?.Invoke();
            checklistUI?.ShowAllFoundMessage();
        }
    }

    public void OnIncorrectHazardFound(HazardMarker marker)
    {
        onIncorrectHazardFound?.Invoke(marker);
        Debug.Log($"[HazardManager] Wrong hazard selected: {marker.hazardDescription}");
    }

    public int FoundCount => _foundCorrect;
    public int TotalCount => _totalCorrect;
}