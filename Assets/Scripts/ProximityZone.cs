using UnityEngine;

public class ProximityZone : MonoBehaviour
{
    [Header("Settings")]
    public float activationRadius = 3f;
    public Transform playerTransform;
    public CircleInteractable circleInteractable;

    private bool _playerInRange = false;

    void Start()
    {
        if (playerTransform == null)
        {
            var xrOrigin = FindAnyObjectByType<Unity.XR.CoreUtils.XROrigin>();
            if (xrOrigin != null)
                playerTransform = xrOrigin.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        bool inRange = dist <= activationRadius;

        if (inRange != _playerInRange)
        {
            _playerInRange = inRange;
            circleInteractable.SetPlayerInRange(inRange);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, activationRadius);
    }
}