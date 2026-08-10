using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_VoidMawProjectile : BossProjectileFramework
{
    //[SerializeField] private float _baseMoveTime;
    [SerializeField] private float _minimumMoveTime;
    [SerializeField] private float _maximumMoveTime;

    [Space]
    [SerializeField] private float _collisionEnableDistance;

    [Space]
    [SerializeField] private float _minimumHeroDistance;
    [SerializeField] private float _maximumHeroDistance;
    
    [Space]
    [SerializeField] private AnimationCurve _moveCurve;
    private float _moveTime;
    
    private Vector3 _startLocation;
    private Vector3 _midPointLocation = new();
    private Vector3 _endLocation;
    
    private int _movementID = 0;
    private Vector3 _movementStartLocation;

    [Space] 
    [SerializeField] private GeneralBossDamageArea _damageArea;
    [SerializeField] private CurveProgression _scaleCurve;
    [SerializeField] private GeneralVFXFunctionality _voidMawVFX;

    private SBP_VoidMaw _associatedMawOwner;
    
    public void AdditionalSetUp(SBP_VoidMaw maw, int mawID, Vector3 startLocation, float heroDistance)
    {
        _associatedMawOwner = maw;
        _startLocation = startLocation;
        
        _midPointLocation.Set(_startLocation.z,_startLocation.y,_startLocation.x);
        _midPointLocation *= mawID > 0 ? -1 : 1;
        
        _endLocation = -startLocation;
        
        //_moveTime = Mathf.Clamp( _baseMoveTime * heroDistance,_minimumMoveTime,float.MaxValue);
        _moveTime = Mathf.Lerp(_minimumMoveTime,_maximumMoveTime,(heroDistance - _minimumHeroDistance) / (_maximumHeroDistance - _minimumHeroDistance));

        UseNextVoidMawMovement();
    }

    private void UseNextVoidMawMovement()
    {
        switch (_movementID)
        {
            case 0:
                StartMovingVoidMaw(_startLocation,_midPointLocation);
                return;
            case 1:
                StartMovingVoidMaw(_midPointLocation,_endLocation);
                return;
            default:
                _voidMawVFX.SetLoopOfParticleSystems(false);
                _voidMawVFX.DetachVisualEffect();
                _voidMawVFX.StartDelayedLifetime();
                _associatedMawOwner.MawReachedEnd();
                return;
        }
        
    }

    public void StartedScalingUp()
    {
        
    }

    public void StartedScalingDown()
    {
        if (_movementID == 2)
        {
            _associatedMawOwner.FinalScalingDownVoidMaw();
        }
    }
    
    private void StartMovingVoidMaw(Vector3 startLocation, Vector3 endLocation)
    {
        _movementID++;
        
        transform.LookAt(transform.position + (startLocation - endLocation));
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
        
        //_scaleCurve.SetDecreaseDelay(_moveTime - (_scaleCurve.GetCurveIncreaseTime() + _scaleCurve.GetCurveDecreaseTime()));
        _scaleCurve.StartMovingUpOnCurve();
        
        StartCoroutine(MoveVoidMaw(startLocation,endLocation));
    }

    private IEnumerator MoveVoidMaw(Vector3 startLocation, Vector3 endLocation)
    {
        float progress = 0;
        bool isCollisionActive = false;
        
        while (progress < 1)
        {
            progress += Time.deltaTime / _moveTime;
            transform.localPosition = Vector3.Lerp(startLocation, endLocation, _moveCurve.Evaluate(progress));
            
            _voidMawVFX.SetEmissionRateMultiplierWithCurve(progress);
            
            if (Vector3.Distance(transform.localPosition, startLocation) <= _collisionEnableDistance)
            {
                if (!isCollisionActive)
                {
                    _damageArea.ToggleProjectileCollider(false);
                    isCollisionActive = true;
                }
            }
            else if (Vector3.Distance(transform.localPosition, endLocation) <= _collisionEnableDistance)
            {
                if (isCollisionActive)
                {
                    _damageArea.ToggleProjectileCollider(false);
                    _scaleCurve.StartMovingDownOnCurve();
                    //_voidMawVFX.SetLoopOfParticleSystems(false);
                    isCollisionActive = false;
                }
            }
            else
            {
                _damageArea.ToggleProjectileCollider(true);
            }
            
            yield return null;
        }
        transform.position = endLocation;
        UseNextVoidMawMovement();
    }
    
    #region Base Ability
    public override void SetUpProjectile(BossBase bossBase, int newAbilityID)
    {
        base.SetUpProjectile(bossBase, newAbilityID);
        
    }
    #endregion
}
