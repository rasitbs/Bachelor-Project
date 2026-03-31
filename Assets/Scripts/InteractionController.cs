using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject menuCanvas;

    private bool _menuOpen = false;

    public void OnButtonActivated()
    {
        _menuOpen = !_menuOpen;
        menuCanvas.SetActive(_menuOpen);
    }

    public void CloseMenu()
    {
        _menuOpen = false;
        menuCanvas.SetActive(false);
    }
}