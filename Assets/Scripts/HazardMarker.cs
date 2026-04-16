using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
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
    private XRSimpleInteractable _interactable;
    private Renderer _renderer;

    void Awake()
    {
        _interactable = GetComponent<XRSimpleInteractable>();
        _renderer = GetComponent<Renderer>();
    }

    void OnEnable()
    {
        _interactable.selectEntered.AddListener(OnSelected);
    }

    void OnDisable()
    {
        _interactable.selectEntered.RemoveListener(OnSelected);
    }

    void OnSelected(SelectEnterEventArgs args)
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