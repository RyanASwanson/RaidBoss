using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    private const string _removalAnimation = "RemoveEntomb";

    public override void SetUpProjectile(BossBase bossBase)
    {
        StartCoroutine(AbilityProcess());
        base.SetUpProjectile(bossBase);
    }

    private IEnumerator AbilityProcess()
    {
        yield return new WaitForSeconds(_entombCompleteDelay);

        EntombComplete();
    }

    private void EntombComplete()
    {
        DisableHitboxes();

        if (CanCreateObstacle())
        {
            CreateNavMeshObstacle();
            Instantiate(_closedParticleVFX, new Vector3(transform.position.x,0,transform.position.z), transform.rotation);
        }
            
        else
            DestroyRemainingWall();

    }
    
    private void DisableHitboxes()
    {
        foreach (GeneralBossDamageArea damageArea in _closingWalls)
        {
            if(damageArea != null)
                damageArea.enabled = false;
        }
    }

    private bool CanCreateObstacle()
    {
        foreach(GeneralBossDamageArea damageArea in _closingWalls)
        {
            if (damageArea == null)
                return false;
        }
        return true;
    }

    private void DestroyRemainingWall()
    {
        foreach (GeneralBossDamageArea damageArea in _closingWalls)
        {
            if (damageArea != null)
                damageArea.DestroyProjectile();
        }
    }

    private void CreateNavMeshObstacle()
    {
        _navMeshObstacle.enabled = true;

        StartCoroutine(RemovalProcess());
    }

    private IEnumerator RemovalProcess()
    {
        yield return new WaitForSeconds(_entombPersistantTime);

        _animator.SetTrigger(_removalAnimation);
    }
}
