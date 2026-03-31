using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChanger : MonoBehaviour
{
    public Image fadeImage; // Drag your ScreenFader here
    public float fadeDuration = 1.0f;

    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        float timer = 0;
        Color color = fadeImage.color;

        // 1. Fade to Black
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Clamp01(timer / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        // 2. Load the Scene
        SceneManager.LoadScene(sceneName);
    }
}
