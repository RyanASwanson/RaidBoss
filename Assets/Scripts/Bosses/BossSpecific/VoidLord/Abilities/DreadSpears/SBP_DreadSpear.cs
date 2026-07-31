using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SBP_DreadSpear : BossProjectileFramework
{
    [SerializeField] private float _dreadSpearDuration;
    [SerializeField] private float _enrageSpearDurationIncrease;
    [SerializeField] private float _spearImpactDecalDurationOffset;
    private WaitForSeconds _dreadSpearWait;

    [Space]
    [SerializeField] private Animator _spearAnimator;
    private const string DREAD_SPEAR_DURATION_ANIM_TRIGGER = "DreadSpearDurationOver";

    [Space] 
    [SerializeField] private Transform _vfxSpawnPoint;
    [SerializeField] private GameObject _spearVFX;
    [SerializeField] private CurveProgression _impactDecalCurve;
    private GeneralVFXFunctionality _spawnedVFX;
    
    private void StartDreadSpearDuration()
    {
        _dreadSpearWait = new WaitForSeconds(_dreadSpearDuration);
        
        StartCoroutine(DreadSpearDuration());
        Destroy(gameObject,_dreadSpearDuration+1);
    }

    private IEnumerator DreadSpearDuration()
    {
        yield return _dreadSpearWait;
        _spawnedVFX.SetLoopOfParticleSystems(false);
        _spearAnimator.SetTrigger(DREAD_SPEAR_DURATION_ANIM_TRIGGER);
        
    }

    private void CreateSpearVFX()
    {
        _spawnedVFX = Instantiate(_spearVFX, _vfxSpawnPoint.transform).GetComponent<GeneralVFXFunctionality>();
        StartCoroutine(VFXProcess());
    }

    private IEnumerator VFXProcess()
    {
        float vfxProgress = 0;
        while (vfxProgress < 1)
        {
            _spawnedVFX.SetEmissionRateMultiplierWithCurve(vfxProgress);
            yield return null;
        }
    }
    
    #region Base Ability
    public override void SetUpProjectile(BossBase bossBase, int newAbilityID, bool wasEnragedOnAbilityActivation)
    {
        base.SetUpProjectile(bossBase, newAbilityID, wasEnragedOnAbilityActivation);

        if (_wasBossEnragedOnAbilityActivation)
        {
            _dreadSpearDuration += _enrageSpearDurationIncrease;
        }
        _impactDecalCurve.SetCurveDecreaseTime(_dreadSpearDuration + _spearImpactDecalDurationOffset);

        CreateSpearVFX();
        
        StartDreadSpearDuration();
    }
    #endregion
}
