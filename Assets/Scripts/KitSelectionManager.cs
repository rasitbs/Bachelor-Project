using UnityEngine;


public class KitSelectionManager : MonoBehaviour
{
    public static KitSelectionManager Instance { get; private set; }

    public KitLoadout SelectedKit { get; private set; }
    public bool HasSelectedKit => SelectedKit != null;

    void Awake()
    {
        // Singleton - only one instance across all scenes
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Called by KitMenuController when player confirms a kit.
    /// </summary>
    public void SelectKit(KitLoadout loadout)
    {
        SelectedKit = loadout;
        Debug.Log($"[KitSelectionManager] Kit selected: {loadout.kitName}");
    }

    /// <summary>
    /// Clears the selected kit (e.g. when restarting the game).
    /// </summary>
    public void ClearKit()
    {
        SelectedKit = null;
    }
}