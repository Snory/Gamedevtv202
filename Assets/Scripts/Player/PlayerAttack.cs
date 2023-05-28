using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField]
    private GeneralEvent _requestForNode;

    private Camera _main;

    [SerializeField]
    private GeneralEvent ObjectDestroyed;

    [SerializeField]
    private Canvas _lilyPadCanvasUI;

    [SerializeField]
    private GameObject _scoreTextItemUI;

    private GameObject _instantiatedScoreItemUI;

    // Start is called before the first frame update
    void Start()
    {
        _main = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 position = _main.ScreenToWorldPoint(Input.mousePosition);

            _requestForNode.Raise(new HexGridNodeRequestEventArgs(this.transform.position, position, OnNodeFound));

        }
    }

    public void OnNodeFound(Node n, int range)
    {

        if (n == null || n.HighLightSource != NodeHighLightSource.PLAYER || range != 1)
        {
            return;
        }

        _instantiatedScoreItemUI = Instantiate(_scoreTextItemUI, _main.WorldToScreenPoint(n.WorldPosition), Quaternion.identity, _lilyPadCanvasUI.transform);

        if(!n.IsOccupiedByEnemy())
        {
            _instantiatedScoreItemUI.GetComponent<TextMeshProUGUI>().color = Color.red;
        } else
        {
            _instantiatedScoreItemUI.GetComponent<TextMeshProUGUI>().color = Color.green;

            GameObject enemyObject = n.GetEnemyObject();
            n.SetEnemyObject(null);
            ObjectDestroyed.Raise(new ObjectDestroyedEventArgs(enemyObject));
            Destroy(enemyObject);
        }




    }
}
