using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_StaticCharge : BossProjectileFramework
{
    [SerializeField] private float _damageToFollowTargetOnSwap;
    [SerializeField] private int _maxSwaps;
    private int _currentSwaps = 0;
    private bool _isDurationOver = false;
    private bool _doesDealDamageOnMovingIn = false;

    [Space] 
    [SerializeField] private float _moveIntoHeroTime;
    private WaitForSeconds _moveInWait;
    
    [SerializeField] private float _moveOutFromHeroTime;
    private WaitForSeconds _moveOutWait;
    
    [Space]
    [SerializeField] private FollowObject _followObject;
    [SerializeField] private GeneralBossDamageArea _damageArea;
    [SerializeField] private CurveProgression _scaleCurve;
    [SerializeField] private CurveProgression _removalCurve;
    
    private HeroBase _currentTarget;
    private HeroBase _previousTarget;
    
    public override void SetUpProjectile(BossBase bossBase, int newAbilityID)
    {
        SubscribeToEvents();
        
        base.SetUpProjectile(bossBase, newAbilityID);
    }

    public void AdditionalSetUp(HeroBase starterTarget)
    {
        _currentTarget = starterTarget;
        
        _moveInWait = new WaitForSeconds(_moveIntoHeroTime);
        _moveOutWait = new WaitForSeconds(_moveOutFromHeroTime);
        
        _damageArea.ToggleProjectileCollider(false);

        StartMoveOutFromHero();
    }
    
    public void StaticChargeHit(HeroBase heroTarget)
    {
        _currentSwaps++;
        _previousTarget = _currentTarget;
        _currentTarget = heroTarget;
        _doesDealDamageOnMovingIn = true;

        PlayAttackHitAudio();

        _damageArea.ToggleProjectileCollider(false);

        StartMoveIntoHero();
    }

    private void StartMoveIntoHero()
    {
        if (_isDurationOver)
        {
            return;
        }
        
        _scaleCurve.StartMovingDownOnCurve();
        StartCoroutine(MoveIntoHero());
    }

    private IEnumerator MoveIntoHero()
    {
        yield return _moveInWait;

        ReachedMoveInHero();
    }

    private void ReachedMoveInHero()
    {
        if (_doesDealDamageOnMovingIn)
        {
            _mySpecificBoss.DamageHero(_previousTarget, _damageToFollowTargetOnSwap);
        }

        if (_currentSwaps < _maxSwaps && !_isDurationOver)
        {
            SwapTarget(_currentTarget);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void StartMoveOutFromHero()
    {
        _scaleCurve.StartMovingUpOnCurve();
        StartCoroutine(MoveOutFromHero());
    }

    private IEnumerator MoveOutFromHero()
    {
        yield return _moveOutWait;
        ReachedMoveOutFromHero();
    }

    private void ReachedMoveOutFromHero()
    {
        _doesDealDamageOnMovingIn = false;
        _damageArea.ToggleProjectileCollider(true);
    }

    private void SwapTarget(HeroBase heroTarget)
    {
        _followObject.StartFollowingObject(heroTarget.gameObject);
        StartMoveOutFromHero();
    }

    public void DurationOver()
    {
        //StartMoveIntoHero();
        _removalCurve.StartMovingUpOnCurve();
        _isDurationOver = true;
    }

    private void PlayAttackHitAudio()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
                BossAbilityAudio[_abilityID].GeneralAbilityAudio[SBA_StaticCharge.STATIC_CHARGE_ATTACK_HIT_AUDIO_ID]);
    }

    private void SubscribeToEvents()
    {
        _damageArea.GetGeneralHitEvent().AddListener(StaticChargeHit);
    }
}
