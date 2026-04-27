using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Unity.VisualScripting;

public class SetGreenActivateNext : MonoBehaviour
{
    // Canvas that contains the task texts
    [SerializeField]
    public Canvas taskCanvas;

    [SerializeField]
    public GameObject liftcube;

    private List<GameObject> tasks;
    public bool isDone = false;
    GameObject curr;

    private void Awake()
    {
        if (taskCanvas != null)
        {
            // Get all childen of the canvas.background_panel with the prefix name "Oppgave" and add them to the tasks list
            tasks = new List<GameObject>();
            foreach (Transform child in taskCanvas.transform)
            {
                if (child.name.StartsWith("Oppgave"))
                {
                    tasks.Add(child.gameObject);
#if UNITY_EDITOR
                    Debug.Log("Added task: " + child.name);
#endif
                }
            }
        }
    }

    private void Start()
    {
        foreach (GameObject task in tasks)
        {
            task.GetComponent<TextMeshProUGUI>().color = Color.white;
        }
#if UNITY_EDITOR
        Debug.Log("Initialized tasks with white color.");
#endif
        curr = GetCurrentTask();
#if UNITY_EDITOR
        Debug.Log("Current task: " + (curr != null ? curr.name : "None"));
#endif
        liftcube.SetActive(false);
    }

    public void SetGreen()
    {
        if (isDone)
        { 
            liftcube.SetActive(true);
            return;
        }

        if (curr != null)
        {
            curr.GetComponent<TextMeshProUGUI>().color = Color.green;
#if UNITY_EDITOR
            Debug.Log("Set task " + curr.name + " to green.");
#endif
        }
        curr = GetCurrentTask();
#if UNITY_EDITOR
        Debug.Log("Current task: " + (curr != null ? curr.name : "None"));
#endif
    }

    private GameObject GetCurrentTask()
    { 
        foreach (GameObject task in tasks)
        {
            if (task.GetComponent<TextMeshProUGUI>().color != Color.green)
            {
                return task;
            }
        }
        isDone = true;
        return null;
    }

    
}