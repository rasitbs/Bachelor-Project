using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KitMenuController : MonoBehaviour
{
    [System.Serializable]
    public class KitEntry
    {
        public GameObject headerButton;
        public GameObject detailPanel;
        public KitLoadout loadout;
    }

    [Header("Kit Entries")]
    public KitEntry[] kits;

    [Header("References")]
    public InteractionController interactionController;
    public BeltRig beltRig;

    [Header("Feedback UI")]
    public GameObject feedbackPanel;
    public TextMeshProUGUI feedbackText;
    public float feedbackDuration = 3f;

    [Header("Detail Panel – PPE Rows")]
    public GameObject ppeRowPrefab;
    public Transform[] kitItemLists;

    private int _openIndex = -1;

    // TODO: Koble inn gruppens poengsystem her når det er klart.
    // KitLoadout.isCorrectKit forteller om valgt kit er riktig.

    void Start()
    {
        CloseAll();
        HideFeedback();
        BuildAllItemLists();
    }

    public void ToggleKit(int index)
    {
        if (_openIndex == index)
        {
            CloseAll();
            _openIndex = -1;
        }
        else
        {
            CloseAll();
            _openIndex = index;
            kits[index].detailPanel.SetActive(true);
        }
    }

    public void ConfirmKit(int index)
    {
        KitEntry selected = kits[index];
        KitLoadout loadout = selected.loadout;

        // 1. Last utstyr på beltet
        if (beltRig != null && loadout != null)
            beltRig.LoadKit(loadout);

        // 2. Feedback
        if (loadout != null && !loadout.isCorrectKit)
        {
            ShowFeedback("Feil kit valgt!", false);
            Debug.Log("[KitMenu] Wrong kit selected.");
        }
        else
        {
            ShowFeedback("Riktig kit! Bra jobbet!", true);
            Debug.Log("[KitMenu] Correct kit selected!");
        }

        // 3. Lukk menyen
        CloseAll();
        _openIndex = -1;
        interactionController.CloseMenu();
    }

    void CloseAll()
    {
        foreach (var kit in kits)
            kit.detailPanel.SetActive(false);
    }

    void HideFeedback()
    {
        if (feedbackPanel != null)
            feedbackPanel.SetActive(false);
    }

    void ShowFeedback(string message, bool correct)
    {
        if (feedbackPanel == null || feedbackText == null) return;
        feedbackText.text = message;
        feedbackText.color = correct ? new Color(0.2f, 0.85f, 0.2f) : new Color(0.9f, 0.2f, 0.2f);
        feedbackPanel.SetActive(true);
        CancelInvoke(nameof(HideFeedback));
        Invoke(nameof(HideFeedback), feedbackDuration);
    }

    void BuildAllItemLists()
    {
        if (ppeRowPrefab == null) return;

        for (int i = 0; i < kits.Length; i++)
        {
            if (kits[i].loadout == null) continue;
            if (i >= kitItemLists.Length || kitItemLists[i] == null) continue;

            Transform listParent = kitItemLists[i];

            foreach (Transform child in listParent)
                Destroy(child.gameObject);

            foreach (var entry in kits[i].loadout.items)
            {
                GameObject row = Instantiate(ppeRowPrefab, listParent);

                TextMeshProUGUI label = row.GetComponentInChildren<TextMeshProUGUI>();
                if (label != null) label.text = entry.itemName;

                Image[] images = row.GetComponentsInChildren<Image>();
                if (images.Length >= 2)
                {
                    images[0].gameObject.SetActive(entry.includedInKit);
                    images[1].gameObject.SetActive(!entry.includedInKit);
                }
            }
        }
    }
}