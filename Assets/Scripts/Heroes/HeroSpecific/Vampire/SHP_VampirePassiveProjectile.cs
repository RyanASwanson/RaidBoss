using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHP_VampirePassiveProjectile : HeroProjectileFramework
{
    [SerializeField] private Vector3 _visualsMoveTargetPosition;
    
    [Space]
    [SerializeField] private MoveBetween _moveBetween;
    [SerializeField] private MoveBetween _visualsMoveBetween;
    [SerializeField] private FloatCurveScalar _curveScalar;
    [SerializeField] private TrailRenderer _trailRenderer;
    
    private float _storedHeal;
    private float _vampireHealMultiplier;
    
    private SH_Vampire _vampireHero;

    public void ActivateHeroPassive()
    {
        _vampireHero.ActivatePassiveHeal(_storedHeal);
    }

    private void ScaleSize()
    {
        _trailRenderer.widthMultiplier = _curveScalar.FloatFromFloatCurve(_storedHeal * _vampireHealMultiplier);
    }
    
    public void AdditionalSetup(SH_Vampire heroScript, float healing, float vampireHealMultiplier)
    {
        _vampireHero = heroScript;
        _storedHeal = healing;
        _vampireHealMultiplier = vampireHealMultiplier;
        
        ScaleSize();
        _moveBetween.StartMoveProcess(_vampireHero.gameObject);
        _visualsMoveBetween.StartMoveProcess(_visualsMoveTargetPosition);
    }
}
