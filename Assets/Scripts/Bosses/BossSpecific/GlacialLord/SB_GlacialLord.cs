using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SB_GlacialLord : SpecificBossFramework
{
    [Space]
    [Header("Ice Crown")]
    [SerializeField] private Vector3 _iceCrownSpawnLocation;
    [SerializeField] private GameObject _iceCrown;

    private GlacialLordIceCrown _currentIceCrown;

    [Header("Passive Attack (Name Pending)")]
    [SerializeField] private GameObject _passiveAttack;
    [SerializeField] private GameObject _passiveAttackTargetZone;

    protected override void StartFight()
    {
        CreateIceCrown();
        base.StartFight();
    }

    #region Ice Crown
    private void CreateIceCrown()
    {
        GameObject crown = Instantiate(_iceCrown, _iceCrownSpawnLocation, Quaternion.identity);

        _currentIceCrown = crown.GetComponent<GlacialLordIceCrown>();

        _currentIceCrown.Setup();
    }
    #endregion

    #region Passive Attack (NAME PENDING)
    
    #endregion


    #region Getters
    public Vector3 GetIceCrownDirection()
    {
        Vector3 crownPos = _currentIceCrown.transform.position;
        Vector3 bossPos = transform.position;

        crownPos = new Vector3(crownPos.x, 0, crownPos.z);
        bossPos = new Vector3(bossPos.x, 0, bossPos.z);

        return (crownPos - bossPos).normalized;
    }

    public bool DoesIceCrownHaveHeroOwner() =>_currentIceCrown.DoesCrownHaveOwner();

    public HeroBase GetIceCrownHeroOwner() => _currentIceCrown.GetCrownHeroOwner();


    public Vector3 CrownLocationClosestToFloor() => _currentIceCrown.GetClosestCrownLocationToFloor();

    #endregion
}
