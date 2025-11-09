using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHP_VampirePassiveProjectile : HeroProjectileFramework
{
    
    [Space]
    [SerializeField] private MoveBetween _moveBetween;
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
        Debug.Log("Stored heal is" + _storedHeal);
        _trailRenderer.widthMultiplier = _curveScalar.FloatFromFloatCurve(_storedHeal * _vampireHealMultiplier);
    }
    
    public void AdditionalSetup(SH_Vampire heroScript, float healing, float vampireHealMultiplier)
    {
        _vampireHero = heroScript;
        _storedHeal = healing;
        _vampireHealMultiplier = vampireHealMultiplier;
        
        ScaleSize();
        _moveBetween.StartMoveProcess(_vampireHero.gameObject);
    }
}
