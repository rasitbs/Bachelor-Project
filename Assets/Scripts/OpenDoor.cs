using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [SerializeField]
    private GameObject door;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private Vector3 closedPosition = new Vector3(0.5f, -0.05f, 0f);
    private Quaternion closedRotation = Quaternion.Euler(0, 0, -90);

    private Vector3 openPosition = new Vector3(0f, 0f, 0f);
    private Quaternion openRotation = Quaternion.Euler(0, 0, 0);
    void Start()
    {
        if (door == null)
        {
            door = this.gameObject;
#if UNITY_EDITOR
            Debug.LogWarning("Door GameObject not assigned. Defaulting to the GameObject this script is attached to.");
#endif
        }
    }

    public void OpenCloseDoor()
    {
        if (door.transform.localPosition == closedPosition && door.transform.localRotation == closedRotation)
        {
            door.transform.localPosition = openPosition;
            door.transform.localRotation = openRotation;
        }
        else
        {
            door.transform.localPosition = closedPosition;
            door.transform.localRotation = closedRotation;
        }
    }
}
