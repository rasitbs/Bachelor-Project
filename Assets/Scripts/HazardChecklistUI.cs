using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class HazardChecklistUI : MonoBehaviour
{
    [Header("References")]
    public Transform checklistParent;       // Vertical Layout Group
    public GameObject checklistRowPrefab;   // Row prefab with checkbox + text
    public TextMeshProUGUI progressText;    // "2 / 6"
    public TextMeshProUGUI allFoundText;    // "Alle faremomenter funnet!"

    [Header("Checkbox Sprites")]
    public Sprite emptyCheckbox;            // □ empty box
    public Sprite filledCheckbox;           // ✓ filled checkmark

    [Header("Colors")]
    public Color foundColor = new Color(0.2f, 0.85f, 0.2f);    // Green
    public Color notFoundColor = new Color(0.8f, 0.8f, 0.8f);  // Grey

    // Internal
    private int _totalCount = 0;
    private int _foundCount = 0;
    private List<GameObject> _rows = new List<GameObject>();

    void Start()
    {
        if (allFoundText != null)
            allFoundText.gameObject.SetActive(false);

        UpdateProgressText();
    }

    
    public void SetTotalCount(int total)
    {
        _totalCount = total;

        // Clear existing rows
        foreach (var row in _rows)
            Destroy(row);
        _rows.Clear();

        // Spawn one empty row per hazard
        for (int i = 0; i < total; i++)
        {
            GameObject row = Instantiate(checklistRowPrefab, checklistParent);
            _rows.Add(row);

            // Set empty checkbox
            Image checkbox = row.GetComponentInChildren<Image>();
            if (checkbox != null && emptyCheckbox != null)
                checkbox.sprite = emptyCheckbox;

            // Set empty description
            TextMeshProUGUI label = row.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
            {
                label.text = "???";
                label.color = notFoundColor;
            }
        }

        UpdateProgressText();
    }

    
    public void AddCorrectEntry(string description)
    {
        if (_foundCount >= _rows.Count) return;

        GameObject row = _rows[_foundCount];

        // Fill checkbox
        Image checkbox = row.GetComponentInChildren<Image>();
        if (checkbox != null && filledCheckbox != null)
            checkbox.sprite = filledCheckbox;

        // Set description
        TextMeshProUGUI label = row.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
        {
            label.text = description;
            label.color = foundColor;
        }

        _foundCount++;
        UpdateProgressText();
    }

    
    public void ShowAllFoundMessage()
    {
        if (allFoundText != null)
        {
            allFoundText.gameObject.SetActive(true);
            allFoundText.text = "Alle faremomenter funnet!";
            allFoundText.color = foundColor;
        }
    }

    private void UpdateProgressText()
    {
        if (progressText != null)
            progressText.text = $"{_foundCount} / {_totalCount}";
    }
}