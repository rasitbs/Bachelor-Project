using UnityEngine;

[CreateAssetMenu(fileName = "KitLoadout", menuName = "PPE/Kit Loadout")]
public class KitLoadout : ScriptableObject
{
    [System.Serializable]
    public class PPEEntry
    {
        public string itemName;
        public GameObject prefab;
        public bool includedInKit;
    }

    [Header("Kit Info")]
    public string kitName = "KIT 1";
    public bool isCorrectKit = false;

    [Header("Equipment")]
    public PPEEntry[] items;
}