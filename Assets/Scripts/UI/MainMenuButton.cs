using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour
{
    void Awake()
    {
        VisualElement root = GetComponent<UIDocument>
	
	Button case1 = root.Q<Button>("Case");
	Button tutorial = root.Q<Button>("Tutorial");

	case1.clicked += () => LoadScene("Scene 1");
	tutorial.clicked += () => LoadScene("Tutorial");
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    void onDisable()
    {
    	case1.clicked -= () => LoadScene("Scene 1");
        tutorial.clicked -= () => LoadScene("Tutorial");
    }
}

