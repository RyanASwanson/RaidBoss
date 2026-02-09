using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SBP_Thunderbolt : BossProjectileFramework
{
    [SerializeField] private float _activationDelay;
    private WaitForSeconds _activationWait;
    
    private Coroutine _thunderboltActivationCoroutine;
    
    [Space]
    [SerializeField] private GeneralVFXFunctionality _cloudVFXFunctionality;
    [SerializeField] private GeneralVFXFunctionality _sparkImpactVFXFunctionality;
    [SerializeField] private GeneralVFXFunctionality _sparkImpactUpwardsVFXFunctionality;
    [SerializeField] private CustomCanvasDecal _impactDecal;
    
    [SerializeField] private GeneralVFXFunctionality[] _impactVFXFunctionality;
    
    [Space]
    [SerializeField] private GeneralBossDamageArea _damageZoneArea;

    [SerializeField] private CurveProgression _scaleCurve;
    

    public override void SetUpProjectile(BossBase bossBase, int newAbilityID)
    {
        base.SetUpProjectile(bossBase, newAbilityID);
        
        _activationWait = new WaitForSeconds(_activationDelay);

    }

    public void StartActivation()
    {
        if (!_thunderboltActivationCoroutine.IsUnityNull())
        {
            StopCoroutine(_thunderboltActivationCoroutine);
        }

        if (_activationDelay > 0)
        {
            _thunderboltActivationCoroutine = StartCoroutine(DelayActivation());
        }
        else
        {
            ActivateThunderbolt();
        }
        
    }

    private IEnumerator DelayActivation()
    {
        yield return _activationWait;
        ActivateThunderbolt();
    }
    
    private void ActivateThunderbolt()
    {
        _damageZoneArea.StartColliderActivationDelay();
        _damageZoneArea.StartColliderLifetime();
        _cloudVFXFunctionality.SetLoopOfParticleSystems(false);
        
        /*_sparkImpactVFXFunctionality.PlayAllParticleSystems();
        _sparkImpactUpwardsVFXFunctionality.PlayAllParticleSystems();*/

        foreach (GeneralVFXFunctionality vfx in _impactVFXFunctionality)
        {
            vfx.PlayAllParticleSystems();
        }
        
        _impactDecal.ActivateCustomDecal();
        
        _scaleCurve.StartMovingUpOnCurve();
    }

    public void CancelThunderboltActivation()
    {
        _cloudVFXFunctionality.SetLoopOfParticleSystems(false);
        if (!_thunderboltActivationCoroutine.IsUnityNull())
        {
            StopCoroutine(_thunderboltActivationCoroutine);
        }
    }
    
}
