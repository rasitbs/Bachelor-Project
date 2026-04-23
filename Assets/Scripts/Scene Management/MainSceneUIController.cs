using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneUIController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string sceneName = "Scene 1"; 
    [SerializeField] private string scenarioName = "HMS Grunnleggende";

    public void StartSessionAndGoToScene()
    {
        if (EventService.Instance != null)
        {
            
            Debug.Log($"[SessionUI] Starting Session '{scenarioName}'");
            
            EventService.Instance.PublishSessionStarted(sceneName);
        }
        else
        {
            Debug.LogWarning("[SessionUI] EventService Instance not found. Is it in the scene?");
        }
    
        SceneManager.LoadScene(sceneName);
    }
}

