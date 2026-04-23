using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private float animationSpeed = 2f;

    private Vector3 closedPosition = new Vector3(0.5f, -0.05f, 0f);
    private Quaternion closedRotation = Quaternion.Euler(0, 0, -90);
    private Vector3 openPosition = new Vector3(0f, 0f, 0f);
    private Quaternion openRotation = Quaternion.Euler(0, 0, 0);

    private bool isOpen = false;
    private bool isAnimating = false;
    private float t = 0f;

    void Start()
    {
        if (door == null)
        {
            door = this.gameObject;
#if UNITY_EDITOR
            Debug.LogWarning("Door not assigned, defaulting to self.");
#endif
        }
    }

    void Update()
    {
        if (!isAnimating) return;

        t += Time.deltaTime * animationSpeed;
        t = Mathf.Clamp01(t);

        if (isOpen)
        {
            door.transform.localPosition = Vector3.Lerp(closedPosition, openPosition, t);
            door.transform.localRotation = Quaternion.Lerp(closedRotation, openRotation, t);
        }
        else
        {
            door.transform.localPosition = Vector3.Lerp(openPosition, closedPosition, t);
            door.transform.localRotation = Quaternion.Lerp(openRotation, closedRotation, t);
        }

        if (t >= 1f) isAnimating = false;
    }

    public void OpenCloseDoor()
    {
        if (isAnimating) return;
        isOpen = !isOpen;
        t = 0f;
        isAnimating = true;
    }

    public bool IsOpen => isOpen;
}