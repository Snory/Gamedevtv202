using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class MonsterMover : MonoBehaviour
{

    [SerializeField] 
    private List<MonsterMovement> _monsterMovements;

    [SerializeField]
    private GeneralEvent _gameSessionStateEnd;

    [SerializeField]
    private GeneralEvent _gameWon, _gameFinished;

    [SerializeField]
    private bool _initialized;

    public UnityEvent<int> CurrentTurn;


    public void OnMonstersGenerated(EventArgs args)
    {
        _monsterMovements = new List<MonsterMovement>();
        MonstersGeneratedEventArgs monstersGeneratedEventArgs = args as MonstersGeneratedEventArgs;
        List<GameObject> monsters = monstersGeneratedEventArgs.Monsters;

        foreach (GameObject monster in monsters)
        {
            MonsterMovement monsterMovement = monster.GetComponent<MonsterMovement>();
            _monsterMovements.Add(monsterMovement);
        }
    }

    public int GetMonsterMoverCount()
    {
        return _monsterMovements.Count;
    }


    public void OnGameSessionStateStart(EventArgs args)
    {
        GameSessionStateEventArgs gameSessionStateEventArgs = (GameSessionStateEventArgs) args;

        if (gameSessionStateEventArgs.State == GameSessionState.ENEMY_MOVEMENT)
        {
            Debug.Log("Monster turn");
            StartMovingMonsters();
        }
    }

    private void StartMovingMonsters()
    {
        StartCoroutine(MonsterMovementRoutine());
    }

    private IEnumerator MonsterMovementRoutine()
    {
        int monstersMoved = _monsterMovements.Count - 1;

        while (monstersMoved >= 0)
        {
            CurrentTurn?.Invoke(monstersMoved);
            MonsterMovement monsterMovement = _monsterMovements[monstersMoved];

            if (!_initialized)
            {                
                monsterMovement.Move(HexGridDirection.NONE); //just to play song
            } else
            {
                monsterMovement.Move(); //move to player
            }

            monstersMoved--;
            yield return new WaitForSeconds(1);
        }

        EndTurn();
    }

    public void OnGameObjectDestroyed(EventArgs args)
    {
        ObjectDestroyedEventArgs objectDestroyedEventArgs = (ObjectDestroyedEventArgs) args;

        MonsterMovement monsterMovement = objectDestroyedEventArgs.DestroyedObject.GetComponent<MonsterMovement>();

        if(monsterMovement != null)
        {
            _monsterMovements.Remove(monsterMovement);
        }

        if(_monsterMovements.Count == 0)
        {
            if (LevelManager.Instance.Lastlevel())
            {
                _gameFinished.Raise();
            } else
            {
                _gameWon.Raise();
            }
        }

    }

    private void EndTurn()
    {
        if (!_initialized)
        {
            _initialized = true;
        }
 
        _gameSessionStateEnd.Raise();
    }

}
