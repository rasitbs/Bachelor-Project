using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneUIController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string sceneName = "Scene 1"; 
    [SerializeField] private string scenarioName = "HMS Grunnleggende";
    
    // We make this static so the FinalSceneUIController can read it later
    // to calculate the total duration across multiple scenes.
    public static float GlobalSessionStartTime { get; private set; }

    /// <summary>
    /// This method will be called by the Button's OnClick event
    /// </summary>
    public void StartSessionAndGoToScene()
    {
        // Check if it is NOT null before trying to use it
        if (EventService.Instance != null)
        {
            GlobalSessionStartTime = Time.time;
            Debug.Log($"[SessionUI] Starting Session '{scenarioName}'. Timestamp : {GlobalSessionStartTime:F1}s");
            
            // Send the MQTT event
            EventService.Instance.PublishSessionStarted(scenarioName);
        }
        else
        {
            Debug.LogWarning("EventService Instance not found. Is it in the scene?");
        }
    
        // Load the first simulation case
        SceneManager.LoadScene(sceneName);
    }
}

