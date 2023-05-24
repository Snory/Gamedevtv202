using System;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public enum HexGridDirection : int { LEFT_TOP = 0, LEFT = 1, LEFT_BOT = 2, RIGHT_TOP = 3, RIGHT = 4, RIGTH_BOT = 5, NONE = 6 }

public class HexGridMovement : MonoBehaviour
{

    [SerializeField]
    private HexGrid _grid;

    Vector2Int[] directionCoords = new Vector2Int[]
    {
            new Vector2Int(0,-1), //0
            new Vector2Int(-1,0), //1
            new Vector2Int(-1,1), //2
            new Vector2Int(1,-1), //3
            new Vector2Int(1,0), //4
            new Vector2Int(0,1), //5
            new Vector2Int (0,0) //None movement
    };

    Vector2Int[] oppositeDirectionCoords = new Vector2Int[]
    {
        new Vector2Int(0,1),
        new Vector2Int(1,0),
        new Vector2Int(1,-1),
        new Vector2Int(-1,1),
        new Vector2Int(-1,0),
        new Vector2Int(0,-1),
        new Vector2Int (0,0) //None movement
    };

    public void OnGridMovementRequest(EventArgs args)
    {        
        HexGridMovementRequestEventArgs gridMovementRequestEventArgs = args as HexGridMovementRequestEventArgs;

        //Get the node where they are standing
        Node currentNode = _grid.GetNodeByWorldPosition(gridMovementRequestEventArgs.FromPosition);

        //ok, do whatever they want with the target node
        Node targetnode = _grid.GetNodeInDirection(currentNode, directionCoords[(int) gridMovementRequestEventArgs.Direction]);
        gridMovementRequestEventArgs.MovementCallback(targetnode);

        if(targetnode == null)
        {
            return;
        }

        //ooooo, they think they are allmighty and want to move with the grid itself!
        if (gridMovementRequestEventArgs.MoveGrid)
        {
            Node currentCenterNode = _grid.GetNodeByWorldPosition(_grid.transform.position);
            Node oppositeToTargetNode = _grid.GetNodeInDirection(currentCenterNode, oppositeDirectionCoords[(int)gridMovementRequestEventArgs.Direction]);
            Debug.Log("Opposite target node coordinations: " + oppositeToTargetNode.GridPosition.ToString());
            _grid.SetGridWorldPosition(oppositeToTargetNode.WorldPosition);
        }        
    }


}


