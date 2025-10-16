using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class GeneralBossAudio
{
    public GeneralBossHealthStaggerAudio HealthStaggerAudio;
    
    public GeneralBossAbilityAudio AbilityAudio;
}

[System.Serializable]
public class GeneralBossHealthStaggerAudio
{
    public SpecificAudio BossTookDamage;
    
    public SpecificAudio BossStaggered;
}

[System.Serializable]
public class GeneralBossAbilityAudio
{
    public SpecificAudio TargetZoneSpawned;
}