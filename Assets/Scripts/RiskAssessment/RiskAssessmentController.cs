using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RiskAssessmentController : MonoBehaviour
{
    [Header("Data - dra inn alle 4 ScriptableObjects")]
    [SerializeField] private List<RiskAssessmentData> categories;

    [Header("Panels")]
    [SerializeField] private GameObject introPanel;
    [SerializeField] private GameObject riskCategoryPanel;
    [SerializeField] private GameObject resultPanel;

    [Header("RiskCategoryPanel UI")]
    [SerializeField] private TextMeshProUGUI categoryTitleText;
    [SerializeField] private Transform toggleContainer;
    [SerializeField] private Button nextButton;

    [Header("ResultPanel UI")]
    [SerializeField] private TextMeshProUGUI resultTitleText;
    [SerializeField] private TextMeshProUGUI resultDetailsText;
    [SerializeField] private TextMeshProUGUI pointsFeedbackText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button closeButton;

    [Header("Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float passingThreshold = 0.75f;
    [SerializeField] private int pointsOnPass = 10;
    [SerializeField] private int penaltyOnFail = 5;

    // Runtime
    private int _currentIndex = 0;
    private int _totalCorrect = 0;
    private int _totalPossible = 0;
    private List<RiskOptionButton> _toggleButtons = new();

    private void OnEnable()
    {
        ShowIntro();
    }

    // ── Public ─────────────────────────────────────

    public void OnStartPressed()
    {
        Debug.Log("OnStartPressed called!");
        _currentIndex = 0;
        _totalCorrect = 0;
        _totalPossible = 0;
        ShowCategory(0);
    }

    public void OnNextPressed()
    {
        ScoreCurrentCategory();
        _currentIndex++;

        if (_currentIndex < categories.Count)
            ShowCategory(_currentIndex);
        else
            ShowResult();
    }

    public void OnRetryPressed()
    {
        _currentIndex = 0;
        _totalCorrect = 0;
        _totalPossible = 0;
        ShowCategory(0);
    }

    public void OnClosePressed()
    {
        gameObject.SetActive(false);
    }

    // ── Private ────────────────────────────────────

    private void ShowIntro()
    {
        introPanel.SetActive(true);
        riskCategoryPanel.SetActive(false);
        resultPanel.SetActive(false);
    }

    private void ShowCategory(int index)
    {
        introPanel.SetActive(false);
        riskCategoryPanel.SetActive(true);
        resultPanel.SetActive(false);

        var data = categories[index];
        var shuffledOptions = ShuffleList(data.options);

        categoryTitleText.text = data.categoryTitle;

        _toggleButtons.Clear();
        for (int i = 0; i < toggleContainer.childCount; i++)
        {
            var child = toggleContainer.GetChild(i);
            var btn = child.GetComponent<RiskOptionButton>();
            if (btn == null) continue;

            if (i < shuffledOptions.Count)
            {
                child.gameObject.SetActive(true);
                btn.Init(shuffledOptions[i].label, shuffledOptions[i].isCorrect);
                _toggleButtons.Add(btn);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    private void ScoreCurrentCategory()
    {
        foreach (var btn in _toggleButtons)
        {
            if (btn.IsCorrect)
            {
                _totalPossible++;
                if (btn.IsSelected) _totalCorrect++;
            }
            else
            {
                if (btn.IsSelected) _totalCorrect--;
            }
        }
        _totalCorrect = Mathf.Max(0, _totalCorrect);
    }

    private void ShowResult()
    {
        introPanel.SetActive(false);
        riskCategoryPanel.SetActive(false);
        resultPanel.SetActive(true);

        float percent = _totalPossible > 0 ? (float)_totalCorrect / _totalPossible : 0f;
        bool passed = percent >= passingThreshold;
        int scorePct = Mathf.RoundToInt(percent * 100);
        int thresholdPct = Mathf.RoundToInt(passingThreshold * 100);

        if (passed)
        {
            resultTitleText.text = "Bra jobbet!";
            resultDetailsText.text = $"Du fikk {scorePct}% riktige.\nDu kan gå videre.";
            pointsFeedbackText.text = $"+{pointsOnPass} poeng";
            pointsFeedbackText.color = new Color(1f, 0.75f, 0f);
            retryButton.gameObject.SetActive(false);
            closeButton.gameObject.SetActive(true);

            EventService.Instance?.PublishRiskAssessmentCompleted(
                passed: true,
                scorePercent: scorePct,
                points: pointsOnPass,
                penalty: 0
            );
        }
        else
        {
            resultTitleText.text = "Ikke bestått";
            resultDetailsText.text = $"Du fikk {scorePct}%.\nDu trenger minst {thresholdPct}% for å gå videre.";
            pointsFeedbackText.text = $"-{penaltyOnFail} poeng";
            pointsFeedbackText.color = Color.red;
            retryButton.gameObject.SetActive(true);
            closeButton.gameObject.SetActive(false);

            EventService.Instance?.PublishRiskAssessmentCompleted(
                passed: false,
                scorePercent: scorePct,
                points: 0,
                penalty: penaltyOnFail
            );
        }
    }

    private List<RiskOption> ShuffleList(List<RiskOption> list)
    {
        List<RiskOption> shuffled = new List<RiskOption>(list);
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }
        return shuffled;
    }
}