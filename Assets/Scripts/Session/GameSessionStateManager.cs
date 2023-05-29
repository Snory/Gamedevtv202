using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameSessionState { TUTORIAL, PLAYER_MOVEMENT, GRID_RECALCULATE, ENEMY_MOVEMENT }
public enum EntityPrepareType { GRID, MONSTERS, PLAYER }

public class GameSessionStateManager : MonoBehaviour
{
    private GameSessionState _currentState;

    [SerializeField]
    private GeneralEvent _sessionStarted;

    [SerializeField]
    private Dictionary<EntityPrepareType, bool> _preparedEntities;

    private bool _tutorialReady;

    private void Awake()
    {
        _preparedEntities = new Dictionary<EntityPrepareType, bool>();

        foreach (var entity in Enum.GetNames(typeof(EntityPrepareType)))
        {
            _preparedEntities.Add((EntityPrepareType)Enum.Parse(typeof(EntityPrepareType), entity), false);
        }
    }

    private void Start()
    {
        if (LevelManager.Instance.GetCurrentLevel() > 0)
        {
            _tutorialReady = true;
        }
        else
        {
            _tutorialReady = false;
        }
    }

    public void OnSessionStateEnded()
    {

        if (_currentState == GameSessionState.TUTORIAL)
        {
            _tutorialReady = true;
            ChangeState(GameSessionState.ENEMY_MOVEMENT);
        }
        else if (_currentState == GameSessionState.PLAYER_MOVEMENT)
        {
            ChangeState(GameSessionState.GRID_RECALCULATE);
        }
        else if (_currentState == GameSessionState.GRID_RECALCULATE)
        {
            if (_tutorialReady)
            {
                ChangeState(GameSessionState.ENEMY_MOVEMENT);
            } else
            {
                ChangeState(GameSessionState.TUTORIAL);
            }

        }
        else if (_currentState == GameSessionState.ENEMY_MOVEMENT)
        {
            ChangeState(GameSessionState.PLAYER_MOVEMENT);
        }
    }

    public void OnEntityPrepared(EventArgs args)
    {
        EntityPreparedEventArgs entityPreparedEventArgs = (EntityPreparedEventArgs)args;
        _preparedEntities[entityPreparedEventArgs.EntityPrepareType] = true;
        StartIfReady();
    }

    private void StartIfReady()
    {
        bool ready = true;

        foreach (var r in _preparedEntities.Values)
        {
            if (!r)
            {
                ready = false;
                break;
            }

        }

        if (ready)
        {
            ChangeState(GameSessionState.PLAYER_MOVEMENT); //start with player movement
        }
    }

    private void ChangeState(GameSessionState newState)
    {
        _currentState = newState;
        _sessionStarted.Raise(new GameSessionStateEventArgs(_currentState));
    }
}
