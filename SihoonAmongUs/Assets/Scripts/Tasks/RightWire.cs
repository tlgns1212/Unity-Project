using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightWire : MonoBehaviour
{
    public EWireColor WireColor { get; private set; }

    public bool IsConnected { get; private set; }

    [SerializeField]
    private List<Image> wireImages;

    [SerializeField]
    private Image lightImage;

    [SerializeField]
    private List<LeftWire> connectedWires = new List<LeftWire>();

    public void SetWireColor(EWireColor wireColor)
    {
        WireColor = wireColor;
        Color color = Color.black;
        switch (WireColor)
        {
            case EWireColor.Red:
                color = Color.red;
                break;
            case EWireColor.Blue:
                color = Color.blue;
                break;
            case EWireColor.Yellow:
                color = Color.yellow;
                break;
            case EWireColor.Magenta:
                color = Color.magenta;
                break;
        }

        foreach (var image in wireImages)
        {
            image.color = color;
        }
    }

    public void ConnectWire(LeftWire leftWire)
    {
        if (connectedWires.Contains(leftWire))
        {
            return;
        }

        connectedWires.Add(leftWire);
        if(connectedWires.Count == 1 && leftWire.WireColor == WireColor)
        {
            lightImage.color = Color.yellow;
            IsConnected = true;
        }
        else
        {
            lightImage.color = Color.grey;
            IsConnected = false;
        }
    }

    public void DisconnectWire(LeftWire leftWire)
    {
        connectedWires.Remove(leftWire);

        if(connectedWires.Count == 1 && connectedWires[0].WireColor == WireColor)
        {
            lightImage.color = Color.yellow;
            IsConnected = true;
        }
        else
        {
            lightImage.color = Color.grey;
            IsConnected = false;
        }
    }
}
