using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SH_Mirage : SpecificHeroFramework
{
    [Space]
    [SerializeField] private GameObject _basicProjectile;

    [SerializeField] private GameObject _manualClone;
    private const float _cloneSpawnOffset = -2;

    private HeroBase _cloneBase;
    private MirageClone _associatedCloneFunc;


    #region Basic Abilities

    #endregion

    #region Manual Abilities
    private void CreateClone()
    {
        Vector3 spawnLocation = _myHeroBase.transform.position + (_myHeroBase.transform.forward * _cloneSpawnOffset);
        /*GameObject cloneObject = Instantiate(_manualClone, spawnLocation, _myHeroBase.transform.rotation);

        _cloneBase = cloneObject.GetComponent<HeroBase>();
        _cloneBase.SetupChildren();*/

        //Transform cloneTransform = _myHeroBase.transform;
        //cloneTransform.transform.position += _myHeroBase.transform.forward * _cloneSpawnOffset;

        /*GameplayManagers.Instance.GetHeroesManager().CreateHeroBase(spawnLocation,
            _myHeroBase.transform.rotation, _myHeroBase.GetHeroSO());*/
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

    public override void SetupSpecificHero(HeroBase heroBase, HeroSO heroSO)
    {
        base.SetupSpecificHero(heroBase, heroSO);
        CreateClone();
    }


}
