using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SH_Mirage : SpecificHeroFramework
{
    [Space]
    [SerializeField] private GameObject _basicProjectile;
    [SerializeField] private GameObject _basicTargetZone;
    private GameObject _currentBasicTargetZone;

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
            _currentBasicTargetZone.transform.eulerAngles = new Vector3(_currentBasicTargetZone.transform.eulerAngles.x,
                0, _currentBasicTargetZone.transform.eulerAngles.z);
            yield return null;
        }
    }

    private Vector3 FindHeroCloneMidpoint()
    {
        return Vector3.Lerp(transform.position, _cloneBase.transform.position, .5f);
    }
    #endregion

    #region Manual Abilities
    private void CreateClone()
    {
        Vector3 spawnLocation = _myHeroBase.transform.position + (_myHeroBase.transform.forward * _cloneSpawnOffset);

        _cloneBase = GameplayManagers.Instance.GetHeroesManager().CreateHeroBase(spawnLocation,
            _myHeroBase.transform.rotation, _cloneSO);
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
