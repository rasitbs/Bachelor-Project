using UnityEngine;

[CreateAssetMenu(fileName = "NewRoutine", menuName = "Traftec/Routine")]
public class RoutineData : ScriptableObject
{
    public string title;
    public Sprite[] pages; // Dra inn alle sider her i Inspector
}