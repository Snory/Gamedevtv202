using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField]
    private GeneralEvent _restartLevel;

    [SerializeField]
    private int _currentLevel;

    [SerializeField]
    private int _maxLevel;

    [SerializeField]
    private int _currentScore;

    public UnityEvent<int> ScoreChanged;

    private void Start()
    {
        _currentScore = 0;
        ScoreChanged?.Invoke(_currentScore);

    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (_currentLevel <= _maxLevel)
            {
                _restartLevel.Raise();
            }
        }
    }

    public int GetCurrentLevel()
    {
        return _currentLevel;
    }

    public bool Lastlevel()
    {
        return _currentLevel == _maxLevel;
    }

    public void AddLevel()
    {
        _currentLevel++;
    }

    public void OnGameOver()
    {
        _currentLevel = 0;
    }

    public void OnScoreChanged(EventArgs args)
    {
        ScoreChangedEventArgs scoreChangedEventArgs = args as ScoreChangedEventArgs;
        _currentScore += scoreChangedEventArgs.Score;
        ScoreChanged?.Invoke(_currentScore);
    }


}
