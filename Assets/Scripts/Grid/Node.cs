using UnityEngine;


public enum NodeHighLightSource { NONE, PLAYER }

public class Node 
{
    private SpriteRenderer _renderer;
    private GameObject _gameObject;
    public Vector2Int GridPosition;
    public Vector2 WorldPosition;
    private int _sortingOrder;

    public NodeHighLightSource HighLightSource { get; private set; }


    public Node(Vector2Int gridPosition, Vector2 worldPosition, int sortingOrder = 0)
    {
        GridPosition = gridPosition;
        WorldPosition = worldPosition;
        _sortingOrder = sortingOrder;
        HighLightSource = NodeHighLightSource.NONE;

    }

    public void SetGameObject(GameObject gameObject)
    {
        _gameObject = gameObject;
        _renderer = _gameObject.GetComponent<SpriteRenderer>();
        _renderer.sortingOrder = _sortingOrder;
    }

    public GameObject GetGameObject()
    {
        return _gameObject;
    }

    public void SetWorldInPosition(Vector2 worldPosition)
    {
        WorldPosition = worldPosition;
        if(_gameObject != null)
        {
            _gameObject.transform.position = worldPosition;
        }
    }

    public void SetHightLight(bool hightLight, float alpha = 1, NodeHighLightSource highLightSource = NodeHighLightSource.NONE)
    {
        _gameObject.SetActive(hightLight);
        _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, alpha);
        HighLightSource = highLightSource;
    }
}
