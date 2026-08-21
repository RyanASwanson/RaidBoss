using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SBP_RayOfHope : BossProjectileFramework
{
    [SerializeField] private float _rayLifetime;
    [SerializeField] private float _rayLifetimeWarning;
    private WaitForSeconds _rayLifetimeWait;
    private WaitForSeconds _rayLifetimeWarningWait;
    
    [Space]
    [SerializeField] private float _baseHealing;

    [Space]
    [SerializeField] private float _rayDestructionRemovalMultiplier;
    [SerializeField] private GameObject _rayOfHopeDestructionVFX;

    [Space]
    [SerializeField] private GameObject _rayCrystal;
    
    [Space]
    [SerializeField] private GeneralBossBuffArea _bossBuffArea;
    [SerializeField] private MoveBetween _crystalMoveBetween;
    [SerializeField] private CurveProgression _appearCurve;
    [SerializeField] private CurveProgression _lifetimeWarningCurve;

    private bool _isRemovingRay = false;

    private void SetUpWaitForSeconds()
    {
        _rayLifetimeWarningWait = new WaitForSeconds(_rayLifetimeWarning);
        _rayLifetimeWait = new WaitForSeconds(_rayLifetime- _rayLifetimeWarning);
    }

    public void HitHero(HeroBase heroBase)
    {
        if (_isRemovingRay)
        {
            return;
        }
        
        // If we did not deal hex damage
        if (!SB_VoidLord.Instance.GetDespairHex().AttemptHexDamage(heroBase))
        {
            if (heroBase.GetHeroStats().IsHeroMaxHealth())
            {
                return;
            }
            
            /*
             * Potentially implement more clear solution on basing healing on amount of living Heroes
             * Healing has to be reduced as Heroes die, or with only 1 hero alive the healing would be all going into 1 Hero.
             */
            _bossBuffArea.DealHealing(heroBase, _baseHealing*HeroesManager.Instance.GetAmountOfLivingHeroes());
        }
        
        RemoveRayOfHope(heroBase);
    }

    private IEnumerator RayLifetime()
    {
        yield return _rayLifetimeWarningWait;
        
        _lifetimeWarningCurve.StartMovingUpOnCurve();
        
        yield return _rayLifetimeWait;
        
        RemoveRayOfHope(null);
    }
    
    public void RemoveRayOfHope(HeroBase heroBase)
    {
        if (_isRemovingRay)
        {
            return;
        }
        
        _isRemovingRay = true;
        _bossBuffArea.ToggleProjectileCollider(false);

        if (!heroBase.IsUnityNull())
        {
            _crystalMoveBetween.transform.SetParent(null);
            _crystalMoveBetween.StartMoveProcess(heroBase.gameObject);
        }
        
        UnsubscribeFromEvents();
        
        _appearCurve.StartMovingDownOnCurve();
    }

    public void RayOfHopeDestroyedByAbility()
    {
        PlayRayOfHopeDestroyedAudio();
        
        Instantiate(_rayOfHopeDestructionVFX, _rayCrystal.transform.position, Quaternion.identity);
        
        _rayCrystal.gameObject.SetActive(false);
        
        _appearCurve.SetCurveDecreaseTime(_appearCurve.GetCurveDecreaseTime()*_rayDestructionRemovalMultiplier);

        SB_VoidLord.Instance.SetHasRayOfHopeBeenDestroyed(true);
            
        RemoveRayOfHope(null);
    }

    public void DestroyRayOfHope()
    {
        Destroy(gameObject);
    }
    
    private void PlayRayOfHopeSpawnAudio()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
                BossAbilityAudio[_abilityID].GeneralAbilityAudio[SBA_FadingHope.RAY_OF_HOPE_SPAWN_AUDIO_ID]);
    }

    private void PlayRayOfHopeDestroyedAudio()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
                BossAbilityAudio[_abilityID].GeneralAbilityAudio[SBA_FadingHope.RAY_OF_HOPE_DESTROYED_AUDIO_ID]);
    }

    private void SubscribeToEvents()
    {
        _bossBuffArea.GetGeneralHitEvent().AddListener(HitHero);
    }

    private void UnsubscribeFromEvents()
    {
        _bossBuffArea.GetGeneralHitEvent().RemoveListener(HitHero);
    }
    
    #region BaseProjectile
    public override void SetUpProjectile(BossBase bossBase, int newAbilityID)
    {
        base.SetUpProjectile(bossBase, newAbilityID);
        SubscribeToEvents();
        SetUpWaitForSeconds();
        
        PlayRayOfHopeSpawnAudio();

        StartCoroutine(RayLifetime());
    }
    #endregion
}
