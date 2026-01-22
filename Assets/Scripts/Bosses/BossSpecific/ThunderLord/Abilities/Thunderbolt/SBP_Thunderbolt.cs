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
        
        _thunderboltActivationCoroutine = StartCoroutine(DelayActivation());
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
