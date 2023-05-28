using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

[Serializable]
public enum HexGridDirection : int { LEFT_TOP = 0, LEFT = 1, LEFT_BOT = 2, RIGHT_TOP = 3, RIGHT = 4, RIGTH_BOT = 5, NONE = 6 }

public class HexGrid : MonoBehaviour
{
    private Dictionary<Vector2Int, Node> _nodes; // i will not have to think about how to generate it :)

    [SerializeField]
    private int _gridSize;

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


    private Vector2Int[] _directionCoords = new Vector2Int[]
    {
            new Vector2Int(0,-1), //0
            new Vector2Int(-1,0), //1
            new Vector2Int(-1,1), //2
            new Vector2Int(1,-1), //3
            new Vector2Int(1,0), //4
            new Vector2Int(0,1), //5
            new Vector2Int (0,0) //None movement
    };

    private Vector2Int[] _oppositeDirectionCoords = new Vector2Int[]
    {
        new Vector2Int(0,1),
        new Vector2Int(1,0),
        new Vector2Int(1,-1),
        new Vector2Int(-1,1),
        new Vector2Int(-1,0),
        new Vector2Int(0,-1),
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


    [ContextMenu("Generate grid")]
    private void GenerateGrid()
    {
        ClearGrid();


        List<Node> nodes = NodesInRange(new Vector2Int(0, 0), _gridSize);

        foreach (Node node in nodes)
        {
            GameObject hexTileGO = Instantiate(_hexTilePrefab, node.WorldPosition, Quaternion.identity, this.transform);
            hexTileGO.name = node.GridPosition.ToString();
            node.SetGameObject(hexTileGO, _nodeSprites[Random.Range(0, _nodeSprites.Length)]);
            node.SetHightLight(_visible);
            _nodes.Add(node.GridPosition, node);
        }

        _gridEntityPrepared.Raise(new EntityPreparedEventArgs(EntityPrepareType.GRID, true));

    }

    private List<Node> NodesInRange(Vector2Int center, int range)
    {
        List<Node> nodes = new List<Node>();

        for (int q = -range; q <= range; q++)
        {
            int r1 = Math.Max(-range, -q - range);
            int r2 = Math.Min(range, -q + range);


            for (int r = r1; r <= r2; r++)
            {
                float axialDistance = Mathf.Abs(q) + Mathf.Abs(r);


                int x = center.x + q;
                int y = center.y + r;

                Vector2Int gridCoordinates = new Vector2Int(x, y);
                Vector2 worldCoordinates = CalculateWorldPosition(x, y);

                nodes.Add(new Node(gridCoordinates, worldCoordinates, r));
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
            List<Node> nodesAround = NodesInRange(currentNodePosition, hexGridHighlightRequestEventArgs.Range);

            foreach (Node n in _nodes.Values)
            {
                if (nodesAround.Contains(n) && n != hexGridHighlightRequestEventArgs.CurrentNode)
                {
                    n.SetHightLight(true, 0.3f, NodeHighLightSource.PLAYER);

                    Vector2Int coordinates = new Vector2Int(n.GridPosition.x - currentNodePosition.x, n.GridPosition.y - currentNodePosition.y);

                    AudioClip sound = _soundSetting.SoundSettings.Where(st => st.NodeCoordination == new Vector2Int(n.GridPosition.x - currentNodePosition.x, n.GridPosition.y - currentNodePosition.y)).First().Sound;
                    n.SetClip(sound);
                }
                else
                {
                    if (n.HighLightSource == NodeHighLightSource.PLAYER)
                    {
                        n.SetHightLight(false);
                        n.SetClip(null);
                    }
                }
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
                DestroyImmediate(node.GetGameObject());
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
        Debug.Log("Center of the world" + this.transform.position.ToString());
        foreach (Node n in _nodes.Values)
        {
            n.SetWorldInPosition(CalculateWorldPosition(n.GridPosition.x, n.GridPosition.y));
            Debug.Log("Calculated RequestedNodeWorldPosition: " + n.ToString());
        }
    }

    public void SetGridWorldPosition(Vector3 worldPosition)
    {
        Debug.Log("Set center of the world: " + worldPosition);
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


    public Node GetNodeByGridPosition(Vector2Int gridPosition)
    {
        return _nodes[gridPosition];
    }

    public Node GetNodeInDirection(Node from, Vector2Int direction)
    {
        Node n;
        _nodes.TryGetValue(new Vector2Int(from.GridPosition.x + direction.x, from.GridPosition.y + direction.y), out n);
        return n;
    }

    public Vector2Int GetDirectionToNode(Node from, Node To)
    {
        Vector2Int direction = new Vector2Int(To.GridPosition.x - from.GridPosition.x, To.GridPosition.y - from.GridPosition.y);

        return direction;
    }

    public Node GetPlayerNode()
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

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> nodes = new List<Node>();

        foreach (var dir in _directionCoords)
        {
            Node n = GetNodeInDirection(node, dir);

            if (n != null && n != node)
            {
                nodes.Add(GetNodeInDirection(node, dir));
            }
        }

        return nodes;
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
