using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using Unity.VisualScripting;
using UnityEngine;

public class SBP_FallingMeteor : BossProjectileFramework
{
    [SerializeField] private GeneralVFXFunctionality _fallingParticles;
    [SerializeField] private GameObject _contactParticles;

    [Space] 
    [SerializeField] private CurveProgression _removalCurve;
    
    [Space]
    [SerializeField] private Animator _meteorAnimator;
    private bool _hasMeteorBeenStopped = false;
    
    private EventInstance _fallingMeteorSFXInstance;
    
    private IEnumerator LookAtTarget(GameObject target)
    {
        Vector3 targetLocation = target.transform.position;
        while (true)
        {
            if (!target.IsUnityNull())
            {
                targetLocation = target.transform.position;
            }
            
            transform.LookAt(targetLocation);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            yield return null;
        }
    }

    public void FloorContact()
    {
        Instantiate(_contactParticles, transform.position, Quaternion.identity);
    }

    public void StopFallingMeteor()
    {
        _hasMeteorBeenStopped = true;
        _removalCurve.StartMovingUpOnCurve();

        AudioManager.Instance.StartFadeOutStopInstance(_fallingMeteorSFXInstance,
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
                BossAbilityAudio[_abilityID].GeneralAbilityAudio[SBA_Meteor.METEOR_PROJECTILE_IMPACT_AUDIO_ID]);
    }

    public void MeteorStopped()
    {
        if (_fallingParticles.IsUnityNull())
        {
            return;
        }
        
        _fallingParticles.StopAllParticleSystems();
        _fallingParticles.DetachVisualEffect();
    }

    private void PlayFallingMeteorSFX()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
                BossAbilityAudio[_abilityID].GeneralAbilityAudio[SBA_Meteor.METEOR_PROJECTILE_IMPACT_AUDIO_ID], out _fallingMeteorSFXInstance);
    }

    #region Base Ability

    public void AdditionalSetUp(GameObject target)
    {
        PlayFallingMeteorSFX();
        StartCoroutine(LookAtTarget(target));
    }
    #endregion
    
    #region Getters
    public bool GetHasMeteorBeenStopped() =>_hasMeteorBeenStopped;
    #endregion
}
