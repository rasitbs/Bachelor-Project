using UnityEngine;

public class DoorConstraint : MonoBehaviour
{
    [SerializeField] private float minAngle = -90f;
    [SerializeField] private float maxAngle = 0f;

    void LateUpdate() 
    {
        Vector3 localRot = transform.localEulerAngles;

        
        float angle = localRot.z;
        if (angle > 180) angle -= 360;

        
        float clampedAngle = Mathf.Clamp(angle, minAngle, maxAngle);

        transform.localRotation = Quaternion.Euler(localRot.x, localRot.y, clampedAngle);
    }
}
