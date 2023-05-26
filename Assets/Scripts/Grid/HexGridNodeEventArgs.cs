using System;

public class HexGridNodeEventArgs : EventArgs
{
    public Node Node;

    public HexGridNodeEventArgs(Node node)
    {
        this.Node = node;
    }
}