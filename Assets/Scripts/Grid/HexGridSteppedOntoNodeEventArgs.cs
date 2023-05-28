using System;

public class HexGridSteppedOntoNodeEventArgs : EventArgs
{
    public Node Node;


    public HexGridSteppedOntoNodeEventArgs(Node node)
    {
        this.Node = node;
    }
}