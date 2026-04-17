using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GeneralHeroAudio
{
    public GeneralHeroHealthAudio HealthAudio;
    
    public GeneralHeroInteractionAudio InteractionAudio;
}

[System.Serializable]
public class GeneralHeroHealthAudio
{
    public SpecificAudio HeroTookDamage;
    
    public SpecificAudio HeroTookHealing;

    public SpecificAudio HeroDied;

    public SpecificAudio LastHeroDied;
}

[System.Serializable]
public class GeneralHeroInteractionAudio
{
    public SpecificAudio HeroControlled;
}