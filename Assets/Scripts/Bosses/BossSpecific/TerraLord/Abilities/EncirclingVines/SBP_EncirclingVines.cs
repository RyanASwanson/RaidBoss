using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

public class SBP_EncirclingVines : BossProjectileFramework
{
    [SerializeField] private float _scaleDuration;
    [SerializeField] private float _scaleDownRemovalDelay;
    [SerializeField] private float _enrageScaleDownDelayIncrease;

    [Space] 
    [SerializeField] private float _endSFXEarlyPlayTime;

    [Space] 
    [SerializeField] private Vector3 _scaleChangeRotation;
    [SerializeField] private AnimationCurve _scaleChangeCurve;
    
    [Space]
    [SerializeField] private FollowObject _followObject;

    [SerializeField] private GeneralBossDamageArea _damageArea;

    [Space] 
    [SerializeField] private CurveProgression _scaleCurve;

    private EventInstance _encirclingVinesLoopInstance;

    public void AdditionalSetUp(GameObject followTarget)
    {
        _followObject.StartFollowingObject(followTarget);

        AdditionalSetUp();
    }

    public void AdditionalSetUp()
    {
        _scaleCurve.SetCurveIncreaseTime(_scaleDuration);
        _scaleCurve.SetCurveDecreaseTime(_scaleDuration);

        if (_wasBossEnragedOnAbilityActivation)
        {
            _scaleDownRemovalDelay += _enrageScaleDownDelayIncrease;
        }
        _scaleCurve.SetDecreaseDelay(_scaleDownRemovalDelay);
        
        _scaleCurve.StartMovingUpOnCurve();

        _damageArea.SetProjectileColliderActivationDelay(_scaleDuration);
        _damageArea.SetProjectileColliderLifeTime(_scaleDuration+_scaleDownRemovalDelay);
        
        _damageArea.StartColliderActivationDelay();
        _damageArea.StartColliderLifetime();

        StartScaleChangeRotationProcess();

        PlayEncirclingVinesLoopSFX();
        StartCoroutine(DelayEncirclingVinesEndSFX());
    }
    
    public void EncirclingVinesDownScaleBegun()
    {
        StopEncirclingVinesLoopSFX();
        StartScaleChangeRotationProcess();
    }
    
    private void PlayEncirclingVinesLoopSFX()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
                BossAbilityAudio[_abilityID].GeneralAbilityAudio[SBA_EncirclingVines.ENCIRCLING_VINES_LOOP_AUDIO_ID], out _encirclingVinesLoopInstance);
    }

    private void StopEncirclingVinesLoopSFX()
    {
        //AudioManager.Instance.StopSpecificAudioInstance(_encirclingVinesLoopInstance, true);
        AudioManager.Instance.StartFadeOutStopInstance(_encirclingVinesLoopInstance,
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
            BossAbilityAudio[_abilityID].GeneralAbilityAudio[SBA_EncirclingVines.ENCIRCLING_VINES_LOOP_AUDIO_ID]);
    }
    
    private IEnumerator DelayEncirclingVinesEndSFX()
    {
        yield return new WaitForSeconds((_scaleDuration + _scaleDownRemovalDelay) - _endSFXEarlyPlayTime);
        PlayEncirclingVinesEndSFX();
    }
    
    private void PlayEncirclingVinesEndSFX()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
                BossAbilityAudio[_abilityID].GeneralAbilityAudio[SBA_EncirclingVines.ENCIRCLING_VINES_END_AUDIO_ID]);
    }
    
    private void StartScaleChangeRotationProcess()
    {
        StartCoroutine(ScaleChangeRotationProcess());
    }
    
    private IEnumerator ScaleChangeRotationProcess()
    {
        float counter = 0;
        Vector3 startingEulerAngles = transform.localEulerAngles;
        Vector3 targetEulerAngles = transform.localEulerAngles + _scaleChangeRotation;

        while (counter < 1)
        {
            counter += Time.deltaTime / _scaleDuration;
            
            transform.localEulerAngles =
                Vector3.Lerp(startingEulerAngles, targetEulerAngles, _scaleChangeCurve.Evaluate(counter));
            yield return null;
        }
        
        transform.localEulerAngles = targetEulerAngles;
    }
}
