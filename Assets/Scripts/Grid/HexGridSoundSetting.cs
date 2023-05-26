using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu (fileName = "HexGridSoundSetting", menuName = "HexGridSoundSetting")]
public class HexGridSoundSetting : ScriptableObject
{
    public List<NodeSoundSetting> SoundSettings;
}

