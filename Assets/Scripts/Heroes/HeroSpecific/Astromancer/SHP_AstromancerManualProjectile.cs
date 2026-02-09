using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHP_AstromancerManualProjectile : HeroProjectileFramework
{
    [SerializeField] private float _projectileDamage;
    [SerializeField] private float _projectileStagger;

    [Space]
    [SerializeField] private float _moveTime;
    [SerializeField] private AnimationCurve _moveTowardsTargetCurve;
    
    [Space]
    [SerializeField] private float _offsetAmount;
    [SerializeField] private AnimationCurve _offsetCurve;
    private Vector3 _offset;
    
    [Space] 
    [SerializeField] private float _minHeight;
    [SerializeField] private float _maxHeight;
    [SerializeField] private AnimationCurve _moveVerticalCurve;

    [Space] 
    [SerializeField] private GameObject _visualsHolder;
    [SerializeField] private GeneralHeroDamageArea _damageArea;
    
    
    /// <summary>
    /// The process of moving the passive projectile from where its created
    /// to the location of the boss
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveProcess()
    {
        Vector3 targetLoc = BossBase.Instance.transform.position;
        Vector3 startLoc = transform.position;
        
        Vector3 direction = (targetLoc - startLoc).normalized;
        
        transform.LookAt(targetLoc);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        float moveTimer = 0;

        while (moveTimer < 1)
        {
            moveTimer += Time.deltaTime / _moveTime;

            Vector3 moveTowardsLocation =
                Vector3.LerpUnclamped(startLoc, targetLoc, _moveTowardsTargetCurve.Evaluate(moveTimer));
            
            _visualsHolder.transform.localPosition = _offset * _offsetCurve.Evaluate(moveTimer);
            
            float verticalLocation = Mathf.Lerp(_minHeight, _maxHeight, _moveVerticalCurve.Evaluate(moveTimer));
            
            //transform.position = new Vector3(sideOffset.x, verticalLocation, sideOffset.z);
            // Sets the location of the projectile
            transform.position = new Vector3(moveTowardsLocation.x, verticalLocation, moveTowardsLocation.z);
            
            // Makes the visuals look at the target
            _visualsHolder.transform.LookAt(targetLoc);
            _visualsHolder.transform.eulerAngles = new Vector3(0, _visualsHolder.transform.eulerAngles.y, 0);
            
            
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

        _damageArea.DestroyProjectile();
    }

    public void AdditionalSetUp(bool isRight)
    {
        if (isRight)
        {
            _offset = new Vector3(_offsetAmount,0,0);
        }
        else
        {
            _offset = new Vector3(-_offsetAmount,0,0);
        }
    }

    #region Base Ability
    public override void SetUpProjectile(HeroBase heroBase, EHeroAbilityType heroAbilityType)
    {
        base.SetUpProjectile(heroBase, heroAbilityType);

        StartCoroutine(MoveProcess());
    }
    #endregion
}
