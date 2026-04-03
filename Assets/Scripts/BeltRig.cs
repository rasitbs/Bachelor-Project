using UnityEngine;

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