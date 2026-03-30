using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement; // Required for loading scenes

public class MainMenuController : MonoBehaviour
{
    [Tooltip("Type the exact name of the scene you want to load here.")]
    public string sceneToLoad = "Scene 1";

    private UIDocument _uiDocument;
    private Button _startButton;

    void OnEnable()
    {
        // 1. Get the UIDocument component attached to this GameObject
        _uiDocument = GetComponent<UIDocument>();

        if (_uiDocument == null)
        {
            Debug.LogError("MainMenuController needs a UIDocument component to work!");
            return;
        }

        // 2. Query the root visual element to find our button by the name we gave it
        var root = _uiDocument.rootVisualElement;
        _startButton = root.Q<Button>("Case"); // Make sure this matches UI Builder exactly!

        if (_startButton != null)
        {
            // 3. Subscribe to the click event
            _startButton.clicked += LoadNextScene;
        }
        else
        {
            Debug.LogError("Could not find a button named 'StartButton' in the UI Document.");
        }
    }

    void OnDisable()
    {
        // Always unsubscribe from events when the object is disabled to prevent memory leaks
        if (_startButton != null)
        {
            _startButton.clicked -= LoadNextScene;
        }
    }

    private void LoadNextScene()
    {
        Debug.Log("Button clicked! Loading scene: " + sceneToLoad);
        SceneManager.LoadScene(sceneToLoad);
    }
}