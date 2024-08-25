using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SH_Mirage : SpecificHeroFramework
{
    [Space]
    [SerializeField] private GameObject _basicProjectile;
    [SerializeField] private GameObject _basicTargetZone;
    private GameObject _currentBasicTargetZone;
    private const float _targetZoneYOffset = -.5f;

    [SerializeField] private HeroSO _cloneSO;
    [SerializeField] private GameObject _manualClone;
    private const float _cloneSpawnOffset = -2;

    private HeroBase _cloneBase;


    #region Basic Abilities
    private void CreateBasicTargetZone()
    {
        _currentBasicTargetZone = Instantiate(_basicTargetZone, FindHeroCloneMidpoint(), Quaternion.identity);

        StartCoroutine(MoveBasicTargetZone());
    }

    private IEnumerator MoveBasicTargetZone()
    {
        while(this != null && _currentBasicTargetZone != null)
        {
            _currentBasicTargetZone.transform.position = FindHeroCloneMidpoint();

            _currentBasicTargetZone.transform.LookAt(transform.position);
            _currentBasicTargetZone.transform.eulerAngles = new Vector3
                (0,_currentBasicTargetZone.transform.eulerAngles.y, 0);
            yield return null;
        }
    }

    private Vector3 FindHeroCloneMidpoint()
    {
        Vector3 tempVector = Vector3.Lerp(transform.position, _cloneBase.transform.position, .5f);
        tempVector = new Vector3(tempVector.x, tempVector.y + _targetZoneYOffset, tempVector.z);
        return tempVector;
    }

    public override void ActivateBasicAbilities()
    {
        CreateBasicAbilityProjectile();
        base.ActivateBasicAbilities();
    }

    public void CreateBasicAbilityProjectile()
    {
        GameObject _newestProjectile = Instantiate(_basicProjectile, 
            _currentBasicTargetZone.transform.position, _currentBasicTargetZone.transform.rotation);

        _newestProjectile.GetComponent<GeneralHeroDamageArea>().SetUpDamageArea(_myHeroBase);
    }
    #endregion

    #region Manual Abilities
    private void CreateClone()
    {
        Vector3 spawnLocation = _myHeroBase.transform.position + (_myHeroBase.transform.forward * _cloneSpawnOffset);

        _cloneBase = GameplayManagers.Instance.GetHeroesManager().CreateHeroBase(spawnLocation,
            _myHeroBase.transform.rotation, _cloneSO);

        ((MirageClone)_cloneBase.GetSpecificHeroScript()).AdditionalSetup(this);
    }

    public override void ActivateManualAbilities(Vector3 attackLocation)
    {
        base.ActivateManualAbilities(attackLocation);
        MoveClone(attackLocation);
    }

    private void MoveClone(Vector3 moveLocation)
    {
        _cloneBase.GetPathfinding().DirectNavigationTo(moveLocation);
    }

    private void CloneBasicAbility()
    {
        CreateBasicAbilityProjectile();
    }
    #endregion

    #region Passive Abilities

    #endregion

    protected override void BattleStarted()
    {
        CreateBasicTargetZone();
        base.BattleStarted();
    }

    public override void SetupSpecificHero(HeroBase heroBase, HeroSO heroSO)
    {
        base.SetupSpecificHero(heroBase, heroSO);
        CreateClone();
    }



}
