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

    private const string _basicAttackAnim = "BasicAttack";
    private const string _splitAttackAnim = "SplitAttack";

    private SH_Vampire _vampireScript;

    public override void SetUpProjectile(HeroBase heroBase)
    {
        base.SetUpProjectile(heroBase);

        StartAnimations();

        StartCoroutine(MoveProjectile());
    }

    public void AdditionalSetup(SH_Vampire heroScript)
    {
        _vampireScript = heroScript;
    }


    private void StartAnimations()
    {
        if (_canSplit)
            _projectileAnimator.SetTrigger(_basicAttackAnim);
        else
            _projectileAnimator.SetTrigger(_splitAttackAnim);
    }

    private IEnumerator MoveProjectile()
    {
        while (true)
        {
            transform.position += transform.forward * _projectileSpeed * Time.deltaTime;
            yield return null;
        }
    }

    public void SplitProjectile()
    {

        for(int i = 0; i < _splitProjectileCount; i++)
        {
            float tempRot = Mathf.Lerp(-_splitAngle, _splitAngle, i / (_splitProjectileCount-1));
            
            Vector3 projRotation = new Vector3(0,tempRot,0) + transform.eulerAngles;

            GameObject newestProjectile = Instantiate(_splitProjectile, transform.position, Quaternion.Euler(projRotation));

            SHP_VampireBasicProjectile projectileFunc = newestProjectile.GetComponent<SHP_VampireBasicProjectile>();
            projectileFunc.SetUpProjectile(_myHeroBase);
            projectileFunc.AdditionalSetup(_vampireScript);

            newestProjectile.GetComponent<GeneralHeroDamageArea>().SetUpDamageArea(_myHeroBase);
        }
    }

    public void TriggerHeroPassive(float damage)
    {
        _vampireScript.AddToPassiveHealingCounter(damage);
    }
}
