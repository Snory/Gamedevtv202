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
        HexGridSteppedOntoNodeEventArgs hexGridNodeEventArgs = args as HexGridSteppedOntoNodeEventArgs;

        AudioClip clip = hexGridNodeEventArgs.Node.GetClip();

        if (clip != null)
        {
            _testAudioSource.PlayOneShot(hexGridNodeEventArgs.Node.GetClip());
        }
    }
}
