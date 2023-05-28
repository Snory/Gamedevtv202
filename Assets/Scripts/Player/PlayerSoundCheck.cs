using UnityEngine;

public class PlayerSoundCheck : MonoBehaviour
{
    [SerializeField]
    private GeneralEvent _requestForNode;

    [SerializeField] 
    private GeneralEvent _nodeSteppedOnto;

    private Camera _main;
    // Start is called before the first frame update
    void Start()
    {
        _main = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        {
            if (Input.GetMouseButtonDown(1))
            {
                Vector3 position = _main.ScreenToWorldPoint(Input.mousePosition);

                _requestForNode.Raise(new HexGridNodeRequestEventArgs(position, OnNodeFound));
            }
        }
    }

    public void OnNodeFound(Node n, int range)
    {

        if (n == null || n.HighLightSource != NodeHighLightSource.PLAYER)
        {
            return;
        }

        _nodeSteppedOnto.Raise(new HexGridSteppedOntoNodeEventArgs(n));

    }
}
