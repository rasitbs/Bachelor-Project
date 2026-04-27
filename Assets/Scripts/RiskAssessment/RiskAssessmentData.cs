using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRiskCategory", menuName = "Traftec/RiskCategory")]
public class RiskAssessmentData : ScriptableObject
{
    public string categoryTitle;
    public List<RiskOption> options;
}

[System.Serializable]
public class RiskOption
{
    public string label;
    public bool isCorrect;
}