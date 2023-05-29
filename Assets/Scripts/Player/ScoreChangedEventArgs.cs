using System;

public class ScoreChangedEventArgs : EventArgs
{
    public int Score;

    public ScoreChangedEventArgs(int score)
    {
        this.Score = score;
    }
}