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

    [Header("Confirm Buttons")]
    public GameObject[] confirmButtons;

    [Header("References")]
    public InteractionController interactionController;
    public BeltRig beltRig;

    [Header("Feedback UI")]
    public GameObject feedbackPanel;
    public TextMeshProUGUI feedbackText;
    public float feedbackDuration = 3f;

    [Header("Detail Panel � PPE Rows")]
    public GameObject ppeRowPrefab;
    public Transform[] kitItemLists;

    [Header("Audio")]
    public AudioSource closingDoorSound;

    [Header("Next Canvas")]
    public GameObject nextCanvas;

    private int _openIndex = -1;

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

            Debug.Log($"[KitMenu] Opening kit {index}, confirmButtons.Length = {confirmButtons.Length}");

            if (confirmButtons.Length > index && confirmButtons[index] != null)
            {
                confirmButtons[index].SetActive(true);
                Debug.Log($"[KitMenu] ConfirmBtn {index} set active!");
            }
            else
            {
                Debug.Log($"[KitMenu] ConfirmBtn {index} is NULL or out of range!");
            }
        }
    }

    public void ConfirmKit(int index)
    {
        KitEntry selected = kits[index];
        KitLoadout loadout = selected.loadout;

        if (beltRig != null && loadout != null)
            beltRig.LoadKit(loadout);
        KitSelectionManager.Instance?.SelectKit(loadout);

        if (loadout != null && !loadout.isCorrectKit)
        {
            ShowFeedback("Feil kit valgt!", false);
            ScorePopup.Instance?.ShowScore(-5);
            EventService.Instance?.PublishKitSelection($"kit_{index}", false, 0, 5);
            Debug.Log("[KitMenu] Wrong kit selected.");
        }
        else
        {
            ShowFeedback("Riktig kit! Bra jobbet!", true);
            ScorePopup.Instance?.ShowScore(10);
            EventService.Instance?.PublishKitSelection($"kit_{index}", true, 10, 0);
            Debug.Log("[KitMenu] Correct kit selected!");
        }

        if (nextCanvas != null)
            nextCanvas.SetActive(true);

        CloseAll();
        _openIndex = -1;
        interactionController.CloseMenu();
        if (closingDoorSound != null)
            closingDoorSound.Play();
    }

    /// <summary>
    /// Called by the button on nextCanvas after the player has confirmed their kit.
    /// Transitions the simulation to Scene 2 (SJA + risk assessment) via the state machine.
    /// Wire this to the nextCanvas proceed-button's OnClick event in the Inspector.
    /// </summary>
    public void ProceedToScene2()
    {
        GameStateManager.Instance?.ChangeState(GameState.Scene2_SJA);
    }

    void CloseAll()
    {
        foreach (var kit in kits)
            kit.detailPanel.SetActive(false);
        foreach (var btn in confirmButtons)
            if (btn != null) btn.SetActive(false);
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