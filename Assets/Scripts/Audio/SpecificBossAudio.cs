using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

/// <summary>
/// Holds the audio unique to a boss
/// </summary>
[System.Serializable]
public class SpecificBossAudio
{
    public EventReference BossMusic;
    
    public SpecificAudio[] MiscellaneousBossAudio;
}
