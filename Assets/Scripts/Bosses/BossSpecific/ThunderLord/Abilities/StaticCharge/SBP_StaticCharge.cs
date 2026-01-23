using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_StaticCharge : BossProjectileFramework
{
    [SerializeField] private float _damageToFollowTargetOnSwap;
    [SerializeField] private int _maxSwaps;
    private int _currentSwaps = 0;

    [Space] 
    [SerializeField] private float _moveIntoHeroTime;
    private WaitForSeconds _moveInWait;
    
    [Space]
    [SerializeField] private float _swapTime;
    private WaitForSeconds _swapWait;
    
    [Space]
    [SerializeField] private FollowObject _followObject;
    [SerializeField] private GeneralBossDamageArea _damageArea;
    
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
        _swapWait = new WaitForSeconds(_swapTime);
    }
    
    public void StaticChargeHit(HeroBase heroTarget)
    {
        _currentSwaps++;
        _previousTarget = _currentTarget;
        _currentTarget = heroTarget;
        
        _damageArea.StartDisableColliderForDuration(_swapWait);

        StartMoveIntoHero();
    }

    private void StartMoveIntoHero()
    {
        StartCoroutine(MoveIntoHero());
    }

    private IEnumerator MoveIntoHero()
    {
        yield return _moveInWait;

        ReachedMoveInHero();
    }

    private void ReachedMoveInHero()
    {
        _previousTarget.GetHeroStats().DealDamageToHero(_damageToFollowTargetOnSwap);
        if (_currentSwaps < _maxSwaps)
        {
            SwapTarget(_currentTarget);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SwapTarget(HeroBase heroTarget)
    {
        _followObject.StartFollowingObject(heroTarget.gameObject);
    }

    public void DurationOver()
    {
        Destroy(gameObject);
    }

    private void SubscribeToEvents()
    {
        _damageArea.GetGeneralHitEvent().AddListener(StaticChargeHit);
    }
}
