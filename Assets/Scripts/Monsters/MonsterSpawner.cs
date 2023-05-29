using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterSpawner : MonoBehaviour
{

    [SerializeField]
    private HexGrid _grid;

    [SerializeField]
    private GameObject _monsterPrefab;

    [SerializeField]
    private GeneralEvent _monsterGenerated;

    [SerializeField]
    private GeneralEvent _entityPrepared;

    [SerializeField]
    private int[] _monsterPerLevel;

    public void OnEntityPrepared(EventArgs args)
    {
        EntityPreparedEventArgs entityPreparedEventArgs = (EntityPreparedEventArgs)args;

        if(entityPreparedEventArgs.EntityPrepareType == EntityPrepareType.GRID && entityPreparedEventArgs.Prepared)
        {
            StartCoroutine(GenerateMonsters());
        }
    }

    private IEnumerator GenerateMonsters()
    {
        int monstersGenerated = 0; 
        List<Node> nodes = _grid.GetNodeList();
        List<GameObject> monsters = new List<GameObject>();

        int currentLevel = _monsterPerLevel.Length;
        if (LevelManager.Instance.GetCurrentLevel() < _monsterPerLevel.Length)
        {
            currentLevel = LevelManager.Instance.GetCurrentLevel();
        }

        while (monstersGenerated < Mathf.Min(_monsterPerLevel[currentLevel], nodes.Count - 1))
        {
            int randomNodeIndex = Random.Range(0, nodes.Count);

            Node n = nodes[randomNodeIndex];

            if (!n.IsOccupied() && n.GridPosition != new Vector2Int(0,0))
            {
                GameObject enemyobject = Instantiate(_monsterPrefab, n.WorldPosition, Quaternion.identity, this.transform);
                MonsterMovement monsterMovement = enemyobject.GetComponent<MonsterMovement>();
                monsterMovement.SetCurrentNode(n);
                n.SetEnemyObject(enemyobject);
                monsters.Add(enemyobject);
                monstersGenerated++;
            }
            yield return null;
        }

        _monsterGenerated.Raise(new MonstersGeneratedEventArgs(monsters));
        _entityPrepared.Raise(new EntityPreparedEventArgs(EntityPrepareType.MONSTERS, true));
    }


}
