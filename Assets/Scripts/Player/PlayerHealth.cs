using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    public GeneralEvent _gameobjectDestroyed;

    [SerializeField]
    private GeneralEvent _gameOver;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Enemy")
        {
            _gameOver.Raise();
        }
    }
}
