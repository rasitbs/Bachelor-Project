using UnityEngine;
using Oculus.Interaction;

[RequireComponent(typeof(Grabbable))]
[RequireComponent(typeof(Rigidbody))]
public class HookItem : MonoBehaviour
{
    [Header("Belt Settings")]
    public Transform beltSlotAnchor;
    public float beltSnapDistance = 0.3f;

    [Header("State")]
    private bool _isGrabbed = false;
    private bool _isAttached = false;
    private bool _isOnBelt = true;

    private Grabbable _grabbable;
    private Rigidbody _rb;

    void Awake()
    {
        _grabbable = GetComponent<Grabbable>();
        _rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        _grabbable.WhenPointerEventRaised += OnPointerEvent;
    }

    void OnDisable()
    {
        _grabbable.WhenPointerEventRaised -= OnPointerEvent;
    }

    void Start()
    {
        SetKinematic(true);
    }

    private void OnPointerEvent(PointerEvent evt)
    {
        switch (evt.Type)
        {
            case PointerEventType.Select:
                OnGrabbed();
                break;
            case PointerEventType.Unselect:
                OnReleased();
                break;
        }
    }

    private void OnGrabbed()
    {
        if (_isAttached) return;
        _isGrabbed = true;
        _isOnBelt = false;
        SetKinematic(false);
    }

    private void OnReleased()
    {
        _isGrabbed = false;
        if (_isAttached) return;

        SetKinematic(true);
        if (beltSlotAnchor != null)
        {
            transform.position = beltSlotAnchor.position;
            transform.rotation = beltSlotAnchor.rotation;
            _isOnBelt = true;
        }
    }

    public void AttachToPoint(Transform attachPoint)
    {
        _isAttached = true;
        _isGrabbed = false;
        SetKinematic(true);
        transform.position = attachPoint.position;
        transform.rotation = attachPoint.rotation;
        transform.SetParent(attachPoint);
        _grabbable.enabled = false;
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