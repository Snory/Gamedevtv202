using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LiliypadManager : MonoBehaviour
{
    [SerializeField]
    private HexGridSoundSetting _setting;

    [SerializeField]
    private AudioSource _source;


    public void OnButton(int range)
    {
        _source.PlayOneShot(_setting.SoundSettings.Where(st => st.Range == range).FirstOrDefault().Sound);
    }

}
