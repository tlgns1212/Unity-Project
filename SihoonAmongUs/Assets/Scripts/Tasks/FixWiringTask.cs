using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EWireColor
{
    None = -1,
    Red,
    Blue,
    Yellow,
    Magenta
}

public class FixWiringTask : MonoBehaviour
{
    [SerializeField]
    private List<LeftWire> leftWires;

    [SerializeField]
    private List<RightWire> rightWires;

    private LeftWire selectedWire;

    private void OnEnable()
    {
        for(int i = 0; i < leftWires.Count; i++)
        {
            leftWires[i].ResetTarget();
            leftWires[i].DisconnectWire();
        }

        List<int> numberPool = new List<int>();
        for(int i = 0; i < 4; i++)
        {
            numberPool.Add(i);
        }

        int index = 0;
        while(numberPool.Count != 0)
        {
            var number = numberPool[Random.Range(0, numberPool.Count)];
            leftWires[index++].SetWireColor((EWireColor)number);
            numberPool.Remove(number);
        }

        for (int i = 0; i < 4; i++)
        {
            numberPool.Add(i);
        }

        index = 0;
        while (numberPool.Count != 0)
        {
            var number = numberPool[Random.Range(0, numberPool.Count)];
            rightWires[index++].SetWireColor((EWireColor)number);
            numberPool.Remove(number);
        }
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Input.mousePosition, Vector2.right, 1f);
            if (hit.collider != null)
            {
                var left = hit.collider.GetComponentInParent<LeftWire>();
                if (left != null)
                {
                    selectedWire = left;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (selectedWire != null)
            {
                RaycastHit2D[] hits = Physics2D.RaycastAll(Input.mousePosition, Vector2.right, 1f);
                foreach (var hit in hits)
                {
                    if (hit.collider != null)
                    {
                        var right = hit.collider.GetComponentInParent<RightWire>();
                        if (right != null)
                        {
                            selectedWire.SetTarget(hit.transform.position, -2f);
                            selectedWire.ConnectWire(right);
                            right.ConnectWire(selectedWire);
                            selectedWire = null;
                            CheckCompleteTask();
                            return;
                        }
                    }
                }

                selectedWire.ResetTarget();
                selectedWire.DisconnectWire();
                selectedWire = null;
                CheckCompleteTask();
            }
        }

        if (selectedWire != null)
        {
            selectedWire.SetTarget(Input.mousePosition, -2f);
        }
    }

    private void CheckCompleteTask()
    {
        bool isAllComplete = true;
        foreach(var wire in leftWires)
        {
            if (!wire.IsConnected)
            {
                isAllComplete = false;
                break;
            }
        }

        if (isAllComplete)
        {
            Close();
        }
    }

    public void Open()
    {
        AmongUsRoomPlayer.MyRoomPlayer.myCharacter.IsMoveable = false;
        gameObject.transform.parent.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    public void Close()
    {
        AmongUsRoomPlayer.MyRoomPlayer.myCharacter.IsMoveable = true;
        gameObject.transform.parent.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
