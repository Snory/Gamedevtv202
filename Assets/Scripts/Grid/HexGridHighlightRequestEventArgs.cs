using System;

public class HexGridHighlightRequestEventArgs : EventArgs
{
    public Node CurrentNode;
    public int Range;
    public NodeHighLightSource HighlightSource;

    public HexGridHighlightRequestEventArgs(Node currentNode, int range, NodeHighLightSource highLightSource)
    {
        this.CurrentNode = currentNode;
        this.Range = range;
        this.HighlightSource = highLightSource;
    }
}