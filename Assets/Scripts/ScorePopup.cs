using UnityEngine;
using TMPro;
using System.Collections;

public class ScorePopup : MonoBehaviour
{
    public static ScorePopup Instance { get; private set; }

    [Header("References")]
    public TextMeshProUGUI popupText;
    public Transform cameraTransform;

    [Header("Settings")]
    public float displayDuration = 1.5f;
    public float floatSpeed = 0.3f;
    public Color positiveColor = new Color(1f, 0.65f, 0f);
    public Color negativeColor = new Color(1f, 0.2f, 0.2f);

    [Header("Position")]
    public float distanceFromCamera = 2f;
    public float heightOffset = 0.2f;
    public float rightOffset = 0.4f;

    private Coroutine _currentCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if (popupText != null)
            popupText.gameObject.SetActive(false);
    }

    public void ShowScore(int points)
    {
        if (_currentCoroutine != null)
            StopCoroutine(_currentCoroutine);
        _currentCoroutine = StartCoroutine(AnimatePopup(points));
    }

    private IEnumerator AnimatePopup(int points)
    {
        bool isPositive = points > 0;
        popupText.text = isPositive ? $"+{points}" : $"{points}";
        popupText.color = isPositive ? positiveColor : negativeColor;
        popupText.fontStyle = FontStyles.Bold;

        Vector3 spawnPos = cameraTransform.position
            + cameraTransform.forward * distanceFromCamera
            + cameraTransform.right * rightOffset
            + cameraTransform.up * heightOffset;

        transform.position = spawnPos;
        transform.rotation = Quaternion.LookRotation(
            transform.position - cameraTransform.position
        );

        popupText.gameObject.SetActive(true);

        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Color startColor = popupText.color;

        while (elapsed < displayDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / displayDuration;

            transform.position = startPos + Vector3.up * (floatSpeed * t);

            float alpha = t < 0.5f ? 1f : 1f - ((t - 0.5f) / 0.5f);
            popupText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        popupText.gameObject.SetActive(false);
    }
}