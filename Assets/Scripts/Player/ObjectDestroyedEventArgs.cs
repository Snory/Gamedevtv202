using System;
using UnityEngine;

public class ObjectDestroyedEventArgs : EventArgs
{
    public GameObject DestroyedObject;

    public ObjectDestroyedEventArgs(GameObject enemyObject)
    {
        this.DestroyedObject = enemyObject;
    }
}