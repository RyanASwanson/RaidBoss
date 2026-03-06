using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using Unity.VisualScripting;
using UnityEngine;

public class SBP_FallingMeteor : BossProjectileFramework
{
    [SerializeField] private GameObject _contactParticles;
    
    [Space]
    [SerializeField] private Animator _meteorAnimator;
    private bool _hasMeteorBeenStopped = false;
    
    private const string REMOVE_PROJECTILE_ANIM_TRIGGER = "RemoveMeteor";
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
        _meteorAnimator.SetTrigger(REMOVE_PROJECTILE_ANIM_TRIGGER);

        AudioManager.Instance.StartFadeOutStopInstance(_fallingMeteorSFXInstance,
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
                BossAbilityAudio[_abilityID].GeneralAbilityAudio[0]);
    }

    private void PlayFallingMeteorSFX()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
                BossAbilityAudio[_abilityID].GeneralAbilityAudio[0], out _fallingMeteorSFXInstance);
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
