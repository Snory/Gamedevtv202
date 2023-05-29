using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnUI : MonoBehaviour
{
    [SerializeField]
    private MonsterMover _monsterMover;

    [SerializeField]
    private GameObject _enemyTurnobject, _playerTurnobject;

    [SerializeField]
    private Image _enemyTurnobjectImage;



    public void OnGameSessionStateStart(EventArgs args)
    {

        GameSessionStateEventArgs gameSessionStateEventArgs = (GameSessionStateEventArgs)args;

        if (gameSessionStateEventArgs.State == GameSessionState.PLAYER_MOVEMENT)
        {
            _enemyTurnobject.SetActive(false);
            _playerTurnobject.SetActive(true);

        }
        else if (gameSessionStateEventArgs.State == GameSessionState.ENEMY_MOVEMENT)
        {
            _enemyTurnobject.SetActive(true);
            _playerTurnobject.SetActive(false);
            _enemyTurnobjectImage.fillAmount = _monsterMover.GetMonsterMoverCount();
            _monsterMover.CurrentTurn.AddListener(OnCurrentTurn);

        }
        else
        {
            _enemyTurnobject.SetActive(false);
            _playerTurnobject.SetActive(false);
        }
    }

    private void OnCurrentTurn(int currentTurn)
    {
        _enemyTurnobjectImage.fillAmount = currentTurn / (float)_monsterMover.GetMonsterMoverCount();
    }
}
