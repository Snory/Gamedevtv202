using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


public class PlayerAttack : MonoBehaviour
{
    [SerializeField]
    private GeneralEvent _requestForNode;

    [SerializeField]
    private GeneralEvent _requestCircleSpell;

    private Camera _main;

    [SerializeField]
    private GeneralEvent ObjectDestroyed;

    [SerializeField]
    private Canvas _lilyPadCanvasUI;

    [SerializeField]
    private GeneralEvent _scoreAdded;

    private bool _enabled;

    [SerializeField]
    private int _ringRange;

    private int _currentRange;

    private bool _circleSpell = false;

    [SerializeField]
    private HexGridSoundSetting _soundSettings;

    [SerializeField]
    private AudioSource _source;

    // Start is called before the first frame update
    void Start()
    {
        _main = Camera.main;
        _currentRange = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && _enabled)
        {
            Ray ray = _main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hitInWorldUI = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, LayerMask.GetMask("InWorldUI"));

            if (!hitInWorldUI)
            {
                Vector3 position = _main.ScreenToWorldPoint(Input.mousePosition);
                _requestForNode.Raise(new HexGridNodeRequestEventArgs(this.transform.position, position, OnNodeFound));
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && _enabled)
        {
            Debug.Log("Start circle spell: " + _circleSpell + " with a range: " + _currentRange);
            if (!_circleSpell)
            {
                BeginCircleSpell();
                CircleSpell();

            }
        }
    }



    private void CircleSpell()
    {
        if (_circleSpell)
        {
            _requestCircleSpell.Raise(new HexGridCircleNodeRequestEventArgs(this.transform.position, _currentRange, OnRingReturned));
        }
    }

    public void OnRingReturned(List<Node> ring, int range)
    {

        if(ring.Count == 0) //mimo rozsah mapy, konec
        {
            EndCircleSpell();
            return;
        }


        //zkontrolovat, zda je to occupied, jestli jo, tak zahrát ton a vložit pauzu
        if (ring.Where(n => n.IsOccupiedByEnemy()).Any())
        {
            StartCoroutine(SingASong(range));
        } else
        {
            CheckIfRingPowerContinues(range);
        }
    }

    private void EndCircleSpell()
    {
        _circleSpell = false;
        _currentRange = 1;
    }

    private void BeginCircleSpell()
    {
        _circleSpell = true;
        _currentRange = 1;
    }

    public IEnumerator SingASong(int range)
    {

        AudioClip audio =  _soundSettings.SoundSettings.Where(st => st.Range == range).FirstOrDefault().Sound;
        _source.PlayOneShot(audio);

        yield return new WaitForSeconds(1);

        CheckIfRingPowerContinues(range);
    }

    private void CheckIfRingPowerContinues(int range)
    {
        if (range < _ringRange)
        {
            _currentRange++;
            CircleSpell();
        }
        else
        {
            EndCircleSpell();
        }
    }



    public void OnGameSessionStateStart(EventArgs args)
    {
        GameSessionStateEventArgs gameSessionStateEventArgs = (GameSessionStateEventArgs)args;

        if (gameSessionStateEventArgs.State == GameSessionState.PLAYER_MOVEMENT)
        {
            _enabled = true;
        } else
        {
            _enabled = false;
        }
        
        EndCircleSpell();
    }

    public void OnNodeFound(Node n, int range)
    {

        if (n == null || n.HighLightSource != NodeHighLightSource.PLAYER || range != 1)
        {
            return;
        }


        if (!n.IsOccupiedByEnemy())
        {
            n.BadHit();
            _scoreAdded.Raise(new ScoreChangedEventArgs(-50));
        }
        else
        {
            n.CorrectHit();
            GameObject enemyObject = n.GetEnemyObject();
            n.SetEnemyObject(null);
            ObjectDestroyed.Raise(new ObjectDestroyedEventArgs(enemyObject));
            Destroy(enemyObject);
            _scoreAdded.Raise(new ScoreChangedEventArgs(100));
        }
    }
}
