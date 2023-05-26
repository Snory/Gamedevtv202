using System;
using UnityEngine;

public class HexGridMovementRequestEventArgs : EventArgs
{
    public Vector2Int FromPosition;
    public HexGridDirection Direction;
    public Action<Node> MovementCallback;
    public HexGridMovementRequestType RequestType;
    public bool MoveGrid;
    public HexGridMovementRequestEventArgs(Vector2Int fromPosition, HexGridDirection direction, Action<Node> movementCallback, bool moveGrid = false)
    {
        this.FromPosition = fromPosition;
        this.Direction = direction;
        MovementCallback = movementCallback;
        MoveGrid = moveGrid;
        RequestType = HexGridMovementRequestType.DIRECTION;
    }

    public HexGridMovementRequestEventArgs(Vector2Int fromPosition, Action<Node> movementCallback)
    {
        this.FromPosition = fromPosition;
        this.Direction = HexGridDirection.NONE;
        MovementCallback = movementCallback;
        MoveGrid = false;
        RequestType = HexGridMovementRequestType.CLOSE_TO_PLAYER;
    }
}