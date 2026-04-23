using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoutineMenuController : MonoBehaviour
{
    [Header("Pages")]
    public GameObject listPage;
    public GameObject detailPage;

    [Header("Detail UI")]
    public TMP_Text detailTitle;
    public Transform imageContainer;
    public GameObject imagePagePrefab;
    public ScrollRect detailScrollView;

    [Header("Routines")]
    public Toggle[] routineToggles;
    public RoutineData[] allRoutines;

    void Start()
    {
        for (int i = 0; i < routineToggles.Length; i++)
        {
            int index = i;
            routineToggles[i].onValueChanged.AddListener((isOn) =>
            {
                if (isOn) OpenRoutine(allRoutines[index]);
            });
        }

        ShowList();
    }

    public void OpenRoutine(RoutineData routine)
    {
        detailTitle.text = routine.title;

        // Fjern gamle bilder
        foreach (Transform child in imageContainer)
            Destroy(child.gameObject);

        // Beregn bredde basert på ScrollView
        float availableWidth = detailScrollView.GetComponent<RectTransform>().rect.width - 40f;

        foreach (Sprite page in routine.pages)
        {
            GameObject img = Instantiate(imagePagePrefab, imageContainer);
            RectTransform rt = img.GetComponent<RectTransform>();
            Image imgComp = img.GetComponent<Image>();
            imgComp.sprite = page;
            imgComp.preserveAspect = true;

            // Beregn høyde basert på aspektforhold
            float aspect = (float)page.texture.height / page.texture.width;
            rt.sizeDelta = new Vector2(availableWidth, availableWidth * aspect);
        }

        listPage.SetActive(false);
        detailPage.SetActive(true);
    }

    public void ShowList()
    {
        foreach (var toggle in routineToggles)
            toggle.SetIsOnWithoutNotify(false);

        detailPage.SetActive(false);
        listPage.SetActive(true);
    }
}