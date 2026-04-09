using UnityEngine;

public class MultimeterProbe : MonoBehaviour
{
    private ElectricalNode connectedNode = null;
    public void ConnectToNode(ElectricalNode node)
    {
        connectedNode = node;
    }

    public void Disconnect()
    {
        connectedNode = null;
    }

    public bool IsConnected => connectedNode != null;

    public float GetPotential()
    {
        return IsConnected ? connectedNode.GetVoltage() : 0f;
    }
}