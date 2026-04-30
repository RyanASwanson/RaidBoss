using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// Provides the functionality for the Terra Lord's Entomb ability projectiles
/// </summary>
public class SBP_Entomb : BossProjectileFramework
{
    [SerializeField] private float _entombCompleteDelay;
    [SerializeField] private float _entombPersistantTime;
    [SerializeField] private float _entombEnragePersistantTimeMultiplier;
    [SerializeField] private float _enrageWallDamageMultiplier;

    [Space]
    [SerializeField] private List<GeneralBossDamageArea> _closingWalls;

    [Space] 
    [SerializeField] private TerraLordUniversalEnvironmentalWeightObject _environmentalWeight;

    [Space]
    [SerializeField] protected CinemachineCameraShakeData _screenShakeData;
    [SerializeField] private GameObject _closedParticleVFX;

    [Space]
    [SerializeField] private NavMeshObstacle _navMeshObstacle;
    [SerializeField] private Collider _environmentCollider;

    [SerializeField] private Animator _animator;
    
    private SBP_Entomb _alternateEntomb;
    private bool _hasEntombBeenCompleted = false;

    private const string REMOVE_PROJECTILE_ANIM_TRIGGER = "RemoveEntomb";

    private IEnumerator AbilityProcess()
    {
        yield return new WaitForSeconds(_entombCompleteDelay);

        EntombComplete();
    }

    /// <summary>
    /// Called when entomb is complete to determine what to do next
    /// </summary>
    private void EntombComplete()
    {
        DisableHitboxes();

        if (CanCreateObstacle())
        {
            if (HasAlternateEntomb() && !_alternateEntomb.HasEntombBeenCompleted())
            {
                _alternateEntomb.DestroyRemainingWalls();
            }
            
            PlayEntombClosedScreenShake();
            PlayEntombClosedSound();
            CreateNavMeshObstacle();
            _environmentalWeight.AddObjectToTerraLordList();
            Instantiate(_closedParticleVFX, new Vector3(transform.position.x,0,transform.position.z), transform.rotation);
        }
        else
        {
            DestroyRemainingWalls();
        }

        _hasEntombBeenCompleted = true;
    }
    
    /// <summary>
    /// Disables the hit boxes of the closing walls
    /// </summary>
    private void DisableHitboxes()
    {
        foreach (GeneralBossDamageArea damageArea in _closingWalls)
        {
            if (!damageArea.IsUnityNull())
            {
                damageArea.enabled = false;
            }
        }
    }

    /// <summary>
    /// Determines if an obstacle can be created based on if both walls are still active
    /// </summary>
    /// <returns></returns>
    private bool CanCreateObstacle()
    {
        foreach(GeneralBossDamageArea damageArea in _closingWalls)
        {
            if (damageArea.IsUnityNull())
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Destroys any walls that are still remaining
    /// </summary>
    public void DestroyRemainingWalls()
    {
        foreach (GeneralBossDamageArea damageArea in _closingWalls)
        {
            if (!damageArea.IsUnityNull())
            {
                PlayEntombDestroyHalfSound();
                damageArea.DestroyProjectile();
            }
        }
    }

    private void MultiplyWallDamage(float multiplier)
    {
        foreach (GeneralBossDamageArea damageArea in _closingWalls)
        {
            if (!damageArea.IsUnityNull())
            {
                damageArea.MultiplyDamageMultiplier(multiplier);
            }
        }
    }

    /// <summary>
    /// Creates the obstacle in the environment that heroes must navigate around
    /// </summary>
    private void CreateNavMeshObstacle()
    {
        _navMeshObstacle.enabled = true;
        _environmentCollider.enabled = true;

        StartCoroutine(RemovalProcess());
    }

    private void PlayEntombClosedScreenShake()
    {
        CameraGameManager.Instance.StartCameraShake(_screenShakeData);
    }

    private void PlayEntombClosedSound()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
                BossAbilityAudio[_abilityID].GeneralAbilityAudio[SBA_Entomb.ENTOMB_CLOSED_IMPACT_AUDIO_ID]);
    }

    private void PlayEntombDestroyHalfSound()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
                BossAbilityAudio[_abilityID].GeneralAbilityAudio[SBA_Entomb.ENTOMB_DESTROY_HALF_AUDIO_ID]);
    }

    /// <summary>
    /// The process of removing the entomb obstacle
    /// </summary>
    /// <returns></returns>
    private IEnumerator RemovalProcess()
    {
        if (_wasBossEnragedOnAbilityActivation)
        {
            _entombPersistantTime *= _entombEnragePersistantTimeMultiplier;
        }
        
        yield return new WaitForSeconds(_entombPersistantTime);

        _animator.SetTrigger(REMOVE_PROJECTILE_ANIM_TRIGGER);
    }

    #region Base Ability
    public override void SetUpProjectile(BossBase bossBase, int newAbilityID)
    {
        base.SetUpProjectile(bossBase, newAbilityID);
        StartCoroutine(AbilityProcess());
    }

    public void AdditionalSetUp(SBP_Entomb alternateEntomb)
    {
        _alternateEntomb = alternateEntomb;

        MultiplyWallDamage(_enrageWallDamageMultiplier);
    }
    #endregion
    
    #region Getters
    public bool HasAlternateEntomb()
    {
        return !_alternateEntomb.IsUnityNull();
    }
    
    public bool HasEntombBeenCompleted() => _hasEntombBeenCompleted;
    #endregion
}
