using System;
using UnityEngine;

public class HexGridNodeRequestEventArgs : EventArgs
{
    public Vector3 RequestorNodeWorldPosition;
    public Vector2Int RequestorNodeGridRotation;
    public Vector3 RequestedNodeWorldPosition;
    public Vector2Int RequestedGridPosition;
    public Action<Node, int> OnNodeFound;


    public HexGridNodeRequestEventArgs(Vector3 requestedNodeWorldPosition, Action<Node, int> onNodeFound)
    {
        this.RequestedNodeWorldPosition = requestedNodeWorldPosition;
        this.OnNodeFound = onNodeFound;
    }

    public HexGridNodeRequestEventArgs(Vector3 requestorNodeWorldPosition, Vector3 requestedNodeWorldPosition, Action<Node, int> onNodeFound)
    {
        this.RequestorNodeWorldPosition = requestorNodeWorldPosition;
        this.RequestedNodeWorldPosition = requestedNodeWorldPosition;
        this.OnNodeFound = onNodeFound;
    }

    public HexGridNodeRequestEventArgs(Vector2Int gridPosition, Action<Node, int> onNodeFound)
    {
        this.RequestedGridPosition = gridPosition;
        this.OnNodeFound = onNodeFound;
    }

    public HexGridNodeRequestEventArgs(Vector2Int requestorNodeGridPosition, Vector2Int requestedNodeGridPosition, Action<Node, int> onNodeFound)
    {
        this.RequestorNodeGridRotation = requestorNodeGridPosition;
        this.RequestedGridPosition = requestedNodeGridPosition;
        this.OnNodeFound = onNodeFound;
    }

}