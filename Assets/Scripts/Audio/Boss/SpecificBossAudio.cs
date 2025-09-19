using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Holds the audio unique to a boss
/// </summary>
[System.Serializable]
public class SpecificBossAudio
{
    public string AssociatedBoss;

    public SpecificBossAbilityAudio[] BossAbilityAudio;
    
    public SpecificAudio[] MiscellaneousBossAudio;
}

[System.Serializable]
public class SpecificBossAbilityAudio
{
    public string AbilityName;

    public SpecificAudio AbilityPrep;
    public SpecificAudio AbilityStart;
    
    public SpecificAudio[] GeneralAbilityAudio;
}
