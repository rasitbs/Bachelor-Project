using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour
{
    // 1. Declare buttons at the class level so all methods can see them
    private Button _case1;
    private Button _tutorial;

    void Awake()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        _case1 = root.Q<Button>("Case");
        _tutorial = root.Q<Button>("Tutorial");

        if (_case1 != null) _case1.clicked += OnCaseClicked;
        if (_tutorial != null) _tutorial.clicked += OnTutorialClicked;
    }

    private void OnCaseClicked() => LoadScene("Scene 1");
    private void OnTutorialClicked() => LoadScene("Tutorial");

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    void OnDisable()
    {
        if (_case1 != null) _case1.clicked -= OnCaseClicked;
        if (_tutorial != null) _tutorial.clicked -= OnTutorialClicked;
    }
}