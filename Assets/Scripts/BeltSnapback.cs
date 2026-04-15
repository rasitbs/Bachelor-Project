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

    [Header("Slot Visual Feedback")]
    public GameObject slotIndicatorPrefab;  // Optional: assign a glowing sphere prefab
    public Color idleColor = new Color(1f, 1f, 0f, 0.4f);      // Gul når grabbed
    public Color nearColor = new Color(0f, 1f, 0.3f, 0.6f);    // Grønn når nær nok

    // Set automatically by BeltRig when spawned
    public Transform slotAnchor;

    private XRGrabInteractable _grab;
    private Rigidbody _rb;
    private bool _isGrabbed = false;
    private bool _isOnBelt = true;
    private bool _isSnapping = false;

    // Slot indicator
    private GameObject _slotIndicator;
    private Renderer _indicatorRenderer;

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
        CreateSlotIndicator();
        HideIndicator();
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        _isGrabbed = true;
        _isOnBelt = false;
        _isSnapping = false;
        SetKinematic(false);
        ShowIndicator();
    }

    private void OnReleased(SelectExitEventArgs args)
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
        // Update indicator color based on distance
        if (_isGrabbed && slotAnchor != null && _indicatorRenderer != null)
        {
            float dist = Vector3.Distance(transform.position, slotAnchor.position);
            if (dist <= snapDistance * 3f)
                _indicatorRenderer.material.color = nearColor;
            else
                _indicatorRenderer.material.color = idleColor;
        }

        // Snap animation
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
            HideIndicator();
        }
    }

    private void CreateSlotIndicator()
    {
        if (slotAnchor == null) return;

        if (slotIndicatorPrefab != null)
        {
            _slotIndicator = Instantiate(slotIndicatorPrefab, slotAnchor.position, Quaternion.identity);
            _slotIndicator.transform.SetParent(slotAnchor);
            _slotIndicator.transform.localPosition = Vector3.zero;
        }
        else
        {
            // Lag en enkel sphere som placeholder
            _slotIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _slotIndicator.transform.SetParent(slotAnchor);
            _slotIndicator.transform.localPosition = Vector3.zero;
            _slotIndicator.transform.localScale = Vector3.one * 0.15f;

            // Fjern collider så den ikke blokkerer
            Destroy(_slotIndicator.GetComponent<Collider>());

            // Lag transparent material
            _indicatorRenderer = _slotIndicator.GetComponent<Renderer>();
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.SetFloat("_Surface", 1); // Transparent
            mat.SetFloat("_Blend", 0);
            mat.renderQueue = 3000;
            mat.color = idleColor;
            _indicatorRenderer.material = mat;
        }

        if (_indicatorRenderer == null)
            _indicatorRenderer = _slotIndicator.GetComponent<Renderer>();
    }

    private void ShowIndicator()
    {
        if (_slotIndicator != null)
            _slotIndicator.SetActive(true);
    }

    private void HideIndicator()
    {
        if (_slotIndicator != null)
            _slotIndicator.SetActive(false);
    }

    private void SetKinematic(bool kinematic)
    {
        if (_rb == null) return;
        _rb.isKinematic = kinematic;
        _rb.useGravity = !kinematic;
    }

    public bool IsOnBelt => _isOnBelt;
}