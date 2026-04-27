using UnityEngine;

public class WristInventory : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject helmetPrefab;
    public GameObject glovesPrefab;
    public GameObject screwdriverPrefab;
    public GameObject multimeterPrefab;

    private GameObject _currentItem;

    public void SpawnHelmet() => SpawnItem(helmetPrefab);
    public void SpawnGloves()
    {
        SpawnItem(glovesPrefab);
        GameStateManager.Instance?.NotifyGlovesEquipped();
    }
    public void SpawnScrewdriver() => SpawnItem(screwdriverPrefab);
    public void SpawnMultimeter() => SpawnItem(multimeterPrefab);

    private void SpawnItem(GameObject prefab)
    {
        if (prefab == null) return;
        if (_currentItem != null)
            Destroy(_currentItem);
        _currentItem = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
    }
}