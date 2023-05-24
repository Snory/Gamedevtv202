using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector2Int _playerStartingPosition;

    public GeneralEvent GridMovementRequest;


    public void OnGridReady()
    {
        GridMovementRequest.Raise(new HexGridMovementRequestEventArgs(this.transform.position, HexGridDirection.NONE, OnGridMovement, true));
    }
   
    public void MoveHexGridByComponent(HexGridMovementComponent direction)
    {
        GridMovementRequest.Raise(new HexGridMovementRequestEventArgs(this.transform.position, direction.Direction, OnGridMovement, true));
    }

    public void OnGridMovement(Node n)
    {
    }

    private void RaisePlayerMoved()
    {

    }
}
