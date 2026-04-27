using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RiskOptionButton : MonoBehaviour
{
    private TextMeshProUGUI _label;
    private Toggle _toggle;
    public bool IsCorrect { get; private set; }
    public bool IsSelected => _toggle != null && _toggle.isOn;

    public void Init(string text, bool correct)
    {
        _label = GetComponentInChildren<TextMeshProUGUI>();
        _toggle = GetComponentInChildren<Toggle>();

        if (_label != null) _label.text = text;
        IsCorrect = correct;
        if (_toggle != null) _toggle.isOn = false;
    }

    public void ResetToggle()
    {
        if (_toggle != null) _toggle.isOn = false;
    }
}