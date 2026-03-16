using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_UnstablePrecipice : BossProjectileFramework
{
    public void FloorImpact()
    {
        PlayImpactSFX();
    }

    private void PlayImpactSFX()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
                BossAbilityAudio[_abilityID].GeneralAbilityAudio[SBA_UnstablePrecipice.UNSTABLE_PRECIPICE_PROJECTILE_IMPACT_AUDIO_ID]);
    }
    
}
