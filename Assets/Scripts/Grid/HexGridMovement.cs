using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


public enum HexGridMovementRequestType { DIRECTION, CLOSE_TO_PLAYER}

public class HexGridMovement : MonoBehaviour
{

    [SerializeField]
    private HexGrid _grid;


    public void OnGridMovementRequest(EventArgs args)
    {        
        HexGridMovementRequestEventArgs gridMovementRequestEventArgs = args as HexGridMovementRequestEventArgs;

        //Get the node where they are standing
        Node currentNode = _grid.GetNodeByGridPosition(gridMovementRequestEventArgs.FromPosition);

        //ok, do whatever they want with the target node
        Node targetnode = null;
        
        if(gridMovementRequestEventArgs.RequestType == HexGridMovementRequestType.DIRECTION)
        {
            targetnode = _grid.GetNodeInDirection(currentNode, _grid.GetGridDirection(gridMovementRequestEventArgs.Direction));
        } else
        {
            List<Node> path = CalculatePathToNode(currentNode, _grid.GetPlayerNode());

            if(path.Count > 0)
            {
                Debug.Log("Path count > 0");
                targetnode = path[0];
            }
        }

        //ooooo, they think they are allmighty and want to move with the grid itself!
        if (gridMovementRequestEventArgs.MoveGrid && targetnode != null)
        {
            Node currentCenterNode = _grid.GetNodeByWorldPosition(_grid.transform.position);

            if(currentCenterNode != null)
            {
                Node oppositeToTargetNode = _grid.GetNodeInDirection(currentCenterNode, _grid.GetGridOppositeDirection(gridMovementRequestEventArgs.Direction));
                _grid.SetGridWorldPosition(oppositeToTargetNode.WorldPosition);
            }
        }        


        gridMovementRequestEventArgs.MovementCallback(targetnode);
    }

    public List<Node> CalculatePathToNode(Node start, Node end)
    {
        Debug.Log("Calculating path from: " + start.ToString() + " to " + end.ToString());

        List<Node> open = new List<Node>();
        List<Node> close = new List<Node>();

        open.Add(start);

        bool pathFound = false;

        while(open.Count > 0)
        {
            Node currentNode = open.OrderBy(n => n.SeachPriority).ThenBy(n => n.MovementCostToEnd).First();

            open.Remove(currentNode);
            close.Add(currentNode);

            if(currentNode == end)
            {
                pathFound = true;
                break;
            }

            List<Node> nodes = _grid.GetNeighbours(currentNode);

            foreach (Node neighbor in nodes)
            {
                if (close.Contains(neighbor)) continue;

                if (neighbor.IsOccupiedByEnemy()) continue;

                float newMovementCostToNode = currentNode.MovementCostFromStart + _grid.GetDistanceBetweenNodes(currentNode, neighbor);

                if (newMovementCostToNode < neighbor.MovementCostFromStart || !open.Contains(neighbor))
                {
                    neighbor.MovementCostFromStart = newMovementCostToNode;
                    neighbor.MovementCostToEnd = _grid.GetDistanceBetweenNodes(neighbor, end);
                    neighbor.ParentNode = currentNode;

                    if (!open.Contains(neighbor))
                    {
                        open.Add(neighbor);
                    }
                }
            }
        }

        List<Node> retracedPath = null;

        if (pathFound)
        {
            retracedPath = RetracePath(start, end);
        }
        else
        {
            retracedPath = new List<Node>();
        }

        //clean references
        foreach (var n in open)
        {
            n.CleanMovementCosts();
        }

        foreach (var n in close)
        {
            n.CleanMovementCosts();
        }

        return retracedPath;


    }

    private List<Node> RetracePath(Node start, Node end)
    {
        List<Node> path = new List<Node>();
        Node current = end;
        while (current != start)
        {
            path.Add(current);
            current = current.ParentNode;
        }
        path.Reverse();
        return path;
    }


}


