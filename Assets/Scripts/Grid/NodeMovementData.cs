using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public struct NodeMovementData : IEquatable<NodeMovementData>
{
    public Node Node;
    public Node ParentNode;
    public int MovementCostFromStart;
    public int MovementCostToEnd;
    public int Priority;

    public NodeMovementData(Node node, Node parentNode = null, int movementCostFromStart = 0, int movementCostToEnd = 0, int priority = 0)
    {
        Node = node;
        ParentNode = parentNode;
        MovementCostFromStart = movementCostFromStart;
        MovementCostToEnd = movementCostToEnd;
        Priority = priority;
    }

    public bool Equals(NodeMovementData other)
    {
        return
                other.Node == this.Node;
    }

    public override bool Equals(System.Object obj)
    {
        if (obj == null)
        {
            return false;
        }

        NodeMovementData node = (NodeMovementData)obj;
        return Equals(node);
    }

    public static bool operator ==(NodeMovementData node1, NodeMovementData node2)
    {
        if (((object)node1) == null || ((object)node2) == null)
            return Equals(node1, node2);

        return node1.Equals(node2);
    }

    public static bool operator !=(NodeMovementData node1, NodeMovementData node2)
    {
        if (((object)node1) == null || ((object)node2) == null)
            return !Equals(node1, node2);

        return !node1.Equals(node2);
    }

    public override int GetHashCode()
    {
        return Node.GetHashCode();
    }

}
