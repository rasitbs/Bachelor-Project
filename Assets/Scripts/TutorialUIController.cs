using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialUIController : MonoBehaviour
{
    [Header("Buttons")]
    public Button button1;
    public Button button2;
    public Button button3;

    [Header("Slider")]
    public Slider tutorialSlider;
    public TextMeshProUGUI sliderValueText;

    [Header("Feedback")]
    public TextMeshProUGUI feedbackText;

    [Header("Button Colors")]
    public Color normalColor = new Color(0.2f, 0.5f, 1f);
    public Color pressedColor = new Color(0.2f, 0.85f, 0.2f);

    void Start()
    {
        // Setup button listeners
        button1?.onClick.AddListener(() => OnButtonPressed(button1, "Knapp 1 trykket!"));
        button2?.onClick.AddListener(() => OnButtonPressed(button2, "Knapp 2 trykket!"));
        button3?.onClick.AddListener(() => OnButtonPressed(button3, "Knapp 3 trykket!"));

        // Setup slider listener
        tutorialSlider?.onValueChanged.AddListener(OnSliderChanged);

        // Init feedback
        if (feedbackText != null)
            feedbackText.text = "";
    }

    void OnButtonPressed(Button btn, string message)
    {
        // Change button color to green
        ColorBlock colors = btn.colors;
        colors.normalColor = pressedColor;
        colors.highlightedColor = pressedColor;
        btn.colors = colors;

        ShowFeedback(message);
        Debug.Log($"[Tutorial] {message}");
    }

    void OnSliderChanged(float value)
    {
        if (sliderValueText != null)
            sliderValueText.text = Mathf.RoundToInt(value).ToString();

        ShowFeedback($"Slider: {Mathf.RoundToInt(value)}");
    }

    void ShowFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            CancelInvoke(nameof(ClearFeedback));
            Invoke(nameof(ClearFeedback), 2f);
        }
    }

    void ClearFeedback()
    {
        if (feedbackText != null)
            feedbackText.text = "";
    }
}