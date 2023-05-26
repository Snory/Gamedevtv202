using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridSound : MonoBehaviour
{

    [SerializeField]
    private AudioSource _testAudioSource;

    public void OnGridNodeSteppedOnto(EventArgs args)
    {
        HexGridNodeEventArgs hexGridNodeEventArgs = args as HexGridNodeEventArgs;

        AudioClip clip = hexGridNodeEventArgs.Node.GetClip();

        if (clip != null)
        {
            _testAudioSource.PlayOneShot(hexGridNodeEventArgs.Node.GetClip());
        }
    }
}
