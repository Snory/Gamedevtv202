using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum HexGridVisualisation { EMPTY, PLAYERONLY, FULL}


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
    private HexGridVisualisation _hexgridVisualisation;

    [SerializeField]
    private GeneralEvent _gridReady;

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
            node.SetGameObject(hexTileGO);
            node.SetHightLight(false);

            _nodes.Add(node.GridPosition, node);
        }
        _gridReady.Raise();
    }

    private List<Node> NodesInRange(Vector2Int center, int range)
    {
        List<Node> nodes = new List<Node>();

        int sortingOrder = 0;

        for (int q = -range; q <= range; q++)
        {
            int r1 = Math.Max(-range, -q - range);
            int r2 = Math.Min(range, -q + range);

            sortingOrder = 0;

            for (int r = r1; r <= r2; r++)
            {               
                sortingOrder++;
                
                int x = center.x + q;
                int y = center.y + r;

                Vector2Int gridCoordinates = new Vector2Int(x, y);
                Vector2 worldCoordinates = CalculateWorldPosition(x, y);

                nodes.Add(new Node(gridCoordinates, worldCoordinates, sortingOrder));
            }
        }
        return nodes;
    }

    private void ClearGrid()
    {
        if (_nodes != null)
        {
            foreach(Node node in _nodes.Values)
            {
                DestroyImmediate(node.GetGameObject());
            }
        }
        _nodes = new Dictionary<Vector2Int, Node>();
    }


    private Vector2 CalculateWorldPosition(int q, int r)
    {
        float x = this.transform.position.x + _hexWidth * (Mathf.Sqrt(3f) * q + Mathf.Sqrt(3f)/2 * r) * this.transform.lossyScale.x;
        float y = this.transform.position.y - _hexHeight * ((3f / 2f) * r) * this.transform.lossyScale.y;

        return new Vector2(x, y);
    }

    private void RecalculateWorldPosition()
    {
        foreach(Node n in _nodes.Values)
        {
            n.SetWorldInPosition(CalculateWorldPosition(n.GridPosition.x, n.GridPosition.y));
        }
    }

    public void SetGridWorldPosition(Vector3 worldPosition)
    {
        this.transform.position = worldPosition;
        RecalculateWorldPosition();
    }

    public Node GetNodeByWorldPosition(Vector3 worldPosition)
    {
        float x = worldPosition.x - this.transform.position.x;
        float y = worldPosition.y - this.transform.position.y;
        float scaledWidth = this.transform.lossyScale.x * _hexWidth;
        float scaledHeight = this.transform.lossyScale.y * _hexHeight;

        float q = Mathf.Sqrt(3f) / 3f * x / scaledWidth + (1f / 3f) * y  / scaledHeight;
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
        return _nodes[new Vector2Int(roundedQ, roundedR)];  
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



}
