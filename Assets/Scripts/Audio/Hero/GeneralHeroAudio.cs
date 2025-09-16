using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GeneralHeroAudio
{
    public GeneralHeroHealthAudio HealthAudio;
}

[System.Serializable]
public class GeneralHeroHealthAudio
{
    public SpecificAudio HeroTookDamage;
    
    public SpecificAudio HeroTookHealing;
}
