using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField]
    private Vector2Int _playerStartingPosition;

    [SerializeField]
    private GeneralEvent _gridMovementRequest;

    [SerializeField]
    private GeneralEvent _gridHighLightRequest;

    [SerializeField]
    private GeneralEvent _gameSessionStateEnd;

    [SerializeField]
    private GeneralEvent _requestForNode;

    [SerializeField]
    private GeneralEvent _entityPrepared;

    private int _countOfTurns;

    private Node _currentNode;

    private bool _initialized;

    [SerializeField]
    private bool _enabled;


    public void Update()
    {
        if (!_enabled) return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        //it is too fast so its skips some rounds
        HexGridDirection direction = HexGridDirection.NONE;

        if(vertical < 0)
        {
            if(horizontal < 0)
            {
                direction = HexGridDirection.LEFT_BOT;
            } else if (horizontal > 0)
            {
                direction = HexGridDirection.RIGTH_BOT;
            }
        } else if (vertical > 0)
        {
            if (horizontal < 0)
            {
                direction = HexGridDirection.LEFT_TOP;
            }
            else if (horizontal > 0)
            {
                direction = HexGridDirection.RIGHT_TOP;
            }
        } else if (horizontal < 0)
        {
            direction = HexGridDirection.LEFT;
        } else if(horizontal > 0)
        {
            direction= HexGridDirection.RIGHT;
        }

        if(direction != HexGridDirection.NONE)
        {
            _gridMovementRequest.Raise(new HexGridMovementRequestEventArgs(_currentNode.GridPosition, direction, OnGridMovement, true));
        }

    }
    public void OnGameSessionStateStart(EventArgs args)
    {

        GameSessionStateEventArgs gameSessionStateEventArgs = (GameSessionStateEventArgs) args;


        if (gameSessionStateEventArgs.State == GameSessionState.PLAYER_MOVEMENT)
        {
            _countOfTurns++;
            _enabled = true;

            if (!_initialized)
            {
                _initialized = true;
                RaiseMovementRequest(_playerStartingPosition, HexGridDirection.NONE);
            }          
        } 
    }
   
    public void MoveHexGridByComponent(HexGridMovementComponent direction)
    {
        RaiseMovementRequest(_currentNode.GridPosition, direction.Direction);
    }

    private void RaiseMovementRequest(Vector2Int fromPosition, HexGridDirection direction)
    {
        if (!_enabled) return;

        _gridMovementRequest.Raise(new HexGridMovementRequestEventArgs(fromPosition, direction, OnGridMovement, true));
    }

    public void OnGridMovement(Node n)
    {

        if (_currentNode != null)
        {
            _currentNode.SetPlayerObject(null);
        }
        
        if(n != null)
        {
            _currentNode = n;
            _currentNode.SetPlayerObject(this.gameObject);
            RaiseHighLightRequest(); 
            EndTurn();
        } else
        {
            Debug.Log("Playermovement[" + _countOfTurns + "]  node not found");
        }
    }

    private void EndTurn()
    {
        _enabled = false;
        _gameSessionStateEnd.Raise();
    }

    private void RaiseHighLightRequest()
    {
        _gridHighLightRequest.Raise(new HexGridHighlightRequestEventArgs(_currentNode, 2, NodeHighLightSource.PLAYER));
    }

    public void OnEntityPrepared(EventArgs args)
    {
        EntityPreparedEventArgs entityPreparedEventArgs = (EntityPreparedEventArgs)args;

        if (entityPreparedEventArgs.EntityPrepareType == EntityPrepareType.GRID && entityPreparedEventArgs.Prepared)
        {
            _requestForNode.Raise(new HexGridNodeRequestEventArgs(_playerStartingPosition, OnNodeFound));
        }
    }

    private void OnNodeFound(Node node, int range)
    {
        _currentNode = node;
        node.SetPlayerObject(this.gameObject);
        _entityPrepared.Raise(new EntityPreparedEventArgs(EntityPrepareType.PLAYER, true));
    }
}
