using System;
using UnityEngine;

public class HexGridMovementRequestEventArgs : EventArgs
{
    public Vector3 FromPosition;
    public HexGridDirection Direction;
    public Action<Node> MovementCallback;
    public bool MoveGrid;
    public HexGridMovementRequestEventArgs(Vector3 fromPosition, HexGridDirection direction, Action<Node> movementCallback, bool moveGrid = false)
    {
        this.FromPosition = fromPosition;
        this.Direction = direction;
        MovementCallback = movementCallback;
        MoveGrid = moveGrid;
    }
}