using UnityEngine;

public class FollowPlayerBody : MonoBehaviour
{
    public Transform headTransform;
    public float verticalOffset = -0.5f;

    void LateUpdate()
    {
        // Følg hodet sin X og Z posisjon, men ignorer rotasjon
        Vector3 newPos = headTransform.position;
        newPos.y = headTransform.position.y + verticalOffset;
        transform.position = newPos;

        // Bare roter rundt Y-aksen (venstre/høyre), ignorer tilt
        float headYaw = headTransform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, headYaw, 0);
    }
}