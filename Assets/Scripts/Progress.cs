using UnityEngine;

public class Progress : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ArmatureSocketObserver _armatureSocketObserver;
    [SerializeField] private LightCoverObserver _lightCoverObserver;
    [SerializeField] private MultimeterScreenUpdater _multimeterScreenUpdater;
    [SerializeField] private MultimeterProbe _redProbe;
    [SerializeField] private MultimeterProbe _blackProbe;


    [Header("Publics")]
    public string currentTask = null;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTask = "Oppgave 1";

        // Ensure references are assigned, if not, try to find them in the scene
       if (_armatureSocketObserver == null)
        {
            GameObject armatureSocketObj = GameObject.Find("Armature Socket");
            if (armatureSocketObj != null)
                _armatureSocketObserver = armatureSocketObj.GetComponentInChildren<ArmatureSocketObserver>();
        }
        if (_lightCoverObserver == null)
        {
            GameObject lightCoverObj = GameObject.Find("Cover Socket");
            if (lightCoverObj != null)
                _lightCoverObserver = lightCoverObj.GetComponentInChildren<LightCoverObserver>();
        }
        if (_multimeterScreenUpdater == null)
        {
            GameObject multimeterScreenObj = GameObject.Find("Screen");
            if (multimeterScreenObj != null)
                _multimeterScreenUpdater = multimeterScreenObj.GetComponentInChildren<MultimeterScreenUpdater>();
        }
        if (_redProbe == null)
        {
            GameObject redProbeObj = GameObject.Find("RedWirePlug");
            if (redProbeObj != null)
                _redProbe = redProbeObj.GetComponentInChildren<MultimeterProbe>();
        }
        if (_blackProbe == null)
        {
            GameObject blackProbeObj = GameObject.Find("BlackWirePlug");
            if (blackProbeObj != null)
                _blackProbe = blackProbeObj.GetComponentInChildren<MultimeterProbe>();
        }
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    // In the internal switch case, for each task, check conditions for fulfillment,
    // And if fulfilled, call setGreenGivePoints() and update currentTask to the next task.
    private void setGreenGivePoints()
    { 
    }

}
