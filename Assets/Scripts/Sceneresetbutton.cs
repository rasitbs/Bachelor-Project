using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneResetButton : MonoBehaviour
{
    [Header("Settings")]
    public float resetDelay = 0f;

    public void ResetScene()
    {
        if (resetDelay > 0f)
            Invoke(nameof(DoReset), resetDelay);
        else
            DoReset();
    }

    private void DoReset()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log($"[SceneResetButton] Reloading scene: {currentScene}");
        SceneManager.LoadScene(currentScene);
    }
}