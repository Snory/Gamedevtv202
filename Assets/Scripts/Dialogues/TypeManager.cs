using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;


public class TypeManager : MonoBehaviour
{

    [SerializeField]
    private TypeWriter _writter;

    [SerializeField]
    private TextMeshProUGUI _text;

    [SerializeField]
    private GameObject _writterPanel;

    [SerializeField]
    private string[] _stringToWrite;

    private int _currentStringToWrite;

    [SerializeField]
    private GeneralEvent _gamesessionstateEnd;

    public UnityEvent Next;


    public void OnWritting(string text)
    {
        _text.text = text;
    }

    public void OnFinished()
    {
        if (_currentStringToWrite == _stringToWrite.Length - 1)
        {
            StartCoroutine(Turnoff());
        }
    }

    private IEnumerator Turnoff()
    {
        yield return new WaitForSeconds(4);
        TurnedOff();
    }

    public void OnNextText()
    {
        Next?.Invoke();

        _currentStringToWrite++;
        if (_currentStringToWrite < _stringToWrite.Length)
        {
            _writter.Type(_stringToWrite[_currentStringToWrite], OnWritting, OnFinished);
        }else if (_currentStringToWrite >= _stringToWrite.Length)
        {
            TurnedOff();
        }
    }

    private void TurnedOff()
    {
        _writterPanel.SetActive(false);
        _gamesessionstateEnd.Raise();
    }

    public void OnGameSessionStateStart(EventArgs args)
    {
        GameSessionStateEventArgs gameSessionStateEventArgs = (GameSessionStateEventArgs)args;

        if (gameSessionStateEventArgs.State == GameSessionState.TUTORIAL)
        {
            _writterPanel.SetActive(true);
            _currentStringToWrite = 0;
            _writter.Type(_stringToWrite[_currentStringToWrite], OnWritting, OnFinished);
        }

    }


}
