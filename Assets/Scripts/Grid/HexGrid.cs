using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

[Serializable]
public enum HexGridDirection : int { LEFT_TOP = 0, LEFT = 1, LEFT_BOT = 2, RIGHT_TOP = 5, RIGHT = 4, RIGTH_BOT = 3, NONE = 6 }

public class HexGrid : MonoBehaviour
{
    private Dictionary<Vector2Int, Node> _nodes; // i will not have to think about how to generate it :)

    [SerializeField]
    private float _hexWidth, _hexHeight;

    [SerializeField]
    private GameObject _hexTilePrefab;

    [SerializeField]
    private bool _visible;

    [SerializeField]
    private GeneralEvent _gridEntityPrepared;

    [SerializeField]
    private Sprite[] _nodeSprites;

    [SerializeField]
    private GeneralEvent _gameSessionStateEnd;


    [SerializeField]
    private int[] _gridSizePerLevel;


    private Vector2Int[] _directionCoords = new Vector2Int[]
    {
            new Vector2Int(0,-1), // 0
            new Vector2Int(-1,0), //1
            new Vector2Int(-1,1), //2
            new Vector2Int(0,1), //3
            new Vector2Int(1,0), //4
            new Vector2Int(1,-1), //5
            new Vector2Int (0,0)

    };

    private Vector2Int[] _oppositeDirectionCoords = new Vector2Int[]
    {

        new Vector2Int(0,1),
        new Vector2Int(1,0),
        new Vector2Int(1,-1),
        new Vector2Int(0,-1),
        new Vector2Int(-1,0),
        new Vector2Int(-1,1),
        new Vector2Int (0,0) //None movement
    };

    [SerializeField]
    private HexGridSoundSetting _soundSetting;

    public Vector2Int GetGridOppositeDirection(HexGridDirection direction)
    {
        return _oppositeDirectionCoords[(int)direction];
    }

    public Vector2Int GetGridDirection(HexGridDirection direction)
    {
        return _directionCoords[(int)direction];
    }


    public void Start()
    {
        GenerateGrid();
    }

    public void OnGridCircleRequest(EventArgs args)
    {
        HexGridCircleNodeRequestEventArgs hexGridCircleNodeRequestEventArgs = args as HexGridCircleNodeRequestEventArgs;

        Node n = GetNodeByWorldPosition(hexGridCircleNodeRequestEventArgs.Position);

        if (n != null)
        {
            hexGridCircleNodeRequestEventArgs.CallBack(TryToGetNodeInRing(n, hexGridCircleNodeRequestEventArgs.Range), hexGridCircleNodeRequestEventArgs.Range);
        }
    }


    [ContextMenu("Generate grid")]
    private void GenerateGrid()
    {
        ClearGrid();


        int currentLevel = _gridSizePerLevel.Length;
        if (LevelManager.Instance.GetCurrentLevel() < _gridSizePerLevel.Length)
        {
            currentLevel = LevelManager.Instance.GetCurrentLevel();
        }


        List<Vector2Int> coordinations = NodesGridCoordinationInRange(new Vector2Int(0, 0), _gridSizePerLevel[currentLevel]);

        foreach (Vector2Int coordination in coordinations)
        {
            Vector2 worldPosition = CalculateWorldPosition(coordination.x, coordination.y);

            Node node = Instantiate(_hexTilePrefab, worldPosition, Quaternion.identity, this.transform).GetComponent<Node>();
            node.gameObject.name = coordination.ToString();
            node.SetNode(_nodeSprites[Random.Range(0, _nodeSprites.Length)], coordination, worldPosition);
            node.SetHightLight(_visible);
            _nodes.Add(node.GridPosition, node);
        }

        _gridEntityPrepared.Raise(new EntityPreparedEventArgs(EntityPrepareType.GRID, true));

    }

    private List<Vector2Int> NodesGridCoordinationInRange(Vector2Int center, int range)
    {
        List<Vector2Int> nodes = new List<Vector2Int>();

        for (int q = -range; q <= range; q++)
        {
            int r1 = Math.Max(-range, -q - range);
            int r2 = Math.Min(range, -q + range);


            for (int r = r1; r <= r2; r++)
            {
                int x = center.x + q;
                int y = center.y + r;

                nodes.Add(new Vector2Int(x, y));
            }
        }
        return nodes;
    }

    public List<Node> GetNodeList()
    {
        return _nodes.Values.ToList();
    }


    public void OnHighLightRequest(EventArgs args)
    {
        HexGridHighlightRequestEventArgs hexGridHighlightRequestEventArgs = args as HexGridHighlightRequestEventArgs;

        if (hexGridHighlightRequestEventArgs.HighlightSource == NodeHighLightSource.PLAYER)
        {
            Vector2Int currentNodePosition = hexGridHighlightRequestEventArgs.CurrentNode.GridPosition;
            List<Vector2Int> nodeCoordsAround = NodesGridCoordinationInRange(currentNodePosition, hexGridHighlightRequestEventArgs.Range);


            foreach (Node n in _nodes.Values)
            {
                if (n.HighLightSource == NodeHighLightSource.PLAYER)
                {
                    n.SetHightLight(false);
                    n.SetClip(null);
                }
            }



            foreach (Vector2Int nodeCoord in nodeCoordsAround)
            {
                Node node = TryGetNodeByGridPosition(nodeCoord);

                if (node == null || node == hexGridHighlightRequestEventArgs.CurrentNode)
                {
                    continue;
                }

                node.SetHightLight(true, 0.3f, NodeHighLightSource.PLAYER);

                Vector2Int coordinates = new Vector2Int(node.GridPosition.x - currentNodePosition.x, node.GridPosition.y - currentNodePosition.y);

                AudioClip sound = _soundSetting.SoundSettings.Where(st => st.NodeCoordination == new Vector2Int(node.GridPosition.x - currentNodePosition.x, node.GridPosition.y - currentNodePosition.y)).First().Sound;
                node.SetClip(sound);

            }

            hexGridHighlightRequestEventArgs.CurrentNode.SetHightLight(true, 1, NodeHighLightSource.PLAYER);
        }
    }

    private void ClearGrid()
    {
        if (_nodes != null)
        {
            foreach (Node node in _nodes.Values)
            {
                DestroyImmediate(node.gameObject);
            }
        }
        _nodes = new Dictionary<Vector2Int, Node>();
    }

    private Vector2 CalculateWorldPosition(int q, int r)
    {

        float x = this.transform.position.x + _hexWidth * (Mathf.Sqrt(3f) * q + Mathf.Sqrt(3f) / 2 * r) * this.transform.lossyScale.x;
        float y = this.transform.position.y - _hexHeight * ((3f / 2f) * r) * this.transform.lossyScale.y;

        return new Vector2(x, y);
    }

    private void RecalculateWorldPosition()
    {
        foreach (Node n in _nodes.Values)
        {
            n.SetWorldInPosition(CalculateWorldPosition(n.GridPosition.x, n.GridPosition.y));
        }
    }

    public void SetGridWorldPosition(Vector3 worldPosition)
    {
        this.transform.position = worldPosition;
    }

    public Node GetNodeByWorldPosition(Vector3 worldPosition)
    {
        float x = worldPosition.x - this.transform.position.x;
        float y = worldPosition.y - this.transform.position.y;
        float scaledWidth = this.transform.lossyScale.x * _hexWidth;
        float scaledHeight = this.transform.lossyScale.y * _hexHeight;

        float q = Mathf.Sqrt(3f) / 3f * x / scaledWidth + (1f / 3f) * y / scaledHeight;
        float r = (2f / 3f * -y / scaledHeight);


        //rounding for floating point errors
        int roundedQ = Mathf.RoundToInt(q);
        int roundedR = Mathf.RoundToInt(r);
        int roundedS = Mathf.RoundToInt(-q - r);

        float qDiff = Mathf.Abs(roundedQ - q);
        float rDiff = Mathf.Abs(roundedR - r);
        float sDiff = Mathf.Abs(roundedS - (-q - r));

        if (qDiff > rDiff && qDiff > sDiff)
        {
            roundedQ = -roundedR - roundedS;
        }
        else if (rDiff > sDiff)
        {
            roundedR = -roundedQ - roundedS;
        }

        Node n = null;

        _nodes.TryGetValue(new Vector2Int(roundedQ, roundedR), out n);

        return n;
    }


    public Node TryGetNodeByGridPosition(Vector2Int gridPosition)
    {
        Node n = null;
        _nodes.TryGetValue(gridPosition, out n);
        return n;
    }

    public Node TryGetNodeInDirection(Node from, Vector2Int direction)
    {


        Node n = null;

        if (from == null)
        {
            return n;
        }

        _nodes.TryGetValue(new Vector2Int(from.GridPosition.x + direction.x, from.GridPosition.y + direction.y), out n);
        return n;
    }

    public Vector2Int GetDirectionToNode(Node from, Node To)
    {
        Vector2Int direction = new Vector2Int(To.GridPosition.x - from.GridPosition.x, To.GridPosition.y - from.GridPosition.y);

        return direction;
    }

    public Node TryGetPlayerNode()
    {
        Node n = null;

        foreach (var node in _nodes.Values)
        {
            if (node.IsOccupiedByPlayer())
            {
                n = node;
                break;
            }
        }

        return n;
    }

    public List<Node> TryGetNeighbours(Node node)
    {
        List<Node> nodes = new List<Node>();

        foreach (var dir in _directionCoords)
        {
            Node n = TryGetNodeInDirection(node, dir);

            if (n != null && n != node)
            {
                nodes.Add(TryGetNodeInDirection(node, dir));
            }
        }

        return nodes;
    }

    private Vector2Int ScaleDirection(Vector2Int direction, int scale)
    {
        return new Vector2Int(direction.x * scale, direction.y * scale);
    }

    public Vector2Int GetGridCoordinationInDirection(Vector2Int center, Vector2Int direction)
    {
        return new Vector2Int(center.x + direction.x, center.y + direction.y);
    }

    public List<Node> TryToGetNodeInRing(Node center, int range)
    {
        List<Node> nodesInRing = new List<Node>();

        Node node = TryGetNodeInDirection(center, ScaleDirection(_directionCoords[4], range));
        Vector2Int current = GetGridCoordinationInDirection(center.GridPosition, ScaleDirection(_directionCoords[4], range));


        for (int i = 0; i < 6; i++)
        {
            for (int r = 0; r < range; r++)
            {
                if (node != null)
                {
                    nodesInRing.Add(node);
                }

                current = GetGridCoordinationInDirection(current, _directionCoords[i]);
                node = TryGetNodeByGridPosition(current);
            }
        }


        return nodesInRing;
    }

    public void OnHexGridRequestNode(EventArgs args)
    {
        HexGridNodeRequestEventArgs hexGridNodeRequestEventArgs = args as HexGridNodeRequestEventArgs;

        Node reqeustedNode = GetNodeByWorldPosition(hexGridNodeRequestEventArgs.RequestedNodeWorldPosition);
        Node requestorNode = GetNodeByWorldPosition(hexGridNodeRequestEventArgs.RequestorNodeWorldPosition);

        int range = -1;
        if (reqeustedNode != null && requestorNode != null)
        {

            range = GetDistanceBetweenNodes(requestorNode, reqeustedNode);
        }

        hexGridNodeRequestEventArgs.OnNodeFound(reqeustedNode, range);
    }

    public int GetDistanceBetweenNodes(Node from, Node to)
    {
        return GetDistanceBetweenGridCoordinations(from.GridPosition, to.GridPosition);
    }

    private int GetDistanceBetweenGridCoordinations(Vector2Int from, Vector2Int to)
    {
        //vector
        Vector2Int direction = new Vector2Int(from.x - to.x, from.y - to.y);
        return (Mathf.Abs(direction.x) + Mathf.Abs(direction.x + direction.y) + Mathf.Abs(direction.y)) / 2;
    }

    public void OnGameSessionStateStart(EventArgs args)
    {
        GameSessionStateEventArgs gameSessionStateEventArgs = (GameSessionStateEventArgs)args;

        if (gameSessionStateEventArgs.State == GameSessionState.GRID_RECALCULATE)
        {
            RecalculateWorldPosition();
            EndTurn();
        }
    }

    private void EndTurn()
    {
        _gameSessionStateEnd.Raise();
    }
}
