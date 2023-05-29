using System;
using UnityEngine;


public enum NodeHighLightSource { NONE, PLAYER }

public class Node : MonoBehaviour, IEquatable<Node>
{
    private SpriteRenderer _renderer;
    public Vector2Int GridPosition;
    public Vector2 WorldPosition;
    private AudioClip _clip;

    private GameObject _playerObject;
    private GameObject _enemyObject;

    public float MovementCostFromStart;
    public float MovementCostToEnd;
    public float SeachPriority;
    public Node ParentNode;

    private Animator _animator;

    private Color _defaultColor;
    private Color _tempColor;
    private bool _animPlaying;


    public NodeHighLightSource HighLightSource { get; private set; }
    private void Awake()
    {
        _renderer = this.GetComponentInChildren<SpriteRenderer>();
        _animator = this.GetComponent<Animator>();
        HighLightSource = NodeHighLightSource.NONE;        
    }

    public void CleanMovementCosts()
    {
        MovementCostFromStart = 0;
        MovementCostToEnd = 0;
        SeachPriority = 0;
        ParentNode = null;
    }

    public void SetNode(Sprite sprite, Vector2Int gridPosition, Vector3 worldPosition)
    {
        _renderer.sprite = sprite;
        _renderer.sortingOrder = gridPosition.y;
        GridPosition = gridPosition;
        WorldPosition = worldPosition;
        _defaultColor = _renderer.color;
    }

    public bool IsOccupiedByPlayer()
    {
        return _playerObject != null;
    }

    public bool IsOccupiedByEnemy()
    {
        return _enemyObject != null;
    }

    public void SetPlayerObject(GameObject playerObject)
    {
        _playerObject = playerObject;
    }

    public void SetEnemyObject(GameObject enemyObject)
    {
        _enemyObject = enemyObject;
    }

    public bool IsOccupied()
    {
        return _enemyObject != null || _playerObject != null;
    }

    public void SetWorldInPosition(Vector2 worldPosition)
    {
        WorldPosition = worldPosition;

        if (_enemyObject != null)
        {
            _enemyObject.transform.position = worldPosition;
        }
    }

    public GameObject GetEnemyObject()
    {
        return _enemyObject;
    }

    public void SetHightLight(bool hightLight, float alpha = 1, NodeHighLightSource highLightSource = NodeHighLightSource.NONE)
    {
        this.gameObject.SetActive(hightLight);
        _renderer.color = new Color(_defaultColor.r, _defaultColor.g, _defaultColor.b, alpha);
        HighLightSource = highLightSource;
    }

    public void SetClip(AudioClip clip)
    {
        _clip = clip;
    }

    public AudioClip GetClip()
    {
        return _clip;
    }

    public bool Equals(Node other)
    {
        if (other == null)
        {
            return false;
        }

        return
                other.GridPosition == this.GridPosition;
    }

    public override bool Equals(System.Object obj)
    {
        if (obj == null)
            return false;

        Node node = obj as Node;
        if (node == null)
            return false;
        else
            return Equals(node);
    }

    public static bool operator ==(Node node1, Node node2)
    {
        if (((object)node1) == null || ((object)node2) == null)
            return Equals(node1, node2);

        return node1.Equals(node2);
    }

    public static bool operator !=(Node node1, Node node2)
    {
        if (((object)node1) == null || ((object)node2) == null)
            return !Equals(node1, node2);

        return !node1.Equals(node2);
    }

    public void CorrectHit()
    {
        if (!_animPlaying)
        {
            _animPlaying = true;
            _tempColor = _renderer.color;
            _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, 1);
            _animator.SetTrigger("CorrectHit");
        }
    }

    public void BadHit()
    {
        if (!_animPlaying)
        {
            _animPlaying = true;
            _tempColor = _renderer.color;
            _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, 1);
            _animator.SetTrigger("BadHit");

        }
    }

    public void AnimationEnded()
    {
        _animPlaying = false;
        _renderer.color = _tempColor;
    }


    public override int GetHashCode()
    {
        return GridPosition.GetHashCode();
    }

    public override string ToString()
    {
        return $"WorldPosition: {WorldPosition.ToString()}, GridPosition: {GridPosition.ToString()}";
    }
}
