using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class GeneralBossAudio
{
    public GeneralBossHealthStaggerAudio HealthStaggerAudio;
    
    public GeneralBossAbilityAudio AbilityAudio;

    public GeneralBossEnrageAudio EnrageAudio;
}

[System.Serializable]
public class GeneralBossHealthStaggerAudio
{
    public SpecificAudio BossTookDamage;
    
    public SpecificAudio BossStaggered;

    public SpecificAudio GeneralBossDied;
}

[System.Serializable]
public class GeneralBossAbilityAudio
{
    public SpecificAudio TargetZoneSpawned;
}

[System.Serializable]
public class GeneralBossEnrageAudio
{
    public SpecificAudio BossEnrageImpending;
    public SpecificAudio BossEnrageStarted;
}