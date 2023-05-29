using System;
using System.Collections.Generic;
using UnityEngine;

public class HexGridCircleNodeRequestEventArgs : EventArgs
{
    public Vector3 Position;
    public int Range;
    public Action<List<Node>, int> CallBack;

    public HexGridCircleNodeRequestEventArgs(Vector3 position, int range, Action<List<Node>, int> callback)
    {
        this.Position = position;
        this.Range = range;
        this.CallBack = callback;
    }
}