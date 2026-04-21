using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

[RequireComponent(typeof(Grabbable))]
[RequireComponent(typeof(Rigidbody))]
public class BeltSnapBack : MonoBehaviour
{
    [Header("Snap Settings")]
    public float snapDistance = 0.4f;
    public float snapSpeed = 15f;

    [Header("Slot Visual Feedback")]
    public Color idleColor = new Color(1f, 1f, 0f, 0.4f);
    public Color nearColor = new Color(0f, 1f, 0.3f, 0.6f);

    public Transform slotAnchor;

    private Grabbable _grabbable;
    private Rigidbody _rb;
    private bool _isGrabbed = false;
    private bool _isOnBelt = true;
    private bool _isSnapping = false;

    private GameObject _slotIndicator;
    private Renderer _indicatorRenderer;

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
        CreateSlotIndicator();
        HideIndicator();
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
        _isGrabbed = true;
        _isOnBelt = false;
        _isSnapping = false;
        SetKinematic(false);
        ShowIndicator();
    }

    private void OnReleased()
    {
        _isGrabbed = false;
        HideIndicator();

        if (slotAnchor == null) return;

        float dist = Vector3.Distance(transform.position, slotAnchor.position);
        if (dist <= snapDistance * 3f)
        {
            _isSnapping = true;
            SetKinematic(true);
        }
        else
        {
            transform.SetParent(null);
            SetKinematic(false);
            _isOnBelt = false;
        }
    }

    void Update()
    {
        if (_isGrabbed && slotAnchor != null && _indicatorRenderer != null)
        {
            float dist = Vector3.Distance(transform.position, slotAnchor.position);
            _indicatorRenderer.material.color = dist <= snapDistance * 3f ? nearColor : idleColor;
        }

        if (!_isSnapping || slotAnchor == null) return;

        transform.position = Vector3.Lerp(transform.position, slotAnchor.position, Time.deltaTime * snapSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, slotAnchor.rotation, Time.deltaTime * snapSpeed);

        if (Vector3.Distance(transform.position, slotAnchor.position) < 0.01f)
        {
            transform.position = slotAnchor.position;
            transform.rotation = slotAnchor.rotation;
            _isSnapping = false;
            _isOnBelt = true;
            SetKinematic(true);
            HideIndicator();
        }
    }

    private void CreateSlotIndicator()
    {
        if (slotAnchor == null) return;

        _slotIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _slotIndicator.transform.SetParent(slotAnchor);
        _slotIndicator.transform.localPosition = Vector3.zero;
        _slotIndicator.transform.localScale = Vector3.one * 0.15f;
        Destroy(_slotIndicator.GetComponent<Collider>());

        _indicatorRenderer = _slotIndicator.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.SetFloat("_Surface", 1);
        mat.renderQueue = 3000;
        mat.color = idleColor;
        _indicatorRenderer.material = mat;
    }

    private void ShowIndicator() { if (_slotIndicator != null) _slotIndicator.SetActive(true); }
    private void HideIndicator() { if (_slotIndicator != null) _slotIndicator.SetActive(false); }
    private void SetKinematic(bool kinematic) { if (_rb != null) { _rb.isKinematic = kinematic; _rb.useGravity = !kinematic; } }

    public bool IsOnBelt => _isOnBelt;
}