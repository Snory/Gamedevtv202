using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameSessionState { PLAYER_MOVEMENT, GRID_RECALCULATE, ENEMY_MOVEMENT }
public enum EntityPrepareType { GRID, MONSTERS }

public class GameSessionStateManager : MonoBehaviour
{
    private GameSessionState _currentState;

    [SerializeField]
    private GeneralEvent _sessionStarted;

    [SerializeField]
    private Dictionary<EntityPrepareType, bool> _preparedEntities;

    private void Awake()
    {
        _preparedEntities = new Dictionary<EntityPrepareType, bool>();

        foreach(var entity in Enum.GetNames(typeof(EntityPrepareType))){
            _preparedEntities.Add((EntityPrepareType) Enum.Parse(typeof(EntityPrepareType), entity), false);
        }
    }

    public void OnSessionStateEnded()
    {
        Debug.Log("Game session ended: " + _currentState.ToString());

        if (_currentState == GameSessionState.PLAYER_MOVEMENT)
        {
            ChangeState(GameSessionState.GRID_RECALCULATE);
        }  
        else if (_currentState == GameSessionState.GRID_RECALCULATE)
        {
            ChangeState(GameSessionState.ENEMY_MOVEMENT);
        } else
        {
            ChangeState(GameSessionState.PLAYER_MOVEMENT);
        }
    }

    public void OnEntityPrepared(EventArgs args)
    {
        EntityPreparedEventArgs entityPreparedEventArgs = (EntityPreparedEventArgs)args;
        Debug.Log("Entity prepared: " + entityPreparedEventArgs.EntityPrepareType);
        _preparedEntities[entityPreparedEventArgs.EntityPrepareType] =  true;
        StartIfReady();
    }

    private void StartIfReady()
    {
        bool ready = true;

        foreach(var r in _preparedEntities.Values)
        {
            if (!r)
            {
                ready = false;
                break;
            }

        }

        if (ready)
        {
            Debug.Log("Game ready");
            ChangeState(GameSessionState.PLAYER_MOVEMENT); //start with player movement
        }
    }

    private void ChangeState(GameSessionState newState)
    {
        _currentState = newState;
        Debug.Log("Session change state raised: " + _currentState.ToString());
        _sessionStarted.Raise(new GameSessionStateEventArgs(_currentState));
    }
}
