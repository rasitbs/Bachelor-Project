using UnityEngine;

public class MultimeterProbe : MonoBehaviour
{
    public bool isRedTo = false;
    public bool isRedFrom = false;
    public bool isBlackTo = false;
    public bool isBlackFrom = false;

    public void Connect(GameObject other)
    {
        if(other.CompareTag("Hot Point Black From"))
        {
            isBlackFrom = true;
        }
        else if(other.CompareTag("Hot Point Black To"))
        {
            isBlackTo = true;
        }
        else if(other.CompareTag("Hot Point Red From"))
        {
            isRedFrom = true;
        }
        else if(other.CompareTag("Hot Point Red To"))
        {
            isRedTo = true;
        }
    }

    public void Disconnect()
    {
        isRedTo = false;
        isRedFrom = false;
        isBlackTo = false;
        isBlackFrom = false;
    }
}