using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHP_AstromancerBasicProjectile : HeroProjectileFramework
{
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _damageScalingPerSecond;

    [Space] 
    [Range(0,1)][SerializeField] private float _percentDistanceToReflectHero;
    [SerializeField] private float _reflectEffectSpawnHeight;
    [SerializeField] private GameObject _reflectEffect;

    [Space] 
    [SerializeField] private float _startingScale;
    [SerializeField] private float _damageScalingScaleMultiplier;
    [SerializeField] private GameObject _projectileScaleHolder;
    private Vector3 _projectileScalerVector = new Vector3();
    
    [Space]
    [SerializeField] private GeneralHeroDamageArea _generalDamageArea;
    [SerializeField] private GeneralHeroHealArea _generalHealArea;

    private bool _hasHitBoss;

    private SH_Astromancer _astromancerScript;

    private Vector3 _storedDirection;

    private float _currentStoredDamageScalingAmount = 0;
    private float _totalDamageScalingAmount = 0;

    private IEnumerator MoveProjectile()
    {
        while (true)
        {
            transform.position += _storedDirection * (_projectileSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator DamageScaling()
    {
        while(true)
        {
            _currentStoredDamageScalingAmount += _damageScalingPerSecond * Time.deltaTime;
            _totalDamageScalingAmount += _damageScalingPerSecond * Time.deltaTime;
            ScaleProjectileBasedOnDamageScaling();
            yield return null;
        }
    }

    private void ScaleProjectileBasedOnDamageScaling()
    {
        float projectileScaleAmount = _startingScale * ((_totalDamageScalingAmount * _damageScalingScaleMultiplier) + 1);
        _projectileScalerVector.Set(projectileScaleAmount,projectileScaleAmount,projectileScaleAmount);
        
        _projectileScaleHolder.transform.localScale = _projectileScalerVector;
    }

    private void AdjustDamageAreaScaling()
    {
        _generalDamageArea.IncreaseDamageMultiplierByAmount(_currentStoredDamageScalingAmount);
    }

    public void HitBoss(Collider collider)
    {
        AdjustDamageAreaScaling();
        if (!_hasHitBoss)
        {
            _currentStoredDamageScalingAmount = 0;
            
            _hasHitBoss = true;

            _generalDamageArea.enabled = false;
            _generalHealArea.enabled = true;

            FlipDirection();
        }
        else
        {
            _generalDamageArea.DestroyProjectile();
        }
            
    }
    
    /// <summary>
    /// Checks if the projectile made contact with the astromancer
    /// </summary>
    /// <param name="collider"></param>
    public void HitHero(Collider collider)
    {
        HeroBase storedSpecificHero = collider.gameObject.GetComponentInParent<HeroBase>();
        if (storedSpecificHero != _myHeroBase)
        {
            return;
        }

        _generalDamageArea.enabled = true;
        _generalHealArea.enabled = false;

        FlipDirection();
        PlayReflectAudio();
        CreateReflectEffect(collider.gameObject);
        TriggerHeroPassive();
    }

    /// <summary>
    /// Causes the astromancer projectile to turn around
    /// </summary>
    private void FlipDirection()
    {
        _storedDirection *= -1;
    }

    public void TriggerHeroPassive()
    {
        _astromancerScript.ActivatePassiveAbilities();
    }

    private void CreateReflectEffect(GameObject reflectHero)
    {
        GameObject reflectEffect = Instantiate(_reflectEffect, Vector3.Lerp(transform.position, reflectHero.transform.position,_percentDistanceToReflectHero), Quaternion.identity);
        reflectEffect.transform.LookAt(gameObject.transform);
        reflectEffect.transform.localEulerAngles = new Vector3(0, reflectEffect.transform.localEulerAngles.y, 0);
        //reflectEffect.transform.position = new Vector3(reflectEffect.transform.position.x,_reflectEffectSpawnHeight,reflectEffect.transform.position.z);
    }
    
    private void PlayReflectAudio()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificHeroAudio[_myHeroBase.GetHeroSO().GetHeroID()]
                .MiscellaneousHeroAudio[SH_Astromancer.BASIC_PROJECTILE_REFLECT_AUDIO_ID]);
    }


    #region Base Ability
    public override void SetUpProjectile(HeroBase heroBase, EHeroAbilityType heroAbilityType)
    {
        base.SetUpProjectile(heroBase, heroAbilityType);
        SubscribeToEvents();
    }

    public void AdditionalSetup(SH_Astromancer heroScript, Vector3 direction)
    {
        _astromancerScript = heroScript;

        _storedDirection = direction;
        StartCoroutine(MoveProjectile());
        StartCoroutine(DamageScaling());
    }

    private void SubscribeToEvents()
    {
        _generalDamageArea.GetEnterEvent().AddListener(HitBoss);
        _generalDamageArea.GetStayEvent().AddListener(HitBoss);
        _generalHealArea.GetEnterEvent().AddListener(HitHero);
        _generalHealArea.GetStayEvent().AddListener(HitHero);
    }
    #endregion
}
