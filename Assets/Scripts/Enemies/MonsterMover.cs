using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
public class MonsterMover : MonoBehaviour
{

    [SerializeField] 
    private List<MonsterMovement> _monsterMovements;

    [SerializeField]
    private GeneralEvent _gameSessionStateEnd;

    [SerializeField]
    private bool _initialized;

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
        int monstersMoved = 0;

        Debug.Log("Count of monsters:" + _monsterMovements.Count);

        while (monstersMoved < _monsterMovements.Count)
        {
            MonsterMovement monsterMovement = _monsterMovements[monstersMoved];

            if (!_initialized)
            {                
                monsterMovement.Move(HexGridDirection.NONE); //just to play song
            } else
            {
                monsterMovement.Move(); //move to player
            }

            monstersMoved++;
            Debug.Log("MonsterMoved: " + monstersMoved +", MonsterCount: " + _monsterMovements.Count);
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
