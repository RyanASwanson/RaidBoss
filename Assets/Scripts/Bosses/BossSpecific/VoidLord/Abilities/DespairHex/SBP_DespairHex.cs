using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SBP_DespairHex : BossProjectileFramework
{
    [Space]
    [SerializeField] private float _projectileDuration;
    [SerializeField] private float _enrageDurationIncrease;
    [SerializeField] private float _durationIncreaseOnHeroHit;
    private Coroutine _durationCoroutine;
    private bool _isDurationOver = false;
    
    [Space] 
    [SerializeField] private float _moveIntoHeroTime;
    private WaitForSeconds _moveInWait;
    
    [SerializeField] private float _moveOutFromHeroTime;
    private WaitForSeconds _moveOutWait;

    [SerializeField] private float _moveOutColliderEnableDelay;
    private WaitForSeconds _moveOutColliderEnableWait;
    
    [SerializeField] private float _ignorePreviousTargetTime;
    private WaitForSeconds _ignorePreviousTargetWait;
    private Coroutine _ignorePreviousTargetCoroutine;
    private HeroBase _ignoreTarget;

    [Space] 
    [SerializeField] private float _moveDelay;
    [SerializeField] private float _projectileMoveCycleTime;
    [SerializeField] private AnimationCurve _projectileMoveCycleCurve;
    private WaitForSeconds _projectileMoveDelayWait;
    
    [Space]
    [SerializeField] private float _projectileDistanceMultiplier;
    [SerializeField] private Vector3[] _projectileMoveVectors;
    [SerializeField] private GameObject[] _projectiles;
    [SerializeField] private LookAtObject[] _inwardParticles;
    [SerializeField] private LookAtObject _sigil;
    private int[] _projectileMovementStartIndices;
    private int[] _projectileDestinationMovementIndices;
    private Coroutine _projectileMovementCoroutine;
    private float _currentMovementProgress;
    
    [Space] 
    [SerializeField] private FollowObject _followObject;
    [SerializeField] private GeneralBossDamageArea _damageArea;
    [SerializeField] private CurveProgression _scaleCurve;
    [SerializeField] private CurveProgression _removalCurve;

    private HeroBase _currentTarget;
    private HeroBase _previousTarget;
    
    private SBA_DespairHex _despairHex;
    
    public void AdditionalSetUp(SBA_DespairHex despairHex, HeroBase followTarget)
    {
        _despairHex = despairHex;
        _currentTarget = followTarget;
        
        _moveInWait = new WaitForSeconds(_moveIntoHeroTime);
        _moveOutWait = new WaitForSeconds(_moveOutFromHeroTime);
        _moveOutColliderEnableWait = new WaitForSeconds(_moveOutColliderEnableDelay);
        _ignorePreviousTargetWait = new WaitForSeconds(_ignorePreviousTargetTime);
        
        _projectileMoveDelayWait = new WaitForSeconds(_moveDelay);
        
        for(int i =0; i < _projectileMoveVectors.Length; i++)
        {
            _projectileMoveVectors[i] *= _projectileDistanceMultiplier;
        }
        
        _projectileMovementStartIndices = new int[_projectiles.Length];
        _projectileMovementStartIndices[0] = 0;
        _projectileMovementStartIndices[1] = (_projectileMoveVectors.Length) / 2;
        
        _projectileDestinationMovementIndices = new int[_projectiles.Length];
        DetermineProjectileMovementDestinations();

        SetInwardVFXTarget(followTarget);
        
        _sigil.StartLookingAtObject(_projectiles[0].transform,true);
            
        StartProjectileMovement();
        
        _damageArea.ToggleProjectileCollider(false);
        _followObject.StartFollowingObject(followTarget.gameObject);
        
        _despairHex.AddHexedHero(followTarget);

        StartMoveOutFromHero();
        StartProjectileDuration();
    }
    
    private void OnDestroy()
    {
        UnsubscribeFromEvents();
        
        _despairHex.RemoveHexedHero(_currentTarget);
    }
    
    #region ProjectileMovement

    private void StartProjectileMovement()
    {
        StopProjectileMovement();
        
        _projectileMovementCoroutine = StartCoroutine(ProjectileMovement());
    }

    private void StopProjectileMovement()
    {
        if (!_projectileMovementCoroutine.IsUnityNull())
        {
            StopCoroutine(_projectileMovementCoroutine);
        }
    }

    private IEnumerator ProjectileMovement()
    {
        while (true)
        {
            yield return _projectileMoveDelayWait;
            
            while (_currentMovementProgress < 1)
            {
                _currentMovementProgress += Time.deltaTime / _projectileMoveCycleTime;
                
                for (int i = 0; i < _projectiles.Length; i++)
                {
                    _projectiles[i].transform.localPosition = Vector3.Lerp(
                        _projectileMoveVectors[_projectileMovementStartIndices[i]],
                        _projectileMoveVectors[_projectileDestinationMovementIndices[i]],
                        _projectileMoveCycleCurve.Evaluate(_currentMovementProgress));
                }
                
                yield return null;
            }

            _currentMovementProgress -= 1;
            IncreaseProjectileMovementStartLocations();
        }
    }

    private void IncreaseProjectileMovementStartLocations()
    {
        for (int i = 0; i < _projectileMovementStartIndices.Length; i++)
        {
            _projectileMovementStartIndices[i]++;
            
            if (_projectileMovementStartIndices[i] >= _projectileMoveVectors.Length)
            {
                _projectileMovementStartIndices[i] = 0;
            }
        }

        DetermineProjectileMovementDestinations();
    }

    private void DetermineProjectileMovementDestinations()
    {
        for (int i = 0; i < _projectileMovementStartIndices.Length; i++)
        {
            _projectileDestinationMovementIndices[i] = _projectileMovementStartIndices[i] + 1;
            
            if (_projectileDestinationMovementIndices[i] >= _projectileMoveVectors.Length)
            {
                _projectileDestinationMovementIndices[i] = 0;
            }
        }
    }
    #endregion
    
    #region VFX

    private void SetInwardVFXTarget(HeroBase followTarget)
    {
        for (int i = 0; i < _inwardParticles.Length; i++)
        {
            _inwardParticles[i].StartLookingAtObject(followTarget.transform,true);
        }
    }
    #endregion

    public void HexHitHero(HeroBase target)
    {
        if (target == _ignoreTarget)
        {
            return;
        }
        
        _previousTarget = _currentTarget;
        _currentTarget = target;
        
        _projectileDuration += _durationIncreaseOnHeroHit;

        StartIgnorePreviousTarget(_previousTarget);
        _damageArea.ToggleProjectileCollider(false);

        PlayHexHitAudio();
        
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
        if (!_isDurationOver)
        {
            SwapTarget(_currentTarget);
        }
        else
        {
            _despairHex.RemoveHexedHero(_currentTarget);
            Destroy(gameObject);
        }
    }

    private void StartMoveOutFromHero()
    {
        SetInwardVFXTarget(_currentTarget);
        _scaleCurve.StartMovingUpOnCurve();
        PlayHexSwapAudio();
        
        StartCoroutine(MoveOutFromHero());
    }

    private IEnumerator MoveOutFromHero()
    {
        yield return _moveOutWait;
        ReachedMoveOutFromHero();

        yield return _moveOutColliderEnableWait;
        ColliderReenabledOnMovingOut();
    }

    private void ReachedMoveOutFromHero()
    {

    }

    private void ColliderReenabledOnMovingOut()
    {
        _damageArea.ToggleProjectileCollider(true);
    }

    private void StartIgnorePreviousTarget(HeroBase target)
    {
        StopIgnorePreviousTarget();
        
        _ignoreTarget = target;

        _ignorePreviousTargetCoroutine = StartCoroutine(IgnorePreviousTargetProcess());
    }

    private void StopIgnorePreviousTarget()
    {
        if (!_ignorePreviousTargetCoroutine.IsUnityNull())
        {
            StopCoroutine(_ignorePreviousTargetCoroutine);
        }
    }

    private IEnumerator IgnorePreviousTargetProcess()
    {
        yield return _ignorePreviousTargetWait;
        _ignoreTarget = null;
    }

    private void SwapTarget(HeroBase heroTarget)
    {
        _despairHex.RemoveHexedHero(_previousTarget);
        _despairHex.AddHexedHero(heroTarget);
        
        _followObject.StartFollowingObject(heroTarget.gameObject);
        StartMoveOutFromHero();
    }

    public void StartProjectileDuration()
    {
        _durationCoroutine = StartCoroutine(ProjectileDuration());
    }

    private IEnumerator ProjectileDuration()
    {
        float duration = 0;

        while (duration < _projectileDuration)
        {
            duration += Time.deltaTime;
            yield return null;
        }
        
        DurationOver();
    }

    public void ForceEndDuration()
    {
        if (_isDurationOver)
        {
            return;
        }
        
        if (!_durationCoroutine.IsUnityNull())
        {
            StopCoroutine(_durationCoroutine);
        }
        
        DurationOver();
    }

    private void DurationOver()
    {
        _damageArea.ToggleProjectileCollider(false);
        _isDurationOver = true;
        _removalCurve.StartMovingUpOnCurve();
    }

    public void RemovalConcluded()
    {
        _despairHex.RemoveHexedHero(_currentTarget);
        Destroy(gameObject);
    }
    
    private void PlayHexSwapAudio()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
                BossAbilityAudio[_abilityID].GeneralAbilityAudio[SBA_DespairHex.DESPAIR_HEX_SWAP_AUDIO_ID]);
    }

    private void PlayHexHitAudio()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
                BossAbilityAudio[_abilityID].GeneralAbilityAudio[SBA_DespairHex.DESPAIR_HEX_HIT_AUDIO_ID]);
    }
    
    private void SubscribeToEvents()
    {
        _damageArea.GetGeneralHitEvent().AddListener(HexHitHero);
    }

    private void UnsubscribeFromEvents()
    {
        _damageArea.GetGeneralHitEvent().RemoveListener(HexHitHero);
    }
    
    #region Base Ability
    public override void SetUpProjectile(BossBase bossBase, int newAbilityID, bool wasEnragedOnAbilityActivation)
    {
        SubscribeToEvents();

        if (wasEnragedOnAbilityActivation)
        {
            _projectileDuration += _enrageDurationIncrease;
        }
        
        base.SetUpProjectile(bossBase, newAbilityID, wasEnragedOnAbilityActivation);
    }
    #endregion
}
