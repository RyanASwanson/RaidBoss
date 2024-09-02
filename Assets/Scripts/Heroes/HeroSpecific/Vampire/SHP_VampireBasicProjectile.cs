using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the functionality to the Vampire basic ability projectile
/// </summary>
public class SHP_VampireBasicProjectile : HeroProjectileFramework
{
    [SerializeField] private float _projectileSpeed;
    [Space]

    [SerializeField] private bool _canSplit;
    [SerializeField] private float _splitProjectileCount;
    [SerializeField] private float _splitAngle;

    [Space]
    [SerializeField] private GameObject _splitProjectile;

    [Space]
    [SerializeField] private Animator _projectileAnimator;

    private const string BASIC_ATTACK_ANIM_TRIGGER = "BasicAttack";
    private const string SPLIT_ATTACK_ANIM_TRIGGER = "SplitAttack";

    private SH_Vampire _vampireHero;

    


    /// <summary>
    /// Triggers different animations based on if the projectile is the original
    /// or a split copy
    /// </summary>
    private void StartAnimations()
    {
        if (_canSplit)
            _projectileAnimator.SetTrigger(BASIC_ATTACK_ANIM_TRIGGER);
        else
            _projectileAnimator.SetTrigger(SPLIT_ATTACK_ANIM_TRIGGER);
    }

    /// <summary>
    /// Moves the projectile along its path
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveProjectile()
    {
        while (true)
        {
            transform.position += transform.forward * _projectileSpeed * Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// Splits the projectile into several other projectiles
    /// </summary>
    public void SplitProjectile()
    {

        for(int i = 0; i < _splitProjectileCount; i++)
        {
            float tempRot = Mathf.Lerp(-_splitAngle, _splitAngle, i / (_splitProjectileCount-1));
            
            Vector3 projRotation = new Vector3(0,tempRot,0) + transform.eulerAngles;

            GameObject newestProjectile = Instantiate(_splitProjectile, transform.position, Quaternion.Euler(projRotation));

            SHP_VampireBasicProjectile projectileFunc = newestProjectile.GetComponent<SHP_VampireBasicProjectile>();

            projectileFunc.SetUpProjectile(_myHeroBase);
            projectileFunc.AdditionalSetup(_vampireHero);

            //Performs the setup for the damage area so that it knows it's owner
            newestProjectile.GetComponent<GeneralHeroDamageArea>().SetUpDamageArea(_myHeroBase);
        }
    }

    /// <summary>
    /// Triggers the passive of the vampire
    /// Called when the projectile does damage
    /// </summary>
    /// <param name="damage"></param>
    public void TriggerHeroPassive(float damage)
    {
        _vampireHero.AddToPassiveHealingCounter(damage);
    }


    #region Base Ability
    public override void SetUpProjectile(HeroBase heroBase)
    {
        base.SetUpProjectile(heroBase);

        StartAnimations();

        StartCoroutine(MoveProjectile());
    }

    public void AdditionalSetup(SH_Vampire heroScript)
    {
        _vampireHero = heroScript;
    }
    #endregion
}
