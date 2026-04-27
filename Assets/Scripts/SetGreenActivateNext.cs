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
    public bool isDone;
    GameObject curr;
    public string currentTaskName;

    private void Awake()
    {
        liftcube.SetActive(false);
        isDone = false;

    }

    private void Start()
    {
        if (taskCanvas != null)
        {
            tasks = new List<GameObject>();

            /*  Oppgave Canvas
             *  | - Background Panel
             *      | - Oppgave 1
             *      | - Oppgave 2
             *      ...
             */

            // Search through all descendants for "Oppgave" GameObjects
            foreach (TextMeshProUGUI child in taskCanvas.GetComponentsInChildren<TextMeshProUGUI>(true))
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

        foreach (GameObject task in tasks)
        {
            task.GetComponent<TextMeshProUGUI>().color = Color.white;
        }
#if UNITY_EDITOR
        Debug.Log("Initialized tasks with white color.");
#endif
        curr = GetCurrentTask();
        currentTaskName = curr != null ? curr.name : "None";
#if UNITY_EDITOR
        Debug.Log("Current task: " + currentTaskName);
#endif
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
        currentTaskName = curr != null ? curr.name : "None";    
#if UNITY_EDITOR
        Debug.Log("Current task: " + currentTaskName);
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