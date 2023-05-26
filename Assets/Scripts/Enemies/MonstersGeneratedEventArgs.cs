using System;
using System.Collections.Generic;
using UnityEngine;

public class MonstersGeneratedEventArgs : EventArgs
{
    public List<GameObject> Monsters;

    public MonstersGeneratedEventArgs(List<GameObject> monsters)
    {
        this.Monsters = monsters;
    }
}