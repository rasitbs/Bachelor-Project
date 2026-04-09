using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Changes only the Trigger and Grip (Bumper) button colors
/// when the player presses them on the controller.
///
/// === SETUP ===
/// 1. Attach to Left Controller and Right Controller
/// 2. Assign triggerRenderer  -> Trigger GameObject renderer
/// 3. Assign gripRenderer     -> Bumper GameObject renderer
/// 4. Assign gripAction and triggerAction from Input Action References
/// </summary>
public class ControllerColorIndicator : MonoBehaviour
{
    [Header("Renderers")]
    public Renderer triggerRenderer;        // Trigger mesh renderer
    public Renderer gripRenderer;           // Bumper (grip) mesh renderer

    [Header("Input Actions")]
    public InputActionReference gripAction;
    public InputActionReference triggerAction;

    [Header("Colors")]
    public Color normalColor = new Color(0.2f, 0.2f, 0.2f);    // Mork gra
    public Color pressedColor = new Color(0f, 0.8f, 1f);        // Cyan/bla

    private Material _triggerMaterial;
    private Material _gripMaterial;

    void Start()
    {
        if (triggerRenderer != null)
        {
            _triggerMaterial = triggerRenderer.material;
            _triggerMaterial.color = normalColor;
        }

        if (gripRenderer != null)
        {
            _gripMaterial = gripRenderer.material;
            _gripMaterial.color = normalColor;
        }
    }

    void OnEnable()
    {
        if (gripAction != null)
        {
            gripAction.action.performed += OnGripPressed;
            gripAction.action.canceled += OnGripReleased;
            gripAction.action.Enable();
        }

        if (triggerAction != null)
        {
            triggerAction.action.performed += OnTriggerPressed;
            triggerAction.action.canceled += OnTriggerReleased;
            triggerAction.action.Enable();
        }
    }

    void OnDisable()
    {
        if (gripAction != null)
        {
            gripAction.action.performed -= OnGripPressed;
            gripAction.action.canceled -= OnGripReleased;
        }

        if (triggerAction != null)
        {
            triggerAction.action.performed -= OnTriggerPressed;
            triggerAction.action.canceled -= OnTriggerReleased;
        }
    }

    private void OnGripPressed(InputAction.CallbackContext ctx)
    {
        if (_gripMaterial != null)
            _gripMaterial.color = pressedColor;
    }

    private void OnGripReleased(InputAction.CallbackContext ctx)
    {
        if (_gripMaterial != null)
            _gripMaterial.color = normalColor;
    }

    private void OnTriggerPressed(InputAction.CallbackContext ctx)
    {
        if (_triggerMaterial != null)
            _triggerMaterial.color = pressedColor;
    }

    private void OnTriggerReleased(InputAction.CallbackContext ctx)
    {
        if (_triggerMaterial != null)
            _triggerMaterial.color = normalColor;
    }
}