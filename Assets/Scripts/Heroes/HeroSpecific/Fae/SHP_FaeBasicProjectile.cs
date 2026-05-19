using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Contains the functionality for the basic projectile of the Fae.
/// SHP stands for Specific Hero Projectile.
/// </summary>
public class SHP_FaeBasicProjectile : HeroProjectileFramework
{
    [SerializeField] private bool _doesProjectileBounceOffWalls;
    [SerializeField] private float _wallBounceSpeedMultiplier = 1;

    [Space]
    [SerializeField] private GameObject _wallBounceEffect;
    
    [Space]
    [SerializeField] private GeneralTranslate _generalTranslate;
    [SerializeField] private GeneralHeroDamageArea _generalDamageArea;
    [SerializeField] private CurveProgression _scaleCurve;
    [SerializeField] private SwapTextures _swapTextures;
    
    private static SH_Fae _associatedFae;
    private bool _hasHitEdgeOfMap = false;

    private IEnumerator CheckForHitEdgeOfMap()
    {
        while (!_hasHitEdgeOfMap)
        {
            float centerDistance = Mathf.Abs(transform.position.x) + Mathf.Abs(transform.position.z);
            if (centerDistance > EnvironmentManager.Instance.GetMapRadius())
            {
                HitEdgeOfMap();
            }
            yield return null;
        }
    }

    private void HitEdgeOfMap()
    {
        _hasHitEdgeOfMap = true;
        
        _scaleCurve.StartMovingDownOnCurve();
    }

    public void RedirectProjectile()
    {
        Instantiate(_wallBounceEffect, transform.position, transform.rotation);
        
        Vector3 startDirection = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 newDirection = new Vector3(_associatedFae._myHeroBase.transform.position.x, 0, _associatedFae._myHeroBase.transform.position.z);
        
        transform.LookAt(newDirection);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
        
        newDirection -= startDirection;
        newDirection.Normalize();
        
        _generalTranslate.StopMoving();
        _generalTranslate.MultiplySpeed(_wallBounceSpeedMultiplier);
        _generalTranslate.StartMoving(newDirection);
        
        _swapTextures.SwapAllTextures();
        
        _generalDamageArea.ToggleProjectileCollider(true);
    }
    
    #region Base Ability
    public override void SetUpProjectile(HeroBase heroBase, EHeroAbilityType heroAbilityType)
    {
        base.SetUpProjectile(heroBase, heroAbilityType);

        if (_associatedFae.IsUnityNull())
        {
            _associatedFae = (SH_Fae)heroBase.GetSpecificHeroScript();
        }
        
        _generalTranslate.StartMoving(transform.forward);

        if (_doesProjectileBounceOffWalls)
        {
            StartCoroutine(CheckForHitEdgeOfMap());
        }
    }
    #endregion
}
