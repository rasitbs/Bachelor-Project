using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class HookItem : MonoBehaviour
{
    [Header("Belt Settings")]
    public Transform beltSlotAnchor;        // Where hook returns if not attached
    public float beltSnapDistance = 0.3f;

    [Header("State")]
    private bool _isGrabbed = false;
    private bool _isAttached = false;
    private bool _isOnBelt = true;

    private XRGrabInteractable _grab;
    private Rigidbody _rb;

    void Awake()
    {
        _grab = GetComponent<XRGrabInteractable>();
        _rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        _grab.selectEntered.AddListener(OnGrabbed);
        _grab.selectExited.AddListener(OnReleased);
    }

    void OnDisable()
    {
        _grab.selectEntered.RemoveListener(OnGrabbed);
        _grab.selectExited.RemoveListener(OnReleased);
    }

    void Start()
    {
        SetKinematic(true);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (_isAttached) return; // Can't grab if already attached
        _isGrabbed = true;
        _isOnBelt = false;
        SetKinematic(false);
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        _isGrabbed = false;

        if (_isAttached) return;

        // Snap back to belt if not attached
        SetKinematic(true);
        if (beltSlotAnchor != null)
        {
            transform.position = beltSlotAnchor.position;
            transform.rotation = beltSlotAnchor.rotation;
            _isOnBelt = true;
        }
    }

    /// <summary>
    /// Called by LiftAttachPoint when hook snaps into place.
    /// </summary>
    public void AttachToPoint(Transform attachPoint)
    {
        _isAttached = true;
        _isGrabbed = false;
        SetKinematic(true);

        transform.position = attachPoint.position;
        transform.rotation = attachPoint.rotation;
        transform.SetParent(attachPoint);

        // Disable grab so player can't pick it up again
        _grab.enabled = false;

        Debug.Log("[HookItem] Hook attached to lift!");
    }

    private void SetKinematic(bool kinematic)
    {
        if (_rb == null) return;
        _rb.isKinematic = kinematic;
        _rb.useGravity = !kinematic;
    }

    public bool IsGrabbed => _isGrabbed;
    public bool IsAttached => _isAttached;
}