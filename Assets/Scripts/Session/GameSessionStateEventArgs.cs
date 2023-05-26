using System;

public class GameSessionStateEventArgs : EventArgs
{
    public GameSessionState State;

    public GameSessionStateEventArgs(GameSessionState currentState)
    {
        this.State = currentState;
    }
}