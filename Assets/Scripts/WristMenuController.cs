using UnityEngine;

public class WristMenuController : MonoBehaviour
{
    public GameObject wristMenuCanvas;
    public Transform palmTransform;
    public float showAngleThreshold = 45f;

    [Header("Rutiner Canvas")]
    public GameObject routineCanvas;

    [Header("Hazard Canvas")]
    public GameObject hazardCanvas;

    void Start()
    {
        if (wristMenuCanvas != null)
            wristMenuCanvas.SetActive(false);
        if (routineCanvas != null)
            routineCanvas.SetActive(false);
        if (hazardCanvas != null)
            hazardCanvas.SetActive(false);
    }

    void Update()
    {
        if (palmTransform == null || wristMenuCanvas == null) return;

        float angle = Vector3.Angle(palmTransform.up, Vector3.up);
        wristMenuCanvas.SetActive(angle > (180f - showAngleThreshold));
    }

    public void ToggleRoutineCanvas()
    {
        if (routineCanvas == null) return;
        if (!routineCanvas.activeSelf)
        {
            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0;
            forward.Normalize();
            routineCanvas.transform.position = Camera.main.transform.position + forward * 0.5f;
            routineCanvas.transform.rotation = Quaternion.LookRotation(forward);
        }
        routineCanvas.SetActive(!routineCanvas.activeSelf);
    }

    public void ToggleHazardCanvas()
    {
        if (hazardCanvas == null) return;
        if (!hazardCanvas.activeSelf)
        {
            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0;
            forward.Normalize();
            hazardCanvas.transform.position = Camera.main.transform.position + forward * 0.5f;
            hazardCanvas.transform.rotation = Quaternion.LookRotation(forward);
        }
        hazardCanvas.SetActive(!hazardCanvas.activeSelf);
    }
}