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
    [SerializeField] private NavMeshObstacle _navMeshObstacle;
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
            CreateNavMeshObstacle();
    }
    
    private void DisableHitboxes()
    {
        foreach (GeneralBossDamageArea bossDamageArea in _closingWalls)
        {
            bossDamageArea.enabled = false;
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

    private void CreateNavMeshObstacle()
    {
        _navMeshObstacle.enabled = true;

        StartCoroutine(RemovalProcess());
    }

    private IEnumerator RemovalProcess()
    {
        yield return new WaitForSeconds(_entombPersistantTime);
        Destroy(gameObject);
    }
}
