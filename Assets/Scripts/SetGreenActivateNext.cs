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

    [SerializeField]
    private MultimeterScreenUpdater multimeterScreenUpdater;

    private List<GameObject> tasks;
    public bool isDone = false;
    GameObject curr;
    public string currentTaskName;

    private bool _lastCheckedToState = false;
    private bool _lastCheckedFromState = false;

    private void Awake()
    {
        tasks = new List<GameObject>();  // Initialize first!
        
        if (taskCanvas == null)
        {
            Debug.LogError("[SetGreenActivateNext] taskCanvas not assigned!", this);
            return;
        }

        foreach (Transform child in taskCanvas.GetComponentsInChildren<Transform>())
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

    private void Start()
    {
        if (multimeterScreenUpdater == null)
        {
            multimeterScreenUpdater = GetComponent<MultimeterScreenUpdater>();
            if (multimeterScreenUpdater == null)
            {
                Debug.LogError("[SetGreenActivateNext] MultimeterScreenUpdater not found!", this);
            }
        }

        foreach (GameObject task in tasks)
        {
            TextMeshProUGUI textComponent = task.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.color = Color.white;
            }
            else
            {
                Debug.LogWarning($"[SetGreenActivateNext] GameObject '{task.name}' has no TextMeshProUGUI component!", this);
            }
        }
#if UNITY_EDITOR
        Debug.Log("Initialized tasks with white color.");
#endif
        curr = GetCurrentTask();
        currentTaskName = curr != null ? curr.name : "None";
#if UNITY_EDITOR
        Debug.Log("Current task: " + currentTaskName);
#endif
        liftcube.SetActive(false);
    }

    private void Update()
    {
        if (multimeterScreenUpdater == null)
            return;

        // Check if hasCheckedTo changed from false to true
        if (multimeterScreenUpdater.hasCheckedTo && !_lastCheckedToState)
        {
            if (currentTaskName == "Oppgave 2")
            {
                SetGreen();
            }
            _lastCheckedToState = true;
        }
        else if (!multimeterScreenUpdater.hasCheckedTo)
        {
            _lastCheckedToState = false;
        }

        // Check if hasCheckedFrom changed from false to true
        if (multimeterScreenUpdater.hasCheckedFrom && !_lastCheckedFromState)
        {
            if (currentTaskName == "Oppgave 3")
            {
                SetGreen();
            }
            _lastCheckedFromState = true;
        }
        else if (!multimeterScreenUpdater.hasCheckedFrom)
        {
            _lastCheckedFromState = false;
        }
    }

    public void SetGreen()
    {

        if (curr != null)
        {
            curr.GetComponent<TextMeshProUGUI>().color = Color.green;
            EventService.Instance?.PublishHazardMarked(curr.name, true, 5, 0);
#if UNITY_EDITOR
            Debug.Log("Set task " + curr.name + " to green.");
#endif
        }
        curr = GetCurrentTask();
        currentTaskName = curr != null ? curr.name : "None";
#if UNITY_EDITOR
        Debug.Log("Current task: " + currentTaskName);
#endif
        if (currentTaskName == "Oppgave 5")
        {
            liftcube.SetActive(true);
            return;
        }
    }

    private GameObject GetCurrentTask()
    {
        foreach (GameObject task in tasks)
        {
            TextMeshProUGUI textComponent = task.GetComponent<TextMeshProUGUI>();
            if (textComponent != null && textComponent.color != Color.green)
            {
                return task;
            }
        }
        isDone = true;
        return null;
    }
}