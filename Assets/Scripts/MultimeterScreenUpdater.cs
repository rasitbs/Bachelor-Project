using UnityEngine;

public class MultimeterScreenUpdater : MonoBehaviour
{
    [SerializeField]
    private GameObject multimeterScreen;
    private GameObject redplug;
    private GameObject blackplug;


    void Start()
    {
        multimeterScreen = this.gameObject;

        // Find the red and black plugs in the scene
        redplug = GameObject.Find("RedWirePlug");
        blackplug = GameObject.Find("BlackWirePlug");
        
        if (redplug == null || blackplug == null)
        {
            #if UNITY_EDITOR
                Debug.LogError("Red or Black plug not found in the scene. Please ensure they are named 'RedWirePlug' and 'BlackWirePlug'.");
            #endif  
        }

    }

}
