using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

/// <summary>
/// Holds the audio unique to a hero
/// </summary>
[System.Serializable]
public class SpecificHeroAudio
{
    public string AssociatedHero;
    
    public SpecificAudio SelectionSelectedAudio;
    
    public SpecificAudio BasicAbilityUsed;
    public SpecificAudio ManualAbilityUsed;
    public SpecificAudio PassiveAbilityUsed;
    
    public SpecificAudio[] MiscellaneousHeroAudio;
}
