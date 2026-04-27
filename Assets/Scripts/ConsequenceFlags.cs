using System.Net;
using UnityEngine;

public class ConsequenceFlags : MonoBehaviour

{
    [SerializeField]
    public SetGreenActivateNext progress;

    public bool isWearingGloves;
    private bool isSet;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isWearingGloves = false;
        isSet = false;

    }

    public void SetGloves()
    {
        if (!isSet)
        {
            isSet = true;
            progress.SetGreen();
        }

        isWearingGloves = !isWearingGloves;
    }

}
