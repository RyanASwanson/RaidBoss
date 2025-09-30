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

    [Space]
    [SerializeField] private List<GeneralBossDamageArea> _closingWalls;

    [Space]
    [SerializeField] private GameObject _closedParticleVFX;

    [Space]
    [SerializeField] private NavMeshObstacle _navMeshObstacle;

    [SerializeField] private Animator _animator;

    private const string REMOVE_PROJECTILE_ANIM_TRIGGER = "RemoveEntomb";
    
    private const int ENTOMB_CLOSED_IMPACT_AUDIO_ID = 0;

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
            PlayEntombClosedSound();
            CreateNavMeshObstacle();
            Instantiate(_closedParticleVFX, new Vector3(transform.position.x,0,transform.position.z), transform.rotation);
        }
        else
        {
            DestroyRemainingWall();
        }
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
    private void DestroyRemainingWall()
    {
        foreach (GeneralBossDamageArea damageArea in _closingWalls)
        {
            if (!damageArea.IsUnityNull())
            {
                damageArea.DestroyProjectile();
            }
        }
    }

    /// <summary>
    /// Creates the obstacle in the environment that heroes must navigate around
    /// </summary>
    private void CreateNavMeshObstacle()
    {
        _navMeshObstacle.enabled = true;

        StartCoroutine(RemovalProcess());
    }

    private void PlayEntombClosedSound()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
                BossAbilityAudio[_abilityID].GeneralAbilityAudio[ENTOMB_CLOSED_IMPACT_AUDIO_ID]);
    }

    /// <summary>
    /// The process of removing the entomb obstacle
    /// </summary>
    /// <returns></returns>
    private IEnumerator RemovalProcess()
    {
        yield return new WaitForSeconds(_entombPersistantTime);

        _animator.SetTrigger(REMOVE_PROJECTILE_ANIM_TRIGGER);
    }

    #region Base Ability
    public override void SetUpProjectile(BossBase bossBase, int newAbilityID)
    {
        base.SetUpProjectile(bossBase, newAbilityID);
        StartCoroutine(AbilityProcess());
    }
    #endregion
}
