using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHP_AlchemistPassiveProjectile : HeroProjectileFramework
{
    [SerializeField] private float _projectileDamage;
    [SerializeField] private float _projectileStagger;

    [Space]
    [SerializeField] private float _moveAwayDistMultiplier;
    [SerializeField] private float _moveAwayTime;
    [SerializeField] private float _waitBeforeTowardsTime;
    [SerializeField] private float _moveTowardsTime;

    public override void SetUpProjectile(HeroBase heroBase)
    {
        base.SetUpProjectile(heroBase);


    }

    public void AdditionalSetup()
    {
        /*PotionTypeSetup();
        StartCoroutine(MovePotionToEndLocation(targetLocation));*/

        StartCoroutine(MoveProcess());
    }


    private IEnumerator MoveProcess()
    {
        Vector3 targetLoc = GameplayManagers.Instance.GetBossManager().GetBossBaseGameObject().transform.position;
        Vector3 startLoc = transform.position;

        float moveTimer = 0;

        while (moveTimer < _moveAwayDistMultiplier)
        {
            moveTimer += Time.deltaTime / _moveAwayTime;
            transform.position = Vector3.LerpUnclamped(startLoc, targetLoc, -Mathf.Pow(moveTimer,2));
            yield return null;
        }

        moveTimer = 0;
        startLoc = transform.position;
        yield return new WaitForSeconds(_waitBeforeTowardsTime);

        while (moveTimer < 1)
        {
            moveTimer += Time.deltaTime / _moveTowardsTime;
            transform.position = Vector3.LerpUnclamped(startLoc,targetLoc, Mathf.Pow(moveTimer,2));
            yield return null;
        }

        EndOfMovement();
    }

    private void EndOfMovement()
    {
        _mySpecificHero.DamageBoss(_projectileDamage);
        _mySpecificHero.StaggerBoss(_projectileStagger);
    }
}
