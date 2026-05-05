using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class SBA_IcicleRain : SpecificBossAbilityFramework
{
    [SerializeField] private Vector3 _enrageUpwardsEffectVisualsPositionOffset;
    [SerializeField] private Vector3 _enrageUpwardsEffectVisualsRotation;
    
    [Space]
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _icicleRain;
    [SerializeField] private GameObject _icicleRainUpwardsVisual;
    
    private BossTargetZoneParent _newestTargetZone;

    public const int ICICLE_RAIN_IMPACT_AUDIO_ID = 0;
    
    #region Base Ability
    protected override void AbilityPrep()
    {
        Instantiate(_icicleRainUpwardsVisual, transform.position, Quaternion.identity);

        if (_wasBossEnragedOnAbilityActivation)
        {
            GameObject upwardsVisual = Instantiate(_icicleRainUpwardsVisual, transform.position, Quaternion.identity);
            upwardsVisual.transform.position += _enrageUpwardsEffectVisualsPositionOffset;
            upwardsVisual.transform.localEulerAngles += _enrageUpwardsEffectVisualsRotation;
        }
        base.AbilityPrep();
    }
    
    
    protected override void StartShowTargetZone()
    {
        //Spawns the target area
        _newestTargetZone = Instantiate(_targetZone, _storedTargetLocation, Quaternion.identity).GetComponent<BossTargetZoneParent>();
        //Adds the target area to the list of target areas
        _currentTargetZones.Add(_newestTargetZone);

        //Makes the target area follow the hero that is being targetted
        //StartCoroutine(FollowHeroTarget(_newestTargetZone.gameObject));

        if (_newestTargetZone.TryGetComponent(out FollowObject followObject))
        {
            followObject.StartFollowingObject(_storedTarget.transform.gameObject);
            followObject.StopFollowingDelayed(4);
        }

        base.StartShowTargetZone();
    }

    protected override void AbilityStart()
    {
        //Spawns the damaging ability
        GameObject newestIcicleRain = Instantiate(_icicleRain, _newestTargetZone.transform.position, Quaternion.identity);

        SBP_IcicleRain icicleFunc = newestIcicleRain.GetComponent<SBP_IcicleRain>();
        icicleFunc.SetUpProjectile(_myBossBase, _abilityID, _wasBossEnragedOnAbilityActivation);
        
        base.AbilityStart();
    }
    #endregion
}
