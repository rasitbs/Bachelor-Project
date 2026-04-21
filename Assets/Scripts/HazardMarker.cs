using UnityEngine;

public class HazardMarker : MonoBehaviour
{
    [Header("Hazard Info")]
    public string hazardDescription = "Faremoment";
    public bool isCorrectHazard = true;

    [Header("Materials")]
    public Material striptedMaterial;
    public Material correctMaterial;
    public Material incorrectMaterial;

    [Header("References")]
    public HazardManager hazardManager;

    private bool _activated = false;
    private Renderer _renderer;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    public void OnSelected()
    {
        if (_activated) return;
        _activated = true;

        if (isCorrectHazard)
        {
            _renderer.material = correctMaterial;
            hazardManager?.OnCorrectHazardFound(this);
            Debug.Log($"[Hazard] Correct: {hazardDescription}");
        }
        else
        {
            _renderer.material = incorrectMaterial;
            hazardManager?.OnIncorrectHazardFound(this);
            Debug.Log($"[Hazard] Incorrect: {hazardDescription}");
        }
    }

    public void ResetMarker()
    {
        _activated = false;
        if (_renderer != null && striptedMaterial != null)
            _renderer.material = striptedMaterial;
    }
}