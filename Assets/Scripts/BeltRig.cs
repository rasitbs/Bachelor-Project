using UnityEngine;
using UnityEngine.SceneManagement;

public class BeltRig : MonoBehaviour
{
    [System.Serializable]
    public class BeltSlot
    {
        public string itemName;
        public Transform slotAnchor;
        [HideInInspector] public GameObject spawnedObject;
    }

    [Header("Belt Slots")]
    public BeltSlot[] slots;

    // Scener hvor kit IKKE skal auto-loades
    private readonly string[] _scenesWithoutKit = { "Main", "Tutorial", "Final" };

    void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        // Sjekk om vi er i en scene uten kit-system
        foreach (string scene in _scenesWithoutKit)
        {
            if (currentScene == scene)
            {
                Debug.Log($"[BeltRig] Scene '{currentScene}' uses no kit system. Skipping auto-load.");
                return;
            }
        }

        // Auto-load valgt kit i Scene 1, 2, 3 og 3-1
        if (KitSelectionManager.Instance != null && KitSelectionManager.Instance.HasSelectedKit)
        {
            LoadKit(KitSelectionManager.Instance.SelectedKit);
            Debug.Log("[BeltRig] Auto-loaded kit from KitSelectionManager.");
        }
        else
        {
            Debug.Log("[BeltRig] No kit selected yet.");
        }
    }

    public void ClearBelt()
    {
        foreach (var slot in slots)
        {
            if (slot.spawnedObject != null)
            {
                Destroy(slot.spawnedObject);
                slot.spawnedObject = null;
            }
        }
    }

    public void LoadKit(KitLoadout loadout)
    {
        ClearBelt();

        foreach (var entry in loadout.items)
        {
            if (!entry.includedInKit) continue;
            if (entry.prefab == null) continue;

            BeltSlot slot = GetSlot(entry.itemName);
            if (slot == null)
            {
                Debug.LogWarning($"[BeltRig] No slot found for: {entry.itemName}");
                continue;
            }

            slot.spawnedObject = Instantiate(entry.prefab, slot.slotAnchor);
            slot.spawnedObject.transform.localPosition = Vector3.zero;
            slot.spawnedObject.transform.localRotation = Quaternion.identity;

            // Auto-assign slotAnchor til BeltSnapBack hvis den finnes
            BeltSnapBack snapBack = slot.spawnedObject.GetComponent<BeltSnapBack>();
            if (snapBack != null)
                snapBack.slotAnchor = slot.slotAnchor;

            Debug.Log($"[BeltRig] Spawned {entry.itemName} on belt.");
        }
    }

    private BeltSlot GetSlot(string itemName)
    {
        foreach (var slot in slots)
            if (slot.itemName == itemName) return slot;
        return null;
    }
}