using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_CrystalBarrage : BossProjectileFramework
{
    [SerializeField] private float _crystalPersistantTime;
    [SerializeField] private float _enrageCrystalPersistantTimeIncrease;
    
    [Space]
    [SerializeField] private GeneralBossDamageArea _damageArea; 
    
    public void ProjectileGroundImpact()
    {
        _damageArea.StartColliderLifetime();
        PlayGroundImpactSFX();
    }

    private void PlayGroundImpactSFX()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
                BossAbilityAudio[_abilityID].GeneralAbilityAudio[SBA_CrystalBarrage.CRYSTAL_BARRAGE_IMPACT_AUDIO_ID]);
    }
    
    #region Base Projectile

    public override void SetUpProjectile(BossBase bossBase, int newAbilityID, bool wasEnragedOnAbilityActivation)
    {
        base.SetUpProjectile(bossBase, newAbilityID, wasEnragedOnAbilityActivation);
        
        _damageArea.SetProjectileColliderLifeTime(wasEnragedOnAbilityActivation
            ? _crystalPersistantTime + _enrageCrystalPersistantTimeIncrease
            : _crystalPersistantTime);
        
    }

    #endregion
    
}
