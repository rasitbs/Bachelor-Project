using UnityEngine;

public class WristMenuController : MonoBehaviour
{
    public GameObject wristMenuCanvas;
    public Transform palmTransform;
    public float showAngleThreshold = 45f;

    void Start()
    {
        if (wristMenuCanvas != null)
            wristMenuCanvas.SetActive(false);
    }

    void Update()
    {
        if (palmTransform == null || wristMenuCanvas == null) return;
        float angle = Vector3.Angle(palmTransform.up, Vector3.up);
        wristMenuCanvas.SetActive(angle > (180f - showAngleThreshold));
    }
}