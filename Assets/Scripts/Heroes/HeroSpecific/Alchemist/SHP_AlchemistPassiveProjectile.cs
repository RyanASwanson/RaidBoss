using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the functionality and movement for the passive projectile created by
/// the alchemist
/// </summary>
public class SHP_AlchemistPassiveProjectile : HeroProjectileFramework
{
    [SerializeField] private float _projectileDamage;
    [SerializeField] private float _projectileStagger;

    [Space]
    [SerializeField] private float _moveTime;

    public override void SetUpProjectile(HeroBase heroBase)
    {
        base.SetUpProjectile(heroBase);

        StartCoroutine(MoveProcess());
    }


    /// <summary>
    /// The process of moving the passive projectile from where its created
    /// to the location of the boss
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveProcess()
    {
        Vector3 targetLoc = GameplayManagers.Instance.GetBossManager().GetBossBaseGameObject().transform.position;
        Vector3 startLoc = transform.position;

        float moveTimer = 0;

        while (moveTimer < 1)
        {
            moveTimer += Time.deltaTime / _moveTime;
            transform.position = Vector3.LerpUnclamped(startLoc,targetLoc, 2 * Mathf.Pow(moveTimer,2) - moveTimer);
            yield return null;
        }

        EndOfMovement();
    }


    /// <summary>
    /// Is called when the projectile reaches the location of the boss
    /// </summary>
    private void EndOfMovement()
    {
        _mySpecificHero.DamageBoss(_projectileDamage);
        _mySpecificHero.StaggerBoss(_projectileStagger);

        Destroy(gameObject);
    }
}
