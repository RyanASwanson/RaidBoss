using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.Serialization;

public class SBP_IcicleRain : BossProjectileFramework
{
    [SerializeField] private float _iciclesSeperationTime;
    private WaitForSeconds _icicleSeperationWait;
    
    [SerializeField] private int _amountOfIcicleSets;
    [SerializeField] private int _enrageIcicleSetIncrease;
    
    [SerializeField] private SBP_IcicleSet[] _icicleSets;
    
    [Space] 
    [SerializeField] private float _impactAudioDelay;
    private WaitForSeconds _impactAudioWait;
    [Space] 
    [SerializeField] private float _impactAudioPitchIncrease;

    private IEnumerator SpikeSpawningProcess()
    {
        for (int i = 0; i <= _amountOfIcicleSets - 1; i++)
        {
            _icicleSets[i].SetUpProjectile(_myBossBase,_abilityID);
            if (_wasBossEnragedOnAbilityActivation)
            {
                _icicleSets[i+_enrageIcicleSetIncrease].SetUpProjectile(_myBossBase,_abilityID);
            }
            
            StartCoroutine(PlayIcicleImpactSfx(i));
            yield return _icicleSeperationWait;
        }
    }

    private IEnumerator PlayIcicleImpactSfx(int icicleSet)
    {
        yield return _impactAudioWait;
        
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
                BossAbilityAudio[_abilityID].GeneralAbilityAudio[SBA_IcicleRain.ICICLE_RAIN_IMPACT_AUDIO_ID], out EventInstance eventInstance);
        
        eventInstance.getPitch(out float pitch);
        eventInstance.setPitch(pitch + (_impactAudioPitchIncrease * icicleSet));
    }

    #region Base Ability
    public override void SetUpProjectile(BossBase bossBase, int newAbilityID)
    {
        base.SetUpProjectile(bossBase, newAbilityID);
        _icicleSeperationWait = new WaitForSeconds(_iciclesSeperationTime);
        _impactAudioWait = new WaitForSeconds(_impactAudioDelay);
        StartCoroutine(SpikeSpawningProcess());
    }
    #endregion
}
