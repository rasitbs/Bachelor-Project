using UnityEngine;

public class Skrue : MonoBehaviour
{
    private bool skrudd = true;
    private Vector3 skrudd_pos = new Vector3(0.458324879f, 0.0172838494f, -0.657422066f);
    private Vector3 uskrudd_pos = new Vector3(0.458324879f, -0.024f, -0.657422066f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (skrudd)
        {             
            transform.localPosition = skrudd_pos;
        }
        else
        {
            transform.localPosition = uskrudd_pos;
        }
    }

    public void ToggleSkrue()
    {
        skrudd = !skrudd;
        if (skrudd)
        {             
            transform.localPosition = skrudd_pos;
        }
        else
        {
            transform.localPosition = uskrudd_pos;
        }
    }



}
