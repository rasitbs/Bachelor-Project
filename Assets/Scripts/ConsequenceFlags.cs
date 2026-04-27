using UnityEngine;

public class ConsequenceFlags : MonoBehaviour

{
    public bool isWearingGloves;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isWearingGloves = false;
    }

    public void SetGloves()
    {
        isWearingGloves = !isWearingGloves;
    }

}
