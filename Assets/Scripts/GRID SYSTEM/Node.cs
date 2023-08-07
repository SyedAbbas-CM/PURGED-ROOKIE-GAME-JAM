using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector3 Position { get; set; } // Grid position
    public Vector3 WorldPosition { get; set; } // Actual world position
    public Node Parent { get; set; }
    public bool Walkable { get; set; }
    public int GCost { get; set; }
    public int HCost { get; set; }
    public int FCost => GCost + HCost;

    public Node()
    {
        Walkable = true;
        GCost = int.MaxValue;
    }
}