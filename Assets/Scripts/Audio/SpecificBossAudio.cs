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
    public string AssociatedBoss;
    
    public SpecificAudio FirstAbilityUsed;
    
    public SpecificAudio SecondAbilityUsed;
    
    public SpecificAudio ThirdAbilityUsed;
    
    public SpecificAudio FourthAbilityUsed;

    public SpecificAudio FifthAbilityUsed;
    
    public SpecificAudio[] MiscellaneousBossAudio;
}
