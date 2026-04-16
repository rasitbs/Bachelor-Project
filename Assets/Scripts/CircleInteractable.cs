using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CircleInteractable : MonoBehaviour
{
    [Header("References")]
    public InteractionController interactionController;
    public Renderer buttonRenderer;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable _interactable;
    private bool _playerInRange = false;

    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    [Header("Visuals")]
    public Color normalEmission = new Color(0f, 1f, 1f);
    public Color hoverEmission = new Color(1f, 1f, 0.2f);

    [Header("Audio")]
    public AudioSource doorSound;

    void Awake()
    {
        _interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
    }

    void OnEnable()
    {
        _interactable.selectEntered.AddListener(OnSelected);
        _interactable.hoverEntered.AddListener(OnHoverEnter);
        _interactable.hoverExited.AddListener(OnHoverExit);
    }

    void OnDisable()
    {
        _interactable.selectEntered.RemoveListener(OnSelected);
        _interactable.hoverEntered.RemoveListener(OnHoverEnter);
        _interactable.hoverExited.RemoveListener(OnHoverExit);
    }

    void OnSelected(SelectEnterEventArgs args)
    {
        if (_playerInRange)
            interactionController.OnButtonActivated();
        if (doorSound != null)
            doorSound.Play();
    }

    void OnHoverEnter(HoverEnterEventArgs args)
    {
        if (buttonRenderer != null)
            buttonRenderer.material.SetColor(EmissionColor, hoverEmission);
    }

    void OnHoverExit(HoverExitEventArgs args)
    {
        if (buttonRenderer != null)
            buttonRenderer.material.SetColor(EmissionColor, normalEmission);
    }

    public void SetPlayerInRange(bool inRange)
    {
        _playerInRange = inRange;
        if (buttonRenderer != null)
            buttonRenderer.enabled = inRange;
    }
}