using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class BeltSnapBack : MonoBehaviour
{
    [Header("Snap Settings")]
    public float snapDistance = 0.4f;
    public float snapSpeed = 15f;

    // Set automatically by BeltRig when spawned
    [HideInInspector] public Transform slotAnchor;

    private XRGrabInteractable _grab;
    private Rigidbody _rb;
    private bool _isGrabbed = false;
    private bool _isOnBelt = true;
    private bool _isSnapping = false;

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
        _isGrabbed = true;
        _isOnBelt = false;
        _isSnapping = false;
        SetKinematic(false);
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        _isGrabbed = false;

        if (slotAnchor == null) return;

        float dist = Vector3.Distance(transform.position, slotAnchor.position);
        if (dist <= snapDistance * 3f)
        {
            _isSnapping = true;
            SetKinematic(true);
        }
        else
        {
            SetKinematic(false);
            _isOnBelt = false;
        }
    }

    void Update()
    {
        if (!_isSnapping || slotAnchor == null) return;

        transform.position = Vector3.Lerp(
            transform.position,
            slotAnchor.position,
            Time.deltaTime * snapSpeed
        );
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            slotAnchor.rotation,
            Time.deltaTime * snapSpeed
        );

        if (Vector3.Distance(transform.position, slotAnchor.position) < 0.01f)
        {
            transform.position = slotAnchor.position;
            transform.rotation = slotAnchor.rotation;
            _isSnapping = false;
            _isOnBelt = true;
            SetKinematic(true);
        }
    }

    private void SetKinematic(bool kinematic)
    {
        if (_rb == null) return;
        _rb.isKinematic = kinematic;
        _rb.useGravity = !kinematic;
    }

    public bool IsOnBelt => _isOnBelt;
}