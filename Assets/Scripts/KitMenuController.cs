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
    }

    [Header("Kit Entries")]
    public KitEntry[] kits;

    [Header("References")]
    public InteractionController interactionController;

    private int _openIndex = -1;

    void Start()
    {
        CloseAll();
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

    void CloseAll()
    {
        foreach (var kit in kits)
            kit.detailPanel.SetActive(false);
    }

    public void ConfirmKit(int index)
    {
        Debug.Log("Valgte: KIT " + (index + 1));
        CloseAll();
        _openIndex = -1;
        // Close the whole menu
        interactionController.CloseMenu();
    }
}