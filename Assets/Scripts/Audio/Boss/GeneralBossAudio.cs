using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class GeneralBossAudio
{
    public GeneralBossHealthStaggerAudio HealthStaggerAudio;
}

[System.Serializable]
public class GeneralBossHealthStaggerAudio
{
    public SpecificAudio BossTookDamage;
    
    public SpecificAudio BossStaggered;
}