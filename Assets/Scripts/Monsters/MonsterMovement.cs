using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    [SerializeField]
    private Node _currentNode;

    [SerializeField]
    private GeneralEvent _nodeSteppedOnto;

    [SerializeField]
    private GeneralEvent _gridMovementRequest;

    public void SetCurrentNode(Node n)
    {
        _currentNode = n;
    }

    public void OnGridMovement(Node n)
    {
        if (n == null) return;


        if(_currentNode != null)
        {
            _currentNode.SetEnemyObject(null);
        } 

        _currentNode = n;
        _currentNode.SetEnemyObject(this.gameObject);
        this.transform.position = n.WorldPosition;

        _nodeSteppedOnto.Raise(new HexGridSteppedOntoNodeEventArgs(_currentNode));
    }


    public void Move(HexGridDirection direction)
    {
        _gridMovementRequest.Raise(new HexGridMovementRequestEventArgs(_currentNode.GridPosition, direction, OnGridMovement, false));
    }

    public void Move()
    {
        _gridMovementRequest.Raise(new HexGridMovementRequestEventArgs(_currentNode.GridPosition, OnGridMovement));
    }
}
